using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetSuccessStories
{
    public class GetSuccessStoriesQueryHandler : IRequestHandler<GetSuccessStoriesQuery, Result<List<CampaignDto>>>
    {
        private readonly IAppDbContext _db;

        public GetSuccessStoriesQueryHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<CampaignDto>>> Handle(GetSuccessStoriesQuery request, CancellationToken cancellationToken)
        {
            var arabic = CampaignMapping.IsArabic(request.Language);

            var query = _db.Campaigns
                .Where(c => c.Status == CampaignStutes.Completed)
                .OrderByDescending(c => c.UpdateAt);

            var campaigns = await _db.ToListAsync(query, cancellationToken);

            return Result.Success(campaigns.Select(c => CampaignMapping.ToDto(c, arabic)).ToList());
        }
    }
}
