using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Features.Categories;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Campaigns.Commend.UpdateCampaign
{
    public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, Result<CampaignAdminDto>>
    {
        private readonly ICampaignRepository _campaigns;
        private readonly IGenericRepository<Category> _categories;
        private readonly IUnitofWork _unitOfWork;
        private readonly TimeProvider _time;

        public UpdateCampaignCommandHandler(
            ICampaignRepository campaigns,
            IGenericRepository<Category> categories,
            IUnitofWork unitOfWork,
            TimeProvider time)
        {
            _campaigns = campaigns;
            _categories = categories;
            _unitOfWork = unitOfWork;
            _time = time;
        }

        public async Task<Result<CampaignAdminDto>> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaign = await _campaigns.GetByIdAsync(request.Id, cancellationToken);
            if (campaign is null)
                return Result.Failure<CampaignAdminDto>(CampaignErrors.NotFound);

            if (await _categories.GetByIdAsync(request.CategoryId, cancellationToken) is null)
                return Result.Failure<CampaignAdminDto>(CategoryErrors.NotFound);

            _campaigns.SetRowVersion(campaign, request.RowVersion);

            var updated = campaign.Update(
                request.TitleAr, request.TitleEn, request.DescriptionAr, request.DescriptionEn,
                request.GoalAmount, request.EndDate, request.CategoryId,
                _time.GetUtcNow().UtcDateTime);

            if (updated.IsFailure)
                return Result.Failure<CampaignAdminDto>(CampaignErrors.FromDomain(updated.Error!));

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
            catch (ConcurrencyException)
            {
                // Someone saved a newer RowVersion first.
                return Result.Failure<CampaignAdminDto>(CampaignErrors.ConcurrencyConflict);
            }

            return Result.Success(CampaignMapping.ToAdmin(campaign));
        }
    }
}