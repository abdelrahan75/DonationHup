using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using MediatR;

namespace DonationHup.Application.Features.Categories.Quey.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
    {
    private readonly IAppDbContext _db;

    public GetCategoriesQueryHandler(IAppDbContext db)
    {
        _db = db;
    }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
        var query = _db.Categories
            .OrderBy(c => c.EnName)
            .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription));

        var categories = await _db.ToListAsync(query, cancellationToken);

        return Result.Success(categories);
    }
}
}
