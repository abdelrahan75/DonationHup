using DonationHup.Application;
using DonationHup.Application.Features.Auth;
using DonationHup.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DonationHup.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/staff")]
    [Authorize(Roles = Roles.Admin)]
    public class StaffController : ControllerBase
    {
        private readonly IAuthService _auth;

        public StaffController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStaffRequest request) =>
            StatusCode(StatusCodes.Status201Created, await _auth.CreateStaffAsync(request));
    }
}
