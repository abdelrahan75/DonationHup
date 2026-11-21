using DonationHup.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.ErrorHandling
{
    public static class ProblemFactory
    {
        public static int StatusOf(ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,
            ErrorType.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
            ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status500InternalServerError
        };

        public static ObjectResult Create(HttpContext http, Error error)
        {
            var status = StatusOf(error.Type);

            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{status}",
                Title = error.Message,
                Status = status
            };
            problem.Extensions["code"] = error.Code;
            problem.Extensions["traceId"] = http.TraceIdentifier;

            if (error is ValidationError validation)
                problem.Extensions["errors"] = validation.Errors;

            return new ObjectResult(problem)
            {
                StatusCode = status,
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}
