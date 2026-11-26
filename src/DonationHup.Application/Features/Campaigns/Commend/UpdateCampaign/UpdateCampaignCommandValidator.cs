using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.UpdateCampaign
{
    public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
    {
        public UpdateCampaignCommandValidator(TimeProvider time)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(200);
            RuleFor(x => x.DescriptionAr).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.DescriptionEn).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.GoalAmount).GreaterThan(0);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.EndDate).Must(d => d > time.GetUtcNow().UtcDateTime)
                .WithMessage("The end date must be in the future.");
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }
}
