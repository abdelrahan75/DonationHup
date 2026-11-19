using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.CreateStaff
{
    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<UserProfileDto>>
    {
        private readonly IAuthService _auth;

        public CreateStaffCommandHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public Task<Result<UserProfileDto>> Handle(CreateStaffCommand request, CancellationToken cancellationToken) =>
            _auth.CreateStaffAsync(request, cancellationToken);
    }
}
