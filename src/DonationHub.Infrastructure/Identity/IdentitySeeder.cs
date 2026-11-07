using DonationHub.Infrastructure.data;
using DonationHup.Application.Optians;
using DonationHup.Domain.Common;
using DonationHup.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, bool isDevelopment)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<AppDbContext>();
            var config = services.GetRequiredService<IConfiguration>();
            var admin = services.GetRequiredService<IOptions<SeedAdminOptions>>().Value;

            foreach (var role in new[] { Roles.Admin, Roles.CampaignManager, Roles.Donor })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = role });
            }

            await EnsureUserAsync(userManager, admin.Email, admin.Password, admin.FullName, Roles.Admin);

            if (!isDevelopment) return;

            await EnsureDevUserAsync(userManager, config, "Manager", Roles.CampaignManager);
            await EnsureDevUserAsync(userManager, config, "Donor", Roles.Donor);

            // 4) خمس تصنيفات
            if (!await context.Categories.AnyAsync())
            {
                var now = DateTime.UtcNow;
                context.Categories.AddRange(
                    Category.Create("الصحة", "Health", "حملات الرعاية الصحية والعلاج", "Healthcare and treatment campaigns", now),
                    Category.Create("التعليم", "Education", "حملات دعم التعليم", "Education support campaigns", now),
                    Category.Create("الإغاثة", "Relief", "حملات الإغاثة العاجلة", "Emergency relief campaigns", now),
                    Category.Create("الأيتام", "Orphans", "حملات رعاية الأيتام", "Orphan care campaigns", now),
                    Category.Create("البيئة", "Environment", "حملات حماية البيئة", "Environmental campaigns", now));
                await context.SaveChangesAsync();
            }
        }

        private static async Task EnsureDevUserAsync(
            UserManager<ApplicationUser> userManager, IConfiguration config, string name, string role)
        {
            var email = config[$"DevSeed:{name}Email"];
            var password = config[$"DevSeed:{name}Password"];

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
                await EnsureUserAsync(userManager, email, password, $"Test {name}", role);
        }

        private static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userManager, string email, string password, string fullName, string role)
        {
            if (await userManager.FindByEmailAsync(email) is not null) return;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Seeding the {role} user failed: {string.Join(", ", result.Errors.Select(e => e.Code))}");
            }

            await userManager.AddToRoleAsync(user, role);
        }
    }
}
