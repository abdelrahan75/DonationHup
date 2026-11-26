using DonationHub.Infrastructure.data;
using DonationHup.Application.Common.Exceptions;
using DonationHup.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitofWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Keep EF Core out of the Application layer.
                throw new ConcurrencyException(ex);
            }
        }
    }
}