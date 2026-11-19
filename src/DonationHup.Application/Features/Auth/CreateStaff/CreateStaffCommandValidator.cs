using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.CreateStaff
{
    public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
    {
        public CreateStaffCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.TemporaryPassword).NotEmpty().MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.");

            RuleFor(x => x.Role)
                .Must(r => r is Domain.Common.Roles.Admin or Domain.Common.Roles.CampaignManager)
                .WithMessage("Role must be 'Admin' or 'CampaignManager'.");

            RuleFor(x => x.PreferredLanguage)
                .Must(l => string.IsNullOrWhiteSpace(l) || l.Trim().ToLowerInvariant() is "en" or "ar")
                .WithMessage("PreferredLanguage must be 'en' or 'ar'.");
        }
    }

}
