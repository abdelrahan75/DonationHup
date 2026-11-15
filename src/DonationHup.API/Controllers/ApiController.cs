using DonationHup.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected ApiController(ISender sender)
        {
            Sender = sender;
        }

        protected ISender Sender { get; }

        protected Guid CurrentUserId => Guid.Parse(User.FindFirst("sub")!.Value);

        protected IActionResult Handle<T>(Result<T> result, Func<T, IActionResult>? onSuccess = null) =>
            result.IsSuccess
                ? onSuccess is null ? Ok(result.Value) : onSuccess(result.Value!)
                : ToProblem(result.Error!);

        protected IActionResult Handle(Result result) =>
            result.IsSuccess ? NoContent() : ToProblem(result.Error!);

        private IActionResult ToProblem(Error error)
        {
            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{status}",
                Title = error.Message,
                Status = status
            };
            problem.Extensions["code"] = error.Code;
            problem.Extensions["traceId"] = HttpContext.TraceIdentifier;

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
