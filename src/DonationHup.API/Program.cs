
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.data;
using DonationHub.Infrastructure.Identity;
using DonationHup.API.ErrorHandling;
using DonationHup.API.Exceptions;
using DonationHup.Application;
using Microsoft.EntityFrameworkCore;

namespace DonationHup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<AuthExceptionHandler>();
            builder.Services.AddJwtAuthentication();





            var app = builder.Build();
            app.UseExceptionHandler();

            using (var scope = app.Services.CreateScope())
            {
                await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Environment.IsDevelopment());
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}
