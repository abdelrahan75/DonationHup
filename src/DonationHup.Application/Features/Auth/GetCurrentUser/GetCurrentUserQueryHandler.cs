using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
    {
        private readonly IAuthService _auth;

        public GetCurrentUserQueryHandler(IAuthService auth)
        {
            _auth = auth;
        }

        public Task<Result<UserProfileDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken) =>
            _auth.GetProfileAsync(request.UserId, cancellationToken);
    }
}
