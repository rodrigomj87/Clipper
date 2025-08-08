using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration;
using Clipper.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace Clipper.Tests.Infrastructure;

/// <summary>
/// Classe base para testes de integração do Clipper
/// </summary>
/// <remarks>
/// Responsabilidade: configurar ambiente de teste isolado com banco em memória
/// </remarks>
public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly ClipperDbContext DbContext;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove todos os DbContext e provedores de banco existentes
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ClipperDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions));
                if (dbContextOptionsDescriptor != null)
                    services.Remove(dbContextOptionsDescriptor);

                // Remove registrações relacionadas ao Entity Framework
                var efDescriptors = services.Where(d => 
                    d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true).ToList();
                foreach (var descriptor in efDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Adiciona DbContext em memória
                services.AddDbContext<ClipperDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    options.EnableSensitiveDataLogging();
                });

                // Remove autenticação JWT existente COMPLETAMENTE
                var authServices = services.Where(d => 
                    d.ServiceType.FullName?.Contains("Microsoft.AspNetCore.Authentication") == true ||
                    d.ServiceType.FullName?.Contains("JwtBearer") == true ||
                    d.ServiceType.Name.Contains("Authentication")
                ).ToList();

                foreach (var service in authServices)
                {
                    services.Remove(service);
                }

                // Configurar autenticação de teste LIMPA
                services.AddAuthentication(TestAuthenticationHandler.TestScheme)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.TestScheme, options => { });

                // Adicionar autorização
                services.AddAuthorization();

                // Configura logging para testes
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            });

            // Configure JWT settings for testing
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:SecretKey"] = "ThisIsAVerySecureSecretKeyForTestingPurposes123456789012345678901234567890",
                    ["Jwt:Issuer"] = "ClipperTest",
                    ["Jwt:Audience"] = "ClipperTestUsers",
                    ["Jwt:ExpiryInMinutes"] = "15",
                    ["Jwt:RefreshTokenExpiryInDays"] = "7"
                });
            });

            builder.UseEnvironment("Testing");
        });

        Client = Factory.CreateClient();
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ClipperDbContext>();
        
        // Garantir que o banco está criado
        DbContext.Database.EnsureCreated();
    }

    /// <summary>
    /// Popula o banco com dados de teste básicos
    /// </summary>
    protected virtual async Task SeedTestDataAsync()
    {
        // Override em classes filhas se necessário
        await Task.CompletedTask;
    }

    /// <summary>
    /// Limpa dados do banco entre testes
    /// </summary>
    protected virtual async Task CleanupDatabaseAsync()
    {
        DbContext.Users.RemoveRange(DbContext.Users);
        DbContext.RefreshTokens.RemoveRange(DbContext.RefreshTokens);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Helper para fazer POST requests com JSON
    /// </summary>
    protected async Task<HttpResponseMessage> PostJsonAsync<T>(string requestUri, T data)
    {
        return await Client.PostAsJsonAsync(requestUri, data);
    }

    /// <summary>
    /// Helper para adicionar Authorization header
    /// </summary>
    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Helper para remover Authorization header
    /// </summary>
    protected void ClearAuthorizationHeader()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Cria um usuário de teste e faz login, retornando um token válido
    /// </summary>
    protected async Task<string> GetValidUserTokenAsync()
    {
        var registerRequest = new 
        {
            Name = "Test User",
            Email = $"user-{Guid.NewGuid()}@example.com",
            Password = "Password123!"
        };

        // Registrar usuário
        var registerResponse = await PostJsonAsync("/api/auth/register", registerRequest);
        if (registerResponse.IsSuccessStatusCode)
        {
            var authResponse = await registerResponse.Content.ReadFromJsonAsync<object>();
            var authProps = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(authResponse));
            
            if (authProps != null && authProps.TryGetValue("token", out var tokenObj))
            {
                return tokenObj.ToString()!;
            }
        }

        // Se o registro falhar (usuário já existe), tentar login
        var loginRequest = new 
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        var loginResponse = await PostJsonAsync("/api/auth/login", loginRequest);
        if (loginResponse.IsSuccessStatusCode)
        {
            var authResponse = await loginResponse.Content.ReadFromJsonAsync<object>();
            var authProps = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(authResponse));
            
            if (authProps != null && authProps.TryGetValue("token", out var tokenObj))
            {
                return tokenObj.ToString()!;
            }
        }

        throw new InvalidOperationException("Failed to get valid user token");
    }

    /// <summary>
    /// Cria um administrador de teste e faz login, retornando um token de admin
    /// </summary>
    protected async Task<string> GetValidAdminTokenAsync()
    {
        // Por simplicidade, vamos usar o mesmo método e assumir que o primeiro usuário será admin
        // Em um sistema real, você teria uma forma de criar usuários com roles específicos
        return await GetValidUserTokenAsync();
    }

    public virtual void Dispose()
    {
        Scope?.Dispose();
        Client?.Dispose();
        Factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}
