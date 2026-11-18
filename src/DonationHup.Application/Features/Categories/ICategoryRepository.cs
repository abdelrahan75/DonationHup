using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Common.Results;
using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<CategoryDto>> GetAllDtosAsync();                         
        Task<bool> ArNameExistsAsync(string nameAr, Guid excludeId, CancellationToken cancellationToken = default);        
        Task<bool> EnNameExistsAsync(string nameEn, Guid excludeId, CancellationToken cancellationToken = default);        
        Task<bool> HasCampaignsAsync(Guid categoryId, CancellationToken cancellationToken = default);        
        
    }
}
