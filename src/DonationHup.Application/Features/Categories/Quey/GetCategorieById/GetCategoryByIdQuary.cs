using DonationHup.Application.Common.Results;
using DonationHup.Domain.Entites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Quey.GetCategorieById
{
     public record  GetCategoryByIdQuary(Guid id) : IRequest<Result<CategoryDto>>
    {
        
    }
}
