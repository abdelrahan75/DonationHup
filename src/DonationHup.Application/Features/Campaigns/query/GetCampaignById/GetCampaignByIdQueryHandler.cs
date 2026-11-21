using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaignById
{
    public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<CampaignDetailsDto>>
    {
        private readonly IAppDbContext _db;
        private readonly TimeProvider _time;

        public GetCampaignByIdQueryHandler(IAppDbContext db, TimeProvider time)
        {
            _db = db;
            _time = time;
        }

        public async Task<Result<CampaignDetailsDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Campaigns.Where(c => c.Id == request.id);
            var campaign = await _db.FirstOrDefaultAsync(query, cancellationToken);

            if (campaign is null)
                return Result.Failure<CampaignDetailsDto>(CampaignErrors.NotFound);

            return Result.Success(CampaignMapping.ToDetails(
                campaign, CampaignMapping.IsArabic(request.Language), _time.GetUtcNow().UtcDateTime));
        }
    }
}
