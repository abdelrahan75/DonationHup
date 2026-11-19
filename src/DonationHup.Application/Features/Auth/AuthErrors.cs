using DonationHup.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Auth
{
    public static class AuthErrors
    {

    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailAlreadyExists", "This email is already registered.", ErrorType.Conflict);

    public static readonly Error RefreshTokenInvalid =
        new("Auth.RefreshTokenInvalid", "The refresh token is invalid, expired or revoked.", ErrorType.Unauthorized);

    public static readonly Error AccountDeactivated =
        new("Auth.AccountDeactivated", "This account is deactivated.", ErrorType.Forbidden);

    public static readonly Error UserNotFound =
        new("Auth.UserNotFound", "User not found.", ErrorType.Unauthorized);

    public static Error RegistrationFailed(string message) =>
        new("Auth.RegistrationFailed", message, ErrorType.Validation);
}
}
