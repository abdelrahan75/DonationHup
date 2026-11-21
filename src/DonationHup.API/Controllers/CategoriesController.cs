using DonationHup.Application.Features.Categories.Commend.CreateCategory;
using DonationHup.Application.Features.Categories.Commend.DeleteCategory;
using DonationHup.Application.Features.Categories.Commend.UpdateCategory;
using DonationHup.Application.Features.Categories.Quey.GetCategorieById;
using DonationHup.Application.Features.Categories.Quey.GetCategories;
using DonationHup.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DonationHup.API.Controllers
{
    [Route("api/v1/categories")]
    public class CategoriesController : ApiController 
    {
        public CategoriesController(ISender sender) : base(sender) { }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
            Handle(await Sender.Send(new GetCategoriesQuery(), ct));

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
             Handle(await Sender.Send(new GetCategoryByIdQuary(id), ct));


        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryCommend command, CancellationToken ct) =>
            Handle(await Sender.Send(command, ct),
                category => CreatedAtAction(nameof(GetById), new { id = category.Id }, category));

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateCategoryCommend command, CancellationToken ct) =>
           Handle(await Sender.Send(command with { Id = id }, ct));

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
            Handle(await Sender.Send(new DeleteCategoryCommend(id), ct));

    }

}
