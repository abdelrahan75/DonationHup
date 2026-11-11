using DonationHup.Application.Features.Categories.ImplementCategory;
using DonationHup.Application.Features.Categories.InterfacesCategory;
using DonationHup.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHup.API.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categories;

        public CategoriesController(ICategoryService categories)
        {
            _categories = categories;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _categories.GetAllAsync());

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _categories.GetByIdAsync(id));

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var category = await _categories.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CategoryRequest request) =>
            Ok(await _categories.UpdateAsync(id, request));

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categories.DeleteAsync(id);
            return NoContent();
        }
    }

}
