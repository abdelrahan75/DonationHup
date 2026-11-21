
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.data;
using DonationHub.Infrastructure.data.Interceptor;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Repositories;
using DonationHup.Application.Common;
using DonationHup.Application.Common.Interfaces;
using DonationHup.Application.Features.Auth;
using DonationHup.Application.Features.Campaigns;
using DonationHup.Application.Features.Categories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Infrastructure
{
    public static class DependancyInfrastraction
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
                 options.UseSqlServer(configuration.GetConnectionString("DN"))
                        .AddInterceptors(new SoftDeleteInterceptor(sp.GetRequiredService<TimeProvider>())));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenaricRepository<>));
            services.AddScoped<IUnitofWork, UnitOfWork>();
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>()); 
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICampaignRepository, CampaignRepository>();

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IAuthService, AuthService>();


            return services;
        }

    }
}
