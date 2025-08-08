/**
 * @file: CorsExtensions.cs
 * @responsibility: registro de políticas CORS no DI
 * @exports: AddCorsPolicy
 * @imports: CorsSettings (Application)
 * @layer: API/Extensions
 */
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Clipper.Application.Common.Settings;

namespace Clipper.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection("CorsSettings").Get<CorsSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy("ClipperCorsPolicy", policy =>
                {
                    policy.WithOrigins(corsSettings.AllowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .SetIsOriginAllowedToAllowWildcardSubdomains()
                          .WithExposedHeaders("Content-Disposition");
                });

                options.AddPolicy("ProductionCorsPolicy", policy =>
                {
                    policy.WithOrigins("https://clipper.example.com", "https://app.clipper.example.com")
                          .WithMethods("GET", "POST", "PUT", "DELETE")
                          .WithHeaders("Content-Type", "Authorization")
                          .AllowCredentials()
                          .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            return services;
        }
    }
}
