/**
 * @file: CorsConfiguration.cs
 * @responsibility: seleção e aplicação dinâmica de política CORS
 * @exports: GetCorsPolicy, ConfigureCors
 * @imports: IWebHostEnvironment, IApplicationBuilder
 * @layer: API/Configuration
 */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Clipper.API.Configuration
{
    public static class CorsConfiguration
    {
        public static string GetCorsPolicy(IWebHostEnvironment environment)
        {
            return environment.IsDevelopment() ? "ClipperCorsPolicy" : "ProductionCorsPolicy";
        }

        public static void ConfigureCors(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            var policyName = GetCorsPolicy(environment);
            app.UseCors(policyName);
        }
    }
}
