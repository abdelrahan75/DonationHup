using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Results
{
    public enum ErrorType
    {
        Validation,
        Unauthorized,
        Forbidden,
        NotFound,
        Conflict,
        PayloadTooLarge,
        UnprocessableEntity,
        TooManyRequests,
        Failure
    }

    public record Error(string Code, string Message, ErrorType Type);

    public record ValidationError(IReadOnlyDictionary<string, string[]> Errors)
        : Error("Validation.Failed", "One or more validation errors occurred.", ErrorType.Validation);
}
