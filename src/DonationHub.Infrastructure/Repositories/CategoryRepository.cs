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

        public Task<bool> ArNameExistsAsync(string nameAr, Guid excludeId, CancellationToken cancellationToken = default) =>
            _context.Categories.AnyAsync(c => c.Id != excludeId && c.ArName == nameAr, cancellationToken);

        public Task<bool> EnNameExistsAsync(string nameEn, Guid excludeId, CancellationToken cancellationToken = default) =>
            _context.Categories.AnyAsync(c => c.Id != excludeId && c.EnName == nameEn, cancellationToken);

        public Task<bool> HasCampaignsAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
            _context.Campaigns.IgnoreQueryFilters().AnyAsync(c => c.CategoryId == categoryId, cancellationToken);
    }

}
