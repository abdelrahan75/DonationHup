using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Quey.GetCategories
{
    public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
    {
    }
}
