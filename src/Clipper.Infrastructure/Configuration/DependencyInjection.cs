using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Clipper.Application.Common.Interfaces;
using Clipper.Infrastructure.Services;
using Clipper.Infrastructure.Repositories;
using Clipper.Common.Settings;

namespace Clipper.Infrastructure.Configuration;

/// <summary>
/// Configuração de injeção de dependência para a camada Infrastructure
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada Infrastructure
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurações
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Repositórios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Serviços
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailValidationService, EmailValidationService>();

        return services;
    }
}
