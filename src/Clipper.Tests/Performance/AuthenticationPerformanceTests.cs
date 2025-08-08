using Microsoft.AspNetCore.Mvc.Testing;
using Clipper.Tests.Infrastructure;
using System.Text.Json;
using System.Diagnostics;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Tests.Performance;

/// <summary>
/// Testes de performance para autenticação
/// </summary>
/// <remarks>
/// Responsabilidade: validar performance sob carga dos endpoints de autenticação
/// </remarks>
public class AuthenticationPerformanceTests : IntegrationTestBase
{
    public AuthenticationPerformanceTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ConcurrentRequests_HandlesLoad()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginData = JsonSerializer.Serialize(new
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        var tasks = new List<Task<HttpResponseMessage>>();
        var sw = Stopwatch.StartNew();

        // Act - 50 requests concorrentes
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(Client.PostAsync("/api/auth/login", 
                new StringContent(loginData, System.Text.Encoding.UTF8, "application/json")));
        }

        var responses = await Task.WhenAll(tasks);
        sw.Stop();

        // Assert
        var successfulRequests = responses.Count(r => r.IsSuccessStatusCode);
        var averageTimePerRequest = sw.ElapsedMilliseconds / (double)responses.Length;

        successfulRequests.Should().BeGreaterThan(40); // Pelo menos 80% de sucesso
        averageTimePerRequest.Should().BeLessThan(1000); // Menos de 1 segundo por request em média

        // Cleanup
        Array.ForEach(responses, r => r.Dispose());
    }

    [Fact]
    public async Task TokenValidation_HighThroughput_PerformsWell()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginResponse = await LoginTestUserAsync();
        var validToken = loginResponse.Token;

        var tasks = new List<Task<HttpResponseMessage>>();
        var sw = Stopwatch.StartNew();

        // Act - 100 validações de token concorrentes
        for (int i = 0; i < 100; i++)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", validToken);
            tasks.Add(client.GetAsync("/api/test-authorization/protected"));
        }

        var responses = await Task.WhenAll(tasks);
        sw.Stop();

        // Assert
        var successfulRequests = responses.Count(r => r.IsSuccessStatusCode);
        var averageTimePerRequest = sw.ElapsedMilliseconds / (double)responses.Length;

        successfulRequests.Should().BeGreaterThan(95); // Pelo menos 95% de sucesso
        averageTimePerRequest.Should().BeLessThan(500); // Menos de 500ms por request em média

        // Cleanup
        Array.ForEach(responses, r => r.Dispose());
    }

    [Fact]
    public async Task Register_ConcurrentRegistrations_HandlesLoad()
    {
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();
        var sw = Stopwatch.StartNew();

        // Act - 20 registros concorrentes com emails únicos
        for (int i = 0; i < 20; i++)
        {
            var registerData = JsonSerializer.Serialize(new
            {
                Email = $"user{i}@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = $"User {i}"
            });

            tasks.Add(Client.PostAsync("/api/auth/register",
                new StringContent(registerData, System.Text.Encoding.UTF8, "application/json")));
        }

        var responses = await Task.WhenAll(tasks);
        sw.Stop();

        // Assert
        var successfulRequests = responses.Count(r => r.IsSuccessStatusCode);
        var averageTimePerRequest = sw.ElapsedMilliseconds / (double)responses.Length;

        successfulRequests.Should().BeGreaterThan(18); // Pelo menos 90% de sucesso
        averageTimePerRequest.Should().BeLessThan(2000); // Menos de 2 segundos por request em média

        // Cleanup
        Array.ForEach(responses, r => r.Dispose());
    }

    [Fact]
    public void JwtGeneration_Performance_BenchmarkTest()
    {
        // Arrange
        var user = CreateTestUser();
        var iterations = 1000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var jwtService = CreateJwtService();
            var token = jwtService.GenerateToken(user);
            token.Should().NotBeNullOrEmpty();
        }

        stopwatch.Stop();

        // Assert
        var averageTimePerToken = stopwatch.ElapsedMilliseconds / (double)iterations;
        averageTimePerToken.Should().BeLessThan(10); // Menos de 10ms por token

        var tokensPerSecond = iterations / stopwatch.Elapsed.TotalSeconds;
        tokensPerSecond.Should().BeGreaterThan(100); // Mais de 100 tokens por segundo
    }

    [Fact]
    public async Task MixedAuthenticationWorkload_RealisticScenario_PerformsWell()
    {
        // Arrange
        await SeedMultipleTestUsersAsync();
        var allTasks = new List<Task<HttpResponseMessage>>();
        var sw = Stopwatch.StartNew();

        // Cenário 1: 10 logins
        for (int i = 0; i < 10; i++)
        {
            var loginData = JsonSerializer.Serialize(new
            {
                Email = $"testuser{i % 5}@example.com", // 5 usuários diferentes
                Password = "Password123!"
            });

            allTasks.Add(Client.PostAsync("/api/auth/login",
                new StringContent(loginData, System.Text.Encoding.UTF8, "application/json")));
        }

        // Cenário 2: 20 acessos protegidos (simulando com tokens válidos)
        for (int i = 0; i < 20; i++)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-user-token");
            allTasks.Add(client.GetAsync("/api/test-authorization/protected"));
        }

        // Act
        var responses = await Task.WhenAll(allTasks);
        sw.Stop();

        // Assert
        var totalRequests = responses.Length;
        var successfulRequests = responses.Count(r => r.IsSuccessStatusCode);
        var successRate = (double)successfulRequests / totalRequests;
        var averageTimePerRequest = sw.ElapsedMilliseconds / (double)totalRequests;

        successRate.Should().BeGreaterThan(0.8); // 80% de sucesso
        averageTimePerRequest.Should().BeLessThan(1000); // Menos de 1 segundo por request em média

        // Cleanup
        Array.ForEach(responses, r => r.Dispose());
    }

    #region Helper Methods

    private async Task SeedTestUserAsync()
    {
        var uniqueEmail = $"test-{Guid.NewGuid()}@example.com";
        var user = new Domain.Entities.User
        {
            Email = uniqueEmail,
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedMultipleTestUsersAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            var user = new Domain.Entities.User
            {
                Email = $"testuser-{Guid.NewGuid()}@example.com",
                Name = $"Test User {i}",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            DbContext.Users.Add(user);
        }
        await DbContext.SaveChangesAsync();
    }

    private async Task<AuthResponse> LoginTestUserAsync()
    {
        var loginData = JsonSerializer.Serialize(new
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        var response = await Client.PostAsync("/api/auth/login", 
            new StringContent(loginData, System.Text.Encoding.UTF8, "application/json"));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    private Domain.Entities.User CreateTestUser()
    {
        return new Domain.Entities.User
        {
            Email = $"test-{Guid.NewGuid()}@example.com",
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private Clipper.Infrastructure.Services.JwtTokenService CreateJwtService()
    {
        var jwtSettings = new Common.Settings.JwtSettings
        {
            SecretKey = "my-super-secret-key-for-testing-purposes-32-chars-minimum",
            Issuer = "ClipperTest",
            Audience = "ClipperTestUsers",
            ExpirationInMinutes = 15,
            RefreshTokenExpirationInDays = 7
        };

        var jwtSettingsMock = new Mock<Microsoft.Extensions.Options.IOptions<Common.Settings.JwtSettings>>();
        jwtSettingsMock.Setup(x => x.Value).Returns(jwtSettings);

        var refreshTokenRepositoryMock = new Mock<Application.Common.Interfaces.IRefreshTokenRepository>();

        return new Clipper.Infrastructure.Services.JwtTokenService(jwtSettingsMock.Object, refreshTokenRepositoryMock.Object);
    }

    #endregion
}
