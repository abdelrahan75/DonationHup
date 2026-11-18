using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.DeleteCategory
{
    public class DeleteCategoryCommendHandler : IRequestHandler<DeleteCategoryCommend, Result>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitofWork _unitOfWork;

        public DeleteCategoryCommendHandler(ICategoryRepository categories, IUnitofWork unitOfWork)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteCategoryCommend request, CancellationToken cancellationToken)
        {
            var Category = await _categories.GetByIdAsync(request.id, cancellationToken);
            if (Category is null)
                return Result.Failure(CategoryErrors.NotFound);

            var hasCampaigns = await _categories.HasCampaignsAsync(request.id, cancellationToken);

            try
            {
                Category.EnsureCanBeDeleted(hasCampaigns);
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error(ex.Code, ex.Message, ErrorType.Conflict));
            }
            _categories.Remove(Category);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}
