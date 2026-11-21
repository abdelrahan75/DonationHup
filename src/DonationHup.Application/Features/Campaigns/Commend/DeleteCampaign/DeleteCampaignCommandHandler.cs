using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.DeleteCampaign
{
    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, Result>
    {
        private readonly ICampaignRepository _campaigns;
        private readonly IUnitofWork _unitOfWork;
        private readonly TimeProvider _time;

        public DeleteCampaignCommandHandler(ICampaignRepository campaigns, IUnitofWork unitOfWork, TimeProvider time)
        {
            _campaigns = campaigns;
            _unitOfWork = unitOfWork;
            _time = time;
        }

        public async Task<Result> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaign = await _campaigns.GetByIdAsync(request.Id, cancellationToken);
            if (campaign is null)
                return Result.Failure(CampaignErrors.NotFound);

            campaign.SoftDelete(_time.GetUtcNow().UtcDateTime);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}
