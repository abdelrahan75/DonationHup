using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Optians
{
    public class JwtOptians
    {
        public const string SectionName = "Jwt";

        [Required, MinLength(32)]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(1, 1440)]
        public int ExpiryMinutes { get; set; } = 15;

        [Range(1, 90)]
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
