using DonationHub.Infrastructure.data;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Common.Interfaces;

using DonationHup.Application.Features.Auth;
using DonationHup.Application.Features.Auth.CreateStaff;
using DonationHup.Application.Features.Auth.Login;
using DonationHup.Application.Features.Auth.Logout;
using DonationHup.Application.Features.Auth.Refresh;
using DonationHup.Application.Features.Auth.Register;
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
        private readonly TimeProvider _time;

        public AuthService(
            UserManager<ApplicationUser> users,
            AppDbContext context,
            IGenericRepository<RefreshToken> refreshTokens,
            IUnitofWork unitOfWork,
            IOptions<JwtOptians> jwtOptions,
            TimeProvider time)
        {
            _users = users;
            _context = context;
            _refreshTokens = refreshTokens;
            _unitOfWork = unitOfWork;
            _jwt = jwtOptions.Value;
            _time = time;
        }

        public Task<Result<UserProfileDto>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken) =>
            CreateUserAsync(command.Email, command.FullName, command.Password, command.PreferredLanguage, Roles.Donor);

        public Task<Result<UserProfileDto>> CreateStaffAsync(CreateStaffCommand command, CancellationToken cancellationToken) =>
            CreateUserAsync(command.Email, command.FullName, command.TemporaryPassword, command.PreferredLanguage, command.Role);

        public async Task<Result<AuthResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _users.FindByEmailAsync(command.Email.Trim());

            // نفس الخطأ للإيميل الغلط والباسورد الغلط (IDN-12)
            if (user is null || !await _users.CheckPasswordAsync(user, command.Password))
                return Result.Failure<AuthResponse>(AuthErrors.InvalidCredentials);

            if (!user.IsActive)
                return Result.Failure<AuthResponse>(AuthErrors.AccountDeactivated);

            return Result.Success(await CreateTokensAsync(user, null, cancellationToken));
        }

        public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var hash = Hash(command.RefreshToken);
            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
            var user = stored is null ? null : await _users.FindByIdAsync(stored.UserId.ToString());

            if (stored is null || user is null || stored.RevokedAt is not null || stored.ExpiresAt <= Now)
                return Result.Failure<AuthResponse>(AuthErrors.RefreshTokenInvalid);

            if (!user.IsActive)
                return Result.Failure<AuthResponse>(AuthErrors.AccountDeactivated);

            return Result.Success(await CreateTokensAsync(user, stored, cancellationToken));
        }

        public async Task<Result> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken)
        {
            var hash = Hash(command.RefreshToken);
            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == hash && t.UserId == command.UserId, cancellationToken);

            if (stored is null || stored.RevokedAt is not null)
                return Result.Success();

            stored.Revoke(Now);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _users.FindByIdAsync(userId.ToString());
            if (user is null)
                return Result.Failure<UserProfileDto>(AuthErrors.UserNotFound);

            return Result.Success(ToProfile(user, await _users.GetRolesAsync(user)));
        }

        private DateTime Now => _time.GetUtcNow().UtcDateTime;

        private async Task<Result<UserProfileDto>> CreateUserAsync(
            string email, string fullName, string password, string? preferredLanguage, string role)
        {
            email = email.Trim();

            if (await _users.FindByEmailAsync(email) is not null)
                return Result.Failure<UserProfileDto>(AuthErrors.EmailAlreadyExists);

            ApplicationUser user;
            try
            {
                user = ApplicationUser.Create(email, fullName, preferredLanguage, Now);
            }
            catch (DomainException ex)
            {
                return Result.Failure<UserProfileDto>(AuthErrors.RegistrationFailed(ex.Message));
            }

            var result = await _users.CreateAsync(user, password);
            if (!result.Succeeded)
                return Result.Failure<UserProfileDto>(
                    AuthErrors.RegistrationFailed(string.Join(" ", result.Errors.Select(e => e.Description))));

            await _users.AddToRoleAsync(user, role);
            return Result.Success(ToProfile(user, new List<string> { role }));
        }

        private async Task<AuthResponse> CreateTokensAsync(ApplicationUser user, RefreshToken? oldToken, CancellationToken cancellationToken)
        {
            var now = Now;

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

            await _refreshTokens.AddAsync(newToken, cancellationToken);
            oldToken?.Revoke(now, newToken.Id);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return new AuthResponse(accessToken, plainRefreshToken, expires);
        }

        private static string Hash(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        private static UserProfileDto ToProfile(ApplicationUser user, IList<string> roles) =>
            new(user.Id, user.Email!, user.FullName, user.PreferredLanguage, user.IsActive, roles, user.CreatedAt);
    }
}

