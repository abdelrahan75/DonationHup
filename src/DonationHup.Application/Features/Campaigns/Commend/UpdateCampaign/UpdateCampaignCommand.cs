using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.UpdateCampaign
{
    public record UpdateCampaignCommand(
        string TitleAr, string TitleEn,
        string DescriptionAr, string DescriptionEn,
        decimal GoalAmount, DateTime EndDate, Guid CategoryId,
        byte[] RowVersion) : IRequest<Result<CampaignAdminDto>>
    {
        public Guid Id { get; init; }
    }
}
