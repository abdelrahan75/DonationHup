using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.Commend.DeleteCategory
{
    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommend>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }
}
