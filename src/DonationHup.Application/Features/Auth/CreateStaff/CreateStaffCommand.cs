using DonationHup.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth.CreateStaff
{
    public record CreateStaffCommand(
    string Email, string FullName, string TemporaryPassword, string Role, string? PreferredLanguage)
    : IRequest<Result<UserProfileDto>>;
}
