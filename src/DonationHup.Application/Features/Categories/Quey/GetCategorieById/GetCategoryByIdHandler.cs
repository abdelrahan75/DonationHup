using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Quey.GetCategorieById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuary, Result<CategoryDto>>
    {
        private readonly IAppDbContext app;

        public GetCategoryByIdHandler(IAppDbContext _app)
        {
            app = _app;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuary request, CancellationToken cancellationToken)
        {
            var query = app.Categories.
                Where(c=> c.Id == request.id)
                .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription));
            var category = await app.FirstOrDefaultAsync(query, cancellationToken);

            return category is null
                        ? Result.Failure<CategoryDto>(CategoryErrors.NotFound)
                        : Result.Success(category);
        }
    }
}
