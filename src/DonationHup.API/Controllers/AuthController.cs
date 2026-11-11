using DonationHup.Application;
using DonationHup.Application.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request) =>
            StatusCode(StatusCodes.Status201Created, await _auth.RegisterAsync(request));

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request) =>
            Ok(await _auth.LoginAsync(request));

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest request) =>
            Ok(await _auth.RefreshAsync(request.RefreshToken));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequest request)
        {
            await _auth.LogoutAsync(UserId, request.RefreshToken);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me() =>
            Ok(await _auth.GetProfileAsync(UserId));
        private Guid UserId => Guid.Parse(User.FindFirst("sub")!.Value);
    }

}
