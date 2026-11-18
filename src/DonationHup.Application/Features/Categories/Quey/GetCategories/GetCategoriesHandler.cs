using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;

namespace DonationHup.Application.Features.Categories.Quey.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
    {
    private readonly IAppDbContext appDbContext;

    public GetCategoriesQueryHandler(IAppDbContext dbContext)
    {
        appDbContext = dbContext;
    }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = appDbContext.Categories
                .OrderBy(c => c.Id)
            .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription));

            var catogeries = await appDbContext.ToListAsync(query, cancellationToken=default);
            return Result.Success(catogeries);

   
      
        
        }
   
    }
}
