using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaignById
{
    public record GetCampaignByIdQuery(Guid id, string? Language = null) : IRequest<Result<CampaignDetailsDto>>

    {
    }
}
