using DonationHup.Application.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Interfaces.Repositories
{
    public interface IAuthService
    {
        Task<UserProfileDto> RegisterAsync(RegisterRequest request);
        Task<UserProfileDto> CreateStaffAsync(CreateStaffRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshAsync(string refreshToken);
        Task LogoutAsync(Guid userId, string refreshToken);
        Task<UserProfileDto> GetProfileAsync(Guid userId);
    }
}
