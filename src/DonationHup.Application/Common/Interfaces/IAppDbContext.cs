using DonationHup.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        IQueryable<Category> Categories { get; }
        IQueryable<Campaign> Campaigns { get; }


        Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken cancellationToken) where T : class;
        Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken cancellationToken) where T : class;
    }
}
