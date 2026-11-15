using DonationHub.Infrastructure.Identity;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Domain.Entites;
using DonationHub.Infrastructure.Identity;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DonationHub.Infrastructure.data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser , IdentityRole<Guid> , Guid> , IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<ProcessedStripeEvent> ProcessedStripeEvents => Set<ProcessedStripeEvent>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        IQueryable<Category> IAppDbContext.Categories => Categories.AsNoTracking();
        IQueryable<Campaign> IAppDbContext.Campaigns => Campaigns.AsNoTracking();

        Task<List<T>> IAppDbContext.ToListAsync<T>(IQueryable<T> query, CancellationToken cancellationToken) where T : class =>
            query.ToListAsync(cancellationToken);

        Task<T?> IAppDbContext.FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken cancellationToken) where T : class =>
            query.FirstOrDefaultAsync(cancellationToken);


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        
    }
}
