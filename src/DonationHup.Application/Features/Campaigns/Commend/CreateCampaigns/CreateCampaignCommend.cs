using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns
{
    public record CreateCampaignCommand(
    string TitleAr, string TitleEn,
    string DescriptionAr, string DescriptionEn,
    decimal GoalAmount, DateTime EndDate, Guid CategoryId) : IRequest<Result<CampaignDto>>
    {
        public Guid UserId { get; init; }

    }
}
