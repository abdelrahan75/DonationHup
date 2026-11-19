using DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns;
using DonationHup.Application.Features.Campaigns.query.GetCampaignById;
using DonationHup.Application.Features.Campaigns.query.GetCampaigns;
using DonationHup.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.Controllers
{
    [Route("api/v1/campaigns")]
    public class CampaignsController : ApiController
    {
        public CampaignsController(ISender sender) : base(sender) { }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
            Handle(await Sender.Send(new GetCampaignsQuery(), ct));

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
            Handle(await Sender.Send(new GetCampaignByIdQuery(id), ct));

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCampaignCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command with { UserId = CurrentUserId }, ct),
                campaign => CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign));
    }
}
