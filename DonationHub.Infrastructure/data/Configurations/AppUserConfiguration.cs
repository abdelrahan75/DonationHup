using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.data.Configurations
{
     public class AppUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public  void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            
            builder.Property((au) => au.FullName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(au => au.Email)
                .IsRequired()
                .HasMaxLength(100);
        }
        }
}
