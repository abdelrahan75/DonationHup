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
    public class IdempotencyRecordConfigure : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");

            builder.HasKey(r => new { r.Key, r.UserId });

            builder.Property(r => r.Key).HasMaxLength(200);
            builder.Property(r => r.RequestHash).IsRequired().HasMaxLength(64);
            builder.Property(r => r.ResponseBody).IsRequired();
        }
    }
}
