using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _auth;

        public LoginCommandHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken) =>
            _auth.LoginAsync(request, cancellationToken);
    }
}
