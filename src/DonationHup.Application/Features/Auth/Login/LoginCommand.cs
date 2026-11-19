using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Login
{
    public record LoginCommand (string Email , string Password) : IRequest<Result<AuthResponse>>
    {
    }
}
