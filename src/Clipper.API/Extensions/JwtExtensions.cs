using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Clipper.Common.Settings;
using Clipper.API.Authorization.Policies;
using Clipper.API.Authorization.Requirements;
using Clipper.API.Authorization.Handlers;

namespace Clipper.API.Extensions;

/// <summary>
/// Extensions para configuração do JWT e autorização
/// </summary>
public static class JwtExtensions
{
    /// <summary>
    /// Adiciona autenticação JWT ao container de serviços
    /// </summary>
    /// <param name="services">Container de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Container de serviços configurado</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind das configurações do JWT
        var jwtSettings = new JwtSettings();
        configuration.GetSection("Jwt").Bind(jwtSettings);

        // Se não houver configuração, usar padrões para teste
        if (string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            jwtSettings.SecretKey = "ThisIsAVerySecureSecretKeyForTestingPurposes123456789012345678901234567890";
            jwtSettings.Issuer = "ClipperTest";
            jwtSettings.Audience = "ClipperTestUsers";
            jwtSettings.ExpirationInMinutes = 15;
            jwtSettings.RefreshTokenExpirationInDays = 7;
        }

        // Registrar as configurações no DI
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton(jwtSettings);

        // Configurar chave de segurança
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var signingKey = new SymmetricSecurityKey(key);

        // Configurar autenticação JWT
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // TODO: Mudar para true em produção
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero // Remove delay padrão de 5 minutos
            };

            // Configurações adicionais para desenvolvimento
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = System.Text.Json.JsonSerializer.Serialize(new { error = "Token inválido ou expirado" });
                        return context.Response.WriteAsync(result);
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Adiciona autorização customizada com policies e handlers
    /// </summary>
    /// <param name="services">Container de serviços</param>
    /// <returns>Container de serviços configurado</returns>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        // Registrar handlers customizados
        services.AddScoped<IAuthorizationHandler, OwnershipAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

        // Configurar autorização com policies customizadas
        services.AddAuthorization(options =>
        {
            // Configurar todas as policies via classe centralizada
            AuthorizationPolicies.ConfigurePolicies(options);
        });

        return services;
    }

    /// <summary>
    /// Adiciona middleware customizado de JWT
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder configurado</returns>
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<Clipper.API.Middleware.JwtMiddleware>();
    }
}
