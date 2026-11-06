using DonationHub.Infrastructure.data;
using DonationHup.Domain.Common;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHup.Application.Common.Interfaces.Repositories;
namespace DonationHub.Infrastructure.Repositories
{
    public class GenaricRepository<T> : IGenericRepository<T> where T : Entity
    {
        private readonly DbSet<T> _set;

        public GenaricRepository(AppDbContext context)
        {
            _set = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
           return await _set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _set.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity) { 
            _set.Update(entity); 
        }

        public void Remove(T entity) { 
            _set.Remove(entity); 
        }
    }
}
