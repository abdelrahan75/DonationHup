using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.GetCurrentUser
{
    public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
    {
        public GetCurrentUserQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }

}
