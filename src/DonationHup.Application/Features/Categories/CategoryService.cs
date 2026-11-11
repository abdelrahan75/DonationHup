
using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitofWork _unitOfWork;
    public CategoryService(ICategoryRepository categories, IUnitofWork unitOfWork)
    {

        _categories = categories;
        _unitOfWork = unitOfWork;
    }

    public Task<List<CategoryDto>> GetAllAsync() =>
        _categories.GetAllDtosAsync();

    public async Task<CategoryDto> GetByIdAsync(Guid id)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw NotFound();
        return ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CategoryRequest request)
    {
        await EnsureNamesAreUniqueAsync(request, Guid.Empty);

        var category = Category.Create(
            request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn, DateTime.UtcNow);

        await _categories.AddAsync(category);
        await _unitOfWork.CompleteAsync();

        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, CategoryRequest request)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw NotFound();

        await EnsureNamesAreUniqueAsync(request, id);

        category.Update(request.NameAr, request.NameEn, request.DescriptionAr, request.DescriptionEn, DateTime.UtcNow);
        _categories.Update(category);
        await _unitOfWork.CompleteAsync();

        return ToDto(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw NotFound();

        var hasCampaigns = await _categories.HasCampaignsAsync(id);

        try
        {
            category.EnsureCanBeDeleted(hasCampaigns); 
        }
        catch (DomainException ex)
        {
            throw new AppException(ex.Code, ex.Message, 409); 
        }

        _categories.Remove(category);
        await _unitOfWork.CompleteAsync();
    }

   
    private async Task EnsureNamesAreUniqueAsync(CategoryRequest request, Guid excludeId)
    {
        if (await _categories.ArNameExistsAsync(request.NameAr.Trim(), excludeId))
            throw new AppException("Category.NameArAlreadyExists", "A category with this Arabic name already exists.", 409);

        if (await _categories.EnNameExistsAsync(request.NameEn.Trim(), excludeId))
            throw new AppException("Category.NameEnAlreadyExists", "A category with this English name already exists.", 409);
    }

    private static AppException NotFound() =>
        new("Category.NotFound", "Category not found.", 404);

    private static CategoryDto ToDto(Category c) =>
        new(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription);
}


