using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.CreateCategory
{
    public class CreateCategoryCommendHandler : IRequestHandler<CreateCategoryCommend, Result<CategoryDto>>
    {

        private readonly ICategoryRepository _categoryRepository;

        private readonly IUnitofWork _unitOfWork;

        private readonly TimeProvider _time;

        public CreateCategoryCommendHandler(ICategoryRepository categoryRepository, IUnitofWork unitOfWork, TimeProvider time)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _time = time;
        }
        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommend request, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.ArNameExistsAsync(request.NameAr.Trim(), Guid.Empty, cancellationToken))
                return Result.Failure<CategoryDto>(CategoryErrors.NameArAlreadyExists);

            if (await _categoryRepository.EnNameExistsAsync(request.NameEn.Trim(), Guid.Empty, cancellationToken))
                return Result.Failure<CategoryDto>(CategoryErrors.NameEnAlreadyExists);

            var category = Category.Create(
                request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn, _time.GetUtcNow().UtcDateTime);

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result.Success(new CategoryDto(category.Id, category.ArName, category.EnName, category.ArDescription, category.EnDescription));
        }
    }
}
