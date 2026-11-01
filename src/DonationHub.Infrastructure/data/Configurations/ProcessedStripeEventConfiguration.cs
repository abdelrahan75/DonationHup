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
    public class ProcessedStripeEventConfiguration : IEntityTypeConfiguration<ProcessedStripeEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedStripeEvent> builder)
        {
            builder.ToTable("ProcessedStripeEvents");
            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId).HasMaxLength(255);
            builder.Property(e => e.Type).IsRequired().HasMaxLength(100);
        }
    }
}