using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.Register
{
    public record RegisterCommand(
    string Email, string FullName, string Password, string? PreferredLanguage) : IRequest<Result<UserProfileDto>>;
}
