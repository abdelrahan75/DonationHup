using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Quey.GetCategorieById
{
    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuary>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }
}
