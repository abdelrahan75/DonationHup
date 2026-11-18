using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns
{
    public class CreateCampaignCommandValidator : AbstractValidator<CreateCampaignCommand>
    {
        public CreateCampaignCommandValidator()
        {
            RuleFor(x => x.TitleAr).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TitleEn).NotEmpty().MaximumLength(200);
            RuleFor(x => x.DescriptionAr).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.DescriptionEn).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.GoalAmount).GreaterThan(0);
            RuleFor(x => x.CategoryId).NotEmpty();
        }
    }

}
