
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.data;
using DonationHub.Infrastructure.Identity;
using DonationHup.API.ErrorHandling;
using DonationHup.API.Exceptions;
using DonationHup.API.OpenApi;
using DonationHup.Application;
using DonationHup.Application.Common.Results;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

namespace DonationHup
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .ConfigureApiBehaviorOptions(o =>
                    o.InvalidModelStateResponseFactory = ctx =>
                    {
                        var errors = ctx.ModelState
                            .Where(e => e.Value!.Errors.Count > 0)
                            .ToDictionary(
                                e => e.Key,
                                e => e.Value!.Errors.Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? "The value is invalid." : x.ErrorMessage).ToArray());
                        return ProblemFactory.Create(ctx.HttpContext, new ValidationError(errors));
                    });
            builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
                app.MapScalarApiReference();

            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}
