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
            var arabic = CampaignMapping.IsArabic(request.Language);

            var query = _db.Campaigns
                .Where(c => c.Id == request.id)
                .Select(c => new CampaignDetailsDto(
                    c.Id,
                    arabic ? c.ArName : c.EnName,
                    arabic ? c.ArDescription : c.EnDescription,
                    c.GoalAmount, c.RaisedAmount, c.EndTime, c.CategoryId, c.Status,
                    0, 0));

            var campaign = await _db.FirstOrDefaultAsync(query, cancellationToken);
            if (campaign is null)
                return Result.Failure<CampaignDetailsDto>(CampaignErrors.NotFound);

            return Result.Success(campaign with
            {
                ProgressPercent = CampaignMapping.ProgressPercent(campaign.RaisedAmount, campaign.GoalAmount),
                DaysLeft = CampaignMapping.DaysLeft(campaign.EndDate, _time.GetUtcNow().UtcDateTime)
            });
        }
    }
}
