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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ArName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.EnName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ArDescription).HasMaxLength(1000);
            builder.Property(c => c.EnDescription).HasMaxLength(1000);

            builder.HasIndex(c => c.ArName).IsUnique();
            builder.HasIndex(c => c.EnName).IsUnique();
            
        }
    }
}

