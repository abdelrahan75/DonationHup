using DonationHup.Application;
using DonationHup.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ApiController
    {
        public AuthController(ISender sender) : base(sender) { }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command, ct), user => StatusCode(StatusCodes.Status201Created, user));

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command, ct));

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command, ct));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command with { UserId = CurrentUserId }, ct));

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me(CancellationToken ct) =>
            Handle(await Sender.Send(new GetCurrentUserQuery(CurrentUserId), ct));
    }

}
