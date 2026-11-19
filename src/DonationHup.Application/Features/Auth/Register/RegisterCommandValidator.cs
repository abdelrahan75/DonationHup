using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.");

            // اللغة اختيارية (الافتراضي en)، ولو اتبعتت لازم en أو ar
            RuleFor(x => x.PreferredLanguage)
                .Must(l => string.IsNullOrWhiteSpace(l) || l.Trim().ToLowerInvariant() is "en" or "ar")
                .WithMessage("PreferredLanguage must be 'en' or 'ar'.");
        }
    }
}
