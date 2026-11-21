using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.DeleteCampaign
{
    public record DeleteCampaignCommand(Guid Id) : IRequest<Result>
    {
    }
}
