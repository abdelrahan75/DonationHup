using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaigns
{
    public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, Result<List<CampaignDto>>>
    {
        private readonly IAppDbContext _db;

        public GetCampaignsQueryHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<CampaignDto>>> Handle(GetCampaignsQuery request, CancellationToken cancellationToken)
        {
            var arabic = CampaignMapping.IsArabic(request.Language);

            var query = _db.Campaigns
                .Where(c => c.Status == CampaignStutes.Active)
                .OrderBy(c => c.EndTime);

            var campaigns = await _db.ToListAsync(query, cancellationToken);

            return Result.Success(campaigns.Select(c => CampaignMapping.ToDto(c, arabic)).ToList());
        }
    }
}
