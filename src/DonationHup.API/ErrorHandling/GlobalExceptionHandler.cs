using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Results;
using DonationHup.Application.Features.Campaigns;
using DonationHup.Application.Features.Categories;
using DonationHup.Domain.Common;
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
            var error = exception switch
            {
                DomainException d when d.Code == CategoryErrors.HasCampaigns.Code => CategoryErrors.HasCampaigns,
                DomainException d => CampaignErrors.FromDomain(d),
                ConcurrencyException => CampaignErrors.ConcurrencyConflict,
                _ => Unexpected
            };

            // Expected business failures are a warning; anything else is an error with the full stack trace.
            if (error == Unexpected)
                _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
            else
                _logger.LogWarning("{Code}: {Message}. TraceId: {TraceId}", error.Code, exception.Message, httpContext.TraceIdentifier);

            var result = ProblemFactory.Create(httpContext, error);
            httpContext.Response.StatusCode = result.StatusCode!.Value;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(result.Value, options: null, contentType: "application/problem+json", cancellationToken);

            return true;
        }
    }
}
