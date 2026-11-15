using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.CreateCategory
{
    public record CreateCategoryCommend (
        Guid Id, string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn):IRequest<Result<CategoryDto>>;

}
