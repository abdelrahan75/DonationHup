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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(t => t.Id);

            // Only the hash is stored (a SHA-256 hash in hex/base64 is well under 128 characters)
            builder.Property(t => t.TokenHash).IsRequired().HasMaxLength(128);
            builder.HasIndex(t => t.TokenHash).IsUnique();

            // User -> RefreshTokens (one-to-many)
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
