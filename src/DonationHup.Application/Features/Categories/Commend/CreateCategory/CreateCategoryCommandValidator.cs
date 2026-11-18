using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommend>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DescriptionAr).MaximumLength(1000);
            RuleFor(x => x.DescriptionEn).MaximumLength(1000);
        }
    }
}
