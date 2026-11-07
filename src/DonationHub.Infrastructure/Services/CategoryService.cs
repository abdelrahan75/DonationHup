using DonationHub.Infrastructure.data;
using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces.Repositories;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using DonationHup.Application.Features.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;                      
        private readonly IGenericRepository<Category> _categories;   
        private readonly IUnitofWork _unitOfWork;

        public CategoryService(AppDbContext context, IGenericRepository<Category> categories, IUnitofWork unitOfWork)
        {
            _context = context;
            _categories = categories;
            _unitOfWork = unitOfWork;
        }

        public Task<List<CategoryDto>> GetAllAsync() =>
            _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.EnName)
                .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription))
                .ToListAsync();

        public async Task<CategoryDto> GetByIdAsync(Guid id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription))
                .FirstOrDefaultAsync();

            return category ?? throw NotFound();
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

            var hasCampaigns = await _context.Campaigns.IgnoreQueryFilters().AnyAsync(c => c.CategoryId == id);

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
            var nameAr = request.NameAr.Trim();
            var nameEn = request.NameEn.Trim();

            if (await _context.Categories.AnyAsync(c => c.Id != excludeId && c.ArName == nameAr))
                throw new AppException("Category.NameArAlreadyExists", "A category with this Arabic name already exists.", 409);

            if (await _context.Categories.AnyAsync(c => c.Id != excludeId && c.EnName == nameEn))
                throw new AppException("Category.NameEnAlreadyExists", "A category with this English name already exists.", 409);
        }

        private static AppException NotFound() =>
            new("Category.NotFound", "Category not found.", 404);

        private static CategoryDto ToDto(Category c) =>
            new(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription);
    }

}
