using DonationHup.Application.Common.Behavior;
using DonationHup.Application.Features.Categories;
using DonationHup.Application.Optians;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHup.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ApplicationDependencyInjection).Assembly);

            services.TryAddSingleton(TimeProvider.System);

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
