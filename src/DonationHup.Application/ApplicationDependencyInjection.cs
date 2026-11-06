using DonationHup.Application.Optians;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHup.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtOptians>()
                .Bind(configuration.GetSection(JwtOptians.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<SeedAdminOptions>()
                .Bind(configuration.GetSection(SeedAdminOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<DonationOptions>()
                .Bind(configuration.GetSection(DonationOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();


            return services;
        }
    }
}
