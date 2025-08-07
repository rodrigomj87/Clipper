using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Clipper.Application.Common.Settings;

namespace Clipper.API.Extensions;

/// <summary>
/// Extensions para configuração do JWT
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
        configuration.GetSection("JwtSettings").Bind(jwtSettings);

        // Validar configurações
        if (!jwtSettings.IsValid())
        {
            throw new InvalidOperationException("Configurações JWT inválidas. Verifique SecretKey, Issuer e Audience.");
        }

        // Registrar as configurações no DI
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
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
    /// Adiciona autorização customizada
    /// </summary>
    /// <param name="services">Container de serviços</param>
    /// <returns>Container de serviços configurado</returns>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Policy básica: usuário autenticado
            options.AddPolicy("RequireUser", policy =>
                policy.RequireAuthenticatedUser());

            // Policy para admin (será implementada nas próximas tasks)
            options.AddPolicy("RequireAdmin", policy =>
                policy.RequireRole("Admin"));

            // Policy padrão
            options.DefaultPolicy = options.GetPolicy("RequireUser")!;
        });

        return services;
    }
}
