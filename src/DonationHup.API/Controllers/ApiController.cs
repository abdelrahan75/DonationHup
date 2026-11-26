using DonationHup.API.ErrorHandling;
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

        private IActionResult ToProblem(Error error) => ProblemFactory.Create(HttpContext, error);
    }
}
