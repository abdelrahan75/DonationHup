using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserProfileDto>>
    {
        private readonly IAuthService _auth;

        public RegisterCommandHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public Task<Result<UserProfileDto>> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
            _auth.RegisterAsync(request, cancellationToken);
    }
}
