using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Refresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly IAuthService _auth;

        public RefreshTokenCommandHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
            _auth.RefreshAsync(request, cancellationToken);
    }

}
