using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaignAdminById
{
    public record GetCampaignAdminByIdQuery(Guid Id) : IRequest<Result<CampaignAdminDto>>
    {
    }
}
