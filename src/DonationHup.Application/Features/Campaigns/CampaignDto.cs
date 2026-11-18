using DonationHup.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns
{
    public record CampaignDto(
    Guid Id,
    string TitleAr,
    string TitleEn,
    string DescriptionAr,
    string DescriptionEn,
    decimal GoalAmount,
    decimal RaisedAmount,
    DateTime EndDate,
    Guid CategoryId,
    CampaignStutes Status);
}
