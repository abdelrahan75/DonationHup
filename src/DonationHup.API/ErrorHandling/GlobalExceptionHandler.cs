using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Features.Campaigns;
using Microsoft.AspNetCore.Diagnostics;

namespace DonationHup.API.ErrorHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private static readonly Error Unexpected =
            new("Common.UnexpectedError", "An unexpected error occurred.", ErrorType.Failure);

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Full details (message + stack trace) go to the log only.
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            var error = exception is ConcurrencyException ? CampaignErrors.ConcurrencyConflict : Unexpected;

            var result = ProblemFactory.Create(httpContext, error);
            httpContext.Response.StatusCode = result.StatusCode!.Value;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(result.Value, options: null, contentType: "application/problem+json", cancellationToken);

            return true;
        }
    }
}
