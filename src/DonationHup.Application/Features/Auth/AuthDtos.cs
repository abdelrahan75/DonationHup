using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth
{
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc);

    public record UserProfileDto(
        Guid Id,
        string Email,
        string FullName,
        string PreferredLanguage,
        bool IsActive,
        IList<string> Roles,
        DateTime CreatedAt);

}
