using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<Result>
    {
        public Guid UserId { get; init; }
    }

}
