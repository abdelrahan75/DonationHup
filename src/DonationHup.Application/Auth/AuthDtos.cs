using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Auth
{
    public class RegisterRequest
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [RegularExpression("^(en|ar)$", ErrorMessage = "PreferredLanguage must be 'en' or 'ar'.")]
        public string PreferredLanguage { get; set; } = "en";
    }

    public class LoginRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class CreateStaffRequest
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string TemporaryPassword { get; set; } = string.Empty;

        [Required, RegularExpression("^(Admin|CampaignManager)$", ErrorMessage = "Role must be 'Admin' or 'CampaignManager'.")]
        public string Role { get; set; } = string.Empty;

        [RegularExpression("^(en|ar)$", ErrorMessage = "PreferredLanguage must be 'en' or 'ar'.")]
        public string PreferredLanguage { get; set; } = "en";
    }

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
