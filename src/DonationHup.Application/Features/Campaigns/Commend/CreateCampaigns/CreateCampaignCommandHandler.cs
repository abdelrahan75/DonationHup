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

namespace DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns
{
    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<CampaignDto>>
    {
        private readonly IGenericRepository<Campaign> _campaigns;
        private readonly IGenericRepository<Category> _categories;
        private readonly IUnitofWork _unitOfWork;
        private readonly TimeProvider _time;

        public CreateCampaignCommandHandler(
            IGenericRepository<Campaign> campaigns,
            IGenericRepository<Category> categories,
            IUnitofWork unitOfWork,
            TimeProvider time)
        {
            _campaigns = campaigns;
            _categories = categories;
            _unitOfWork = unitOfWork;
            _time = time;
        }

        public async Task<Result<CampaignDto>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            if (await _categories.GetByIdAsync(request.CategoryId, cancellationToken) is null)
                return Result.Failure<CampaignDto>(CategoryErrors.NotFound);

            Campaign campaign;
            try
            {
                campaign = Campaign.Create(
                    request.TitleAr, request.TitleEn, request.DescriptionAr, request.DescriptionEn,
                    request.GoalAmount, request.EndDate, request.CategoryId, request.UserId,
                    _time.GetUtcNow().UtcDateTime);
            }
            catch (DomainException ex)
            {
                return Result.Failure<CampaignDto>(new Error(ex.Code, ex.Message, ErrorType.UnprocessableEntity));
            }

            await _campaigns.AddAsync(campaign, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success(new CampaignDto(
                campaign.Id, campaign.ArName, campaign.EnName, campaign.ArDescription, campaign.EnDescription,
                campaign.GoalAmount, campaign.RaisedAmount, campaign.EndTime, campaign.CategoryId, campaign.Status));
        }
    }
}
