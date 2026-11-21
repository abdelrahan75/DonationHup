using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetSuccessStories
{
    public record GetSuccessStoriesQuery(string? Language = null) : IRequest<Result<List<CampaignDto>>>
    {
    }
}
