
using DonationHub.Infrastructure.data;
using DonationHub.Infrastructure.Repositories;
using DonationHup.Application.Common.Interfaces.Repositories;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DN")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenaricRepository<>));
            services.AddScoped<IUnitofWork, UnitOfWork>();

            return services;
        }

    }
}
