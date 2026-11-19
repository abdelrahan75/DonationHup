using DonationHup.Application;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Features.Auth.CreateStaff;
using DonationHup.Application.Features.Auth.Login;
using DonationHup.Application.Features.Auth.Logout;
using DonationHup.Application.Features.Auth.Refresh;
using DonationHup.Application.Features.Auth.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<Result<UserProfileDto>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken);
        Task<Result<UserProfileDto>> CreateStaffAsync(CreateStaffCommand command, CancellationToken cancellationToken);
        Task<Result<AuthResponse>> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
        Task<Result<AuthResponse>> RefreshAsync(RefreshTokenCommand command, CancellationToken cancellationToken);
        Task<Result> LogoutAsync(LogoutCommand command, CancellationToken cancellationToken);
        Task<Result<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
    }
}
