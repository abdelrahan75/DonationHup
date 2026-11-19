using DonationHup.Application;
using DonationHup.Application.Features.Auth;
using DonationHup.Application.Features.Auth.CreateStaff;
using DonationHup.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DonationHup.API.Controllers
{
    [Route("api/v1/admin/staff")]
    [Authorize(Roles = Roles.Admin)]
    public class StaffController : ApiController
    {
        public StaffController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStaffCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command, ct), user => StatusCode(StatusCodes.Status201Created, user));
    }
}
