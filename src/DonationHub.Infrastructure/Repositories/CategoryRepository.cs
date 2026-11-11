using DonationHub.Infrastructure.data;
using DonationHup.Application.Features.Categories;
using DonationHup.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Repositories
{
    public class CategoryRepository : GenaricRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<CategoryDto>> GetAllDtosAsync() =>
            _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.EnName)
                .Select(c => new CategoryDto(c.Id, c.ArName, c.EnName, c.ArDescription, c.EnDescription))
                .ToListAsync();

        public Task<bool> ArNameExistsAsync(string nameAr, Guid excludeId) =>
            _context.Categories.AnyAsync(c => c.Id != excludeId && c.ArName == nameAr);

        public Task<bool> EnNameExistsAsync(string nameEn, Guid excludeId) =>
            _context.Categories.AnyAsync(c => c.Id != excludeId && c.EnName == nameEn);

        // IgnoreQueryFilters: الحملة المحذوفة soft لسه بتشاور على التصنيف (FK)، فلازم تتحسب
        public Task<bool> HasCampaignsAsync(Guid categoryId) =>
            _context.Campaigns.IgnoreQueryFilters().AnyAsync(c => c.CategoryId == categoryId);
    }

}
