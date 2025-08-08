/**
 * @file: SecurityExtensions.cs
 * @responsibility: Extensões para registrar serviços e middleware de segurança
 * @exports: SecurityExtensions
 * @imports: IServiceCollection, IApplicationBuilder, IXssProtectionService, XssProtectionService, SecurityHeadersMiddleware
 * @layer: API/Extensions
 */
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Services;
using Clipper.API.Middleware;

namespace Clipper.API.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddScoped<IXssProtectionService, XssProtectionService>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
        return services;
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
