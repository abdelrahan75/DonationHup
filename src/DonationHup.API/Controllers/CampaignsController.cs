using DonationHup.Application.Features.Campaigns.Commend.CreateCampaigns;
using DonationHup.Application.Features.Campaigns.Commend.DeleteCampaign;
using DonationHup.Application.Features.Campaigns.Commend.UpdateCampaign;
using DonationHup.Application.Features.Campaigns.query.GetCampaignAdminById;
using DonationHup.Application.Features.Campaigns.query.GetCampaignById;
using DonationHup.Application.Features.Campaigns.query.GetCampaigns;
using DonationHup.Application.Features.Campaigns.query.GetSuccessStories;
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

        private string? Language => Request.Headers.AcceptLanguage.ToString();


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
             Handle(await Sender.Send(new GetCampaignsQuery(Language), ct));

        [AllowAnonymous]
        [HttpGet("success-stories")]
        public async Task<IActionResult> GetSuccessStories(CancellationToken ct) =>
            Handle(await Sender.Send(new GetSuccessStoriesQuery(Language), ct));

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
            Handle(await Sender.Send(new GetCampaignByIdQuery(id, Language), ct));

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id:guid}/admin")]
        public async Task<IActionResult> GetAdminById(Guid id, CancellationToken ct) =>
            Handle(await Sender.Send(new GetCampaignAdminByIdQuery(id), ct));

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCampaignCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command with { UserId = CurrentUserId }, ct),
                campaign => CreatedAtAction(nameof(GetById), new { id = campaign.Id }, campaign));

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCampaignCommand command, CancellationToken ct) =>
            Handle(await Sender.Send(command with { Id = id }, ct));

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
            Handle(await Sender.Send(new DeleteCampaignCommand(id), ct));
    }
}
