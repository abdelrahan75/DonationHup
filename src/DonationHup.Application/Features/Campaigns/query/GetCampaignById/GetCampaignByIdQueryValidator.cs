using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaignById
{
    public class GetCampaignByIdQueryValidator : AbstractValidator<GetCampaignByIdQuery>
    {
        public GetCampaignByIdQueryValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }

}
