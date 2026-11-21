using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.query.GetCampaignAdminById
{
    public class GetCampaignAdminByIdQueryHandler : IRequestHandler<GetCampaignAdminByIdQuery, Result<CampaignAdminDto>>
    {
        private readonly IAppDbContext _db;

        public GetCampaignAdminByIdQueryHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<CampaignAdminDto>> Handle(GetCampaignAdminByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Campaigns.Where(c => c.Id == request.Id);
            var campaign = await _db.FirstOrDefaultAsync(query, cancellationToken);

            return campaign is null
                ? Result.Failure<CampaignAdminDto>(CampaignErrors.NotFound)
                : Result.Success(CampaignMapping.ToAdmin(campaign));
        }
    }
}
