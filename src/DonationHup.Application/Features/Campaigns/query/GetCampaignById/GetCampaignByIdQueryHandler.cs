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
    public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<CampaignDto>>
    {
        private readonly IAppDbContext _db;

        public GetCampaignByIdQueryHandler(IAppDbContext db)
        {
            _db = db;
        }
        public async Task<Result<CampaignDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Campaigns
            .Where(c => c.Id == request.id)
            .Select(c => new CampaignDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription, c.GoalAmount, c.RaisedAmount, c.EndTime, c.CategoryId, c.Status));

            var Campaign = await _db.FirstOrDefaultAsync(query, cancellationToken);

            return Campaign is null
            ? Result.Failure<CampaignDto>(CampaignErrors.NotFound)
            : Result.Success(Campaign);
        }
    }
}
