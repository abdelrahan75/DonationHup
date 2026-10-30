using DonationHub.Infrastructure.Identity;
using DonationHup.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.data.Configurations
{
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ArName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.EnName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.ArDescription).IsRequired().HasMaxLength(4000);
            builder.Property(c => c.EnDescription).IsRequired().HasMaxLength(4000);

            builder.Property(c => c.GoalAmount).HasPrecision(18, 2);
            builder.Property(c => c.RaisedAmount).HasPrecision(18, 2);

            builder.Property(c => c.BannerFileName).HasMaxLength(260);
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            builder.Property(c => c.RowVersion).IsRowVersion();

            builder.HasOne(c => c.Category)
                 .WithMany(c => c.Campaigns)
                 .HasForeignKey(c => c.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Donations)
                 .WithOne(d => d.Campaign)
                 .HasForeignKey(d => d.CampaignId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
