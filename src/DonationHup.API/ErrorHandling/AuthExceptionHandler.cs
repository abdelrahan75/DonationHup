using DonationHup.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.ErrorHandling
{
    public class AuthExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetails;

        public AuthExceptionHandler(IProblemDetailsService problemDetails)
        {
            _problemDetails = problemDetails;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not AppException appException)
                return false;

            httpContext.Response.StatusCode = appException.StatusCode;

            var problem = new ProblemDetails
            {
                Status = appException.StatusCode,
                Title = appException.Code,
                Detail = appException.Message
            };
            problem.Extensions["code"] = appException.Code;
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem
            });
        }
    }
}
