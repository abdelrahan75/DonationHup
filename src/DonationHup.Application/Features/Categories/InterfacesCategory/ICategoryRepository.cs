using DonationHup.Application.Common.Interfaces.Repositories;
using DonationHup.Application.Features.Categories.ImplementCategory;
using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Features.Categories.InterfacesCategory
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<CategoryDto>> GetAllDtosAsync();                         
        Task<bool> ArNameExistsAsync(string nameAr, Guid excludeId);        
        Task<bool> EnNameExistsAsync(string nameEn, Guid excludeId);        
        Task<bool> HasCampaignsAsync(Guid categoryId);                     
    }
}
