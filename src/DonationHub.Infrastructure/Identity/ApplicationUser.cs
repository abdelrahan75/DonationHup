using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        private static readonly string[] SupportedLanguages = { "en", "ar" };

        public string FullName { get;private set; } = string.Empty;
        public string? PreferredLanguage { get; private set; } = "en"; 
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        private ApplicationUser() { } 

        public static ApplicationUser Create(string email, string fullName, string? preferredLanguage, DateTime nowUtc)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || email.Trim().Length > 100)
                throw new DomainException("User.InvalidEmail", "Email is invalid or longer than 100 characters.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email.Trim(),
                Email = email.Trim(),
                CreatedAt = nowUtc
            };

            user.ChangeFullName(fullName);
            user.ChangePreferredLanguage(preferredLanguage);
            return user;
        }

        public void ChangeFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length > 100)
                throw new DomainException("User.InvalidFullName", "Full name is required and must not exceed 100 characters.");

            FullName = fullName.Trim();
        }

        public void ChangePreferredLanguage(string? preferredLanguage)
        {
            var language = string.IsNullOrWhiteSpace(preferredLanguage) ? "en" : preferredLanguage.Trim().ToLowerInvariant();

            if (!SupportedLanguages.Contains(language))
                throw new DomainException("User.InvalidLanguage", "Preferred language must be 'en' or 'ar'.");

            PreferredLanguage = language;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;
    }
}
