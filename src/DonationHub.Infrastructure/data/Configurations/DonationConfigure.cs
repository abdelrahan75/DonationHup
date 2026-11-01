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
    public class DonationConfigure : IEntityTypeConfiguration<Donation>
    {
    public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("Donations");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Amount).HasPrecision(18, 2);
            builder.Property(d => d.Stutes).HasConversion<string>().HasMaxLength(20);
            builder.Property(d => d.FailureReason).HasMaxLength(500);
            builder.Property(d => d.ReceiptNumber).HasMaxLength(20);
            builder.Property(d => d.ReceiptFileName).HasMaxLength(260);

            builder.HasMany(d => d.PaymentAttempts)
                .WithOne()
                .HasForeignKey(p => p.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(d => d.DonorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
