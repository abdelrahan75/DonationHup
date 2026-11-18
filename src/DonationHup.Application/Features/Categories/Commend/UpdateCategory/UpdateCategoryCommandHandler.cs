using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommend, Result<CategoryDto>>
    {
        private readonly ICategoryRepository _categories;
        private readonly IUnitofWork _unitOfWork;
        private readonly TimeProvider _time;

        public UpdateCategoryCommandHandler(ICategoryRepository categories, IUnitofWork unitOfWork, TimeProvider time)
        {
            _categories = categories;
            _unitOfWork = unitOfWork;
            _time = time;
        }

        public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommend request, CancellationToken cancellationToken)
        {
            var category = await _categories.GetByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return Result.Failure<CategoryDto>(CategoryErrors.NotFound);

            if (await _categories.ArNameExistsAsync(request.NameAr.Trim(), request.Id, cancellationToken))
                return Result.Failure<CategoryDto>(CategoryErrors.NameArAlreadyExists);

            if (await _categories.EnNameExistsAsync(request.NameEn.Trim(), request.Id, cancellationToken))
                return Result.Failure<CategoryDto>(CategoryErrors.NameEnAlreadyExists);

            category.Update(
                request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn, _time.GetUtcNow().UtcDateTime);

            _categories.Update(category);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success(new CategoryDto(category.Id, category.ArName, category.EnName, category.ArDescription, category.EnDescription));

        }
    }
}
