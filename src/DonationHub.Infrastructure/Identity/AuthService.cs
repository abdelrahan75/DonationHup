using DonationHub.Infrastructure.data;
using DonationHup.Application.Auth;
using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces.Repositories;
using DonationHup.Application.Optians;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly AppDbContext _context;
        private readonly IGenericRepository<RefreshToken> _refreshTokens;
        private readonly IUnitofWork _unitOfWork;
        private readonly JwtOptians _jwt;

        public AuthService(
            UserManager<ApplicationUser> users,
            AppDbContext context,
            IGenericRepository<RefreshToken> refreshTokens,
            IUnitofWork unitOfWork,
            IOptions<JwtOptians> jwtOptions)
        {
            _users = users;
            _context = context;
            _refreshTokens = refreshTokens;
            _unitOfWork = unitOfWork;
            _jwt = jwtOptions.Value;
        }

        public Task<UserProfileDto> RegisterAsync(RegisterRequest request) =>
            CreateUserAsync(request.Email, request.FullName, request.Password, request.PreferredLanguage, Roles.Donor);

        public Task<UserProfileDto> CreateStaffAsync(CreateStaffRequest request) =>
            CreateUserAsync(request.Email, request.FullName, request.TemporaryPassword, request.PreferredLanguage, request.Role);

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _users.FindByEmailAsync(request.Email.Trim());

            if (user is null || !await _users.CheckPasswordAsync(user, request.Password))
                throw new AuthException("Auth.InvalidCredentials", "Invalid email or password.", 401);

            if (!user.IsActive)
                throw new AuthException("Auth.AccountDeactivated", "This account is deactivated.", 403);

            return await CreateTokensAsync(user, null);
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            var hash = Hash(refreshToken);
            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
            var user = stored is null ? null : await _users.FindByIdAsync(stored.UserId.ToString());

            if (stored is null || user is null || stored.RevokedAt is not null || stored.ExpiresAt <= DateTime.UtcNow)
                throw new AuthException("Auth.RefreshTokenInvalid", "The refresh token is invalid, expired or revoked.", 401);

            if (!user.IsActive)
                throw new AuthException("Auth.AccountDeactivated", "This account is deactivated.", 403);

            return await CreateTokensAsync(user, stored);
        }

        public async Task LogoutAsync(Guid userId, string refreshToken)
        {
            var hash = Hash(refreshToken);
            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash && t.UserId == userId);

            if (stored is null || stored.RevokedAt is not null) return;

            stored.Revoke(DateTime.UtcNow);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _users.FindByIdAsync(userId.ToString())
                ?? throw new AuthException("Auth.UserNotFound", "User not found.", 401);

            return ToProfile(user, await _users.GetRolesAsync(user));
        }

        private async Task<UserProfileDto> CreateUserAsync(
            string email, string fullName, string password, string preferredLanguage, string role)
        {
            email = email.Trim();

            if (await _users.FindByEmailAsync(email) is not null)
                throw new AuthException("Auth.EmailAlreadyExists", "This email is already registered.", 409);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = fullName.Trim(),
                PreferredLanguage = preferredLanguage,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _users.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new AuthException("Auth.RegistrationFailed", string.Join(" ", result.Errors.Select(e => e.Description)), 400);

            await _users.AddToRoleAsync(user, role);
            return ToProfile(user, new List<string> { role });
        }

      
        private async Task<AuthResponse> CreateTokensAsync(ApplicationUser user, RefreshToken? oldToken)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        };
            foreach (var role in await _users.GetRolesAsync(user))
                claims.Add(new Claim("role", role));

            var expires = now.AddMinutes(_jwt.ExpiryMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var jwt = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims, now, expires,
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var plainRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var newToken = RefreshToken.Create(user.Id, Hash(plainRefreshToken), now, TimeSpan.FromDays(_jwt.RefreshTokenExpiryDays));

            await _refreshTokens.AddAsync(newToken);
            oldToken?.Revoke(now, newToken.Id);
            await _unitOfWork.CompleteAsync();

            return new AuthResponse(accessToken, plainRefreshToken, expires);
        }

        private static string Hash(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private static UserProfileDto ToProfile(ApplicationUser user, IList<string> roles) =>
            new(user.Id, user.Email!, user.FullName, user.PreferredLanguage, user.IsActive, roles, user.CreatedAt);
    }

}
