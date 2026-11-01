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
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.ToTable("PaymentAttempts");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.StripeCheckoutSessionId).IsRequired().HasMaxLength(255);
            builder.Property(p => p.StripePaymentIntentId).HasMaxLength(255);
            builder.Property(p => p.FailureCode).HasMaxLength(100);
            builder.Property(p => p.Amount).HasPrecision(18, 2);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        }
    }
}
