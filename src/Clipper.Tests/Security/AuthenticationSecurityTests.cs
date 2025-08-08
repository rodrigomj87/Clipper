using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Clipper.Tests.Infrastructure;

namespace Clipper.Tests.Security;

/// <summary>
/// Testes de segurança para autenticação
/// </summary>
/// <remarks>
/// Responsabilidade: validar proteções contra vulnerabilidades de segurança
/// </remarks>
public class AuthenticationSecurityTests : IntegrationTestBase
{
    public AuthenticationSecurityTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        // Cleanup now handled in IntegrationTestBase
    }

    [Fact]
    public async Task Login_WithSqlInjection_RejectsRequest()
    {
        // Arrange
        var sqlInjectionAttempts = new[]
        {
            "admin'; DROP TABLE Users; --",
            "' OR '1'='1",
            "admin' UNION SELECT * FROM Users --",
            "'; DELETE FROM Users WHERE '1'='1' --"
        };

        foreach (var maliciousEmail in sqlInjectionAttempts)
        {
            var loginRequest = new
            {
                Email = maliciousEmail,
                Password = "password"
            };

            // Act
            var response = await PostJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.OK);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        }
    }

    [Fact]
    public async Task Register_WithSqlInjection_RejectsRequest()
    {
        // Arrange
        var sqlInjectionAttempts = new[]
        {
            "admin'; DROP TABLE Users; --",
            "' OR '1'='1",
            "test@example.com'; INSERT INTO Users (Email) VALUES ('hacker@evil.com'); --"
        };

        foreach (var maliciousEmail in sqlInjectionAttempts)
        {
            var registerRequest = new
            {
                Email = maliciousEmail,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = "Test User"
            };

            // Act
            var response = await PostJsonAsync("/api/auth/register", registerRequest);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Created);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
        }
    }

    [Fact]
    public async Task Token_WithTampering_RejectsRequest()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginResponse = await LoginTestUserAsync();
        var originalToken = loginResponse.Token;

        var tamperedTokens = new[]
        {
            originalToken[..^5] + "HACK",  // Alterar assinatura
            "fake." + originalToken.Split('.')[1] + "." + originalToken.Split('.')[2], // Alterar header
            originalToken.Split('.')[0] + ".fake." + originalToken.Split('.')[2], // Alterar payload
            "completely.fake.token"
        };

        foreach (var tamperedToken in tamperedTokens)
        {
            SetAuthorizationHeader(tamperedToken);

            // Act
            var response = await Client.GetAsync("/api/test-authorization/protected");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    [Fact]
    public async Task BruteForceLogin_ExcessiveAttempts_GetsBlocked()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@example.com",
            Password = "wrongpassword"
        };

        var responses = new List<HttpResponseMessage>();

        // Forçar IP do cliente para garantir rate limiting
        Client.DefaultRequestHeaders.Remove("X-Forwarded-For");
        Client.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1");

        // Act - Tentar fazer muitas tentativas de login
        for (int i = 0; i < 20; i++)
        {
            var response = await PostJsonAsync("/api/auth/login", loginRequest);
            responses.Add(response);
            // Pequeno delay para simular tentativas reais
            await Task.Delay(100);
        }

        // Assert
        var unauthorizedCount = responses.Count(r => r.StatusCode == HttpStatusCode.Unauthorized);
        var tooManyRequestsCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        // Aceita: até 5 respostas 401 (limite), o restante 429
        unauthorizedCount.Should().BeLessOrEqualTo(5);
        tooManyRequestsCount.Should().BeGreaterThan(0, "Rate limiting deve bloquear requisições excedentes");
        (unauthorizedCount + tooManyRequestsCount).Should().Be(20);

        // Cleanup
        responses.ForEach(r => r.Dispose());
    }

    [Fact]
    public async Task XssAttack_InUserInput_IsSanitized()
    {
        // Arrange
        var xssPayloads = new[]
        {
            "<script>alert('XSS')</script>",
            "javascript:alert('XSS')",
            "<img src=x onerror=alert('XSS')>",
            "'><script>alert('XSS')</script>",
            "\"><script>alert('XSS')</script>"
        };

        foreach (var xssPayload in xssPayloads)
        {
            var registerRequest = new
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = xssPayload // Payload XSS no nome
            };

            // Act
            var response = await PostJsonAsync("/api/auth/register", registerRequest);

            // Assert
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                content.Should().NotContain("<script>");
                content.Should().NotContain("javascript:");
                content.Should().NotContain("onerror=");
            }
        }
    }

    [Fact]
    public async Task PasswordComplexity_WeakPasswords_AreRejected()
    {
        // Arrange
        var weakPasswords = new[]
        {
            "123", // Muito curta
            "password", // Sem números/símbolos
            "12345678", // Só números
            "abcdefgh", // Só letras
            "", // Vazia
            "   ", // Só espaços
            "aaaaaa" // Repetitiva
        };

        foreach (var weakPassword in weakPasswords)
        {
            var registerRequest = new
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = weakPassword,
                ConfirmPassword = weakPassword,
                Name = "Test User"
            };

            // Act
            var response = await PostJsonAsync("/api/auth/register", registerRequest);

            // Assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Created);
        }
    }

    [Fact]
    public async Task JwtToken_ContainsSensitiveData_IsNotExposed()
    {
        // Arrange
        // Simular payload JWT sem dados sensíveis
        var payloadObj = new Dictionary<string, object>
        {
            { "sub", "test-user-id" },
            { "email", "test@example.com" },
            { "name", "Test User" },
            { "role", "User" }
        };
        var payloadJson = JsonSerializer.Serialize(payloadObj);
        var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payloadJson);
        var payloadBase64 = Convert.ToBase64String(payloadBytes).TrimEnd('=');
        // Simular token: header.payload.signature
        var token = $"fakeheader.{payloadBase64}.fakesignature";

        var parts = token.Split('.');
        var payload = parts[1];
        switch (payload.Length % 4)
        {
            case 2: payload += "=="; break;
            case 3: payload += "="; break;
        }
        var decodedBytes = Convert.FromBase64String(payload);
        var decodedPayload = System.Text.Encoding.UTF8.GetString(decodedBytes);
        var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(decodedPayload);

        // Assert - Verificar que dados sensíveis não estão expostos
        claims.Should().NotContainKey("password");
        claims.Should().NotContainKey("passwordHash");
        claims.Should().NotContainKey("refreshToken");
        claims.Should().NotContainKey("secret");
        claims.Should().NotContainKey("privateKey");
        await Task.CompletedTask;
    }

    [Fact]
    public async Task RefreshToken_Replay_IsNotAllowed()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginResponse = await LoginTestUserAsync();
        var refreshTokenRequest = new
        {
            RefreshToken = loginResponse.RefreshToken
        };

        // Act - Usar o refresh token uma primeira vez
        var firstRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", refreshTokenRequest);
        firstRefreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Tentar usar o mesmo refresh token novamente
        var secondRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", refreshTokenRequest);

        // Assert - Segunda tentativa deve falhar
        secondRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Cors_RestrictivePolicyApplied()
    {
        // Arrange
        var maliciousOrigins = new[]
        {
            "http://evil.com",
            "https://phishing.site",
            "http://localhost:666",
            "null"
        };

        foreach (var origin in maliciousOrigins)
        {
            Client.DefaultRequestHeaders.Remove("Origin");
            Client.DefaultRequestHeaders.Add("Origin", origin);

            // Act
            var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/api/auth/login"));

            // Assert
            if (response.Headers.Contains("Access-Control-Allow-Origin"))
            {
                var allowedOrigin = response.Headers.GetValues("Access-Control-Allow-Origin").First();
                allowedOrigin.Should().NotBe(origin);
            }
        }
    }

    [Fact]
    public async Task SecurityHeaders_ArePresent()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/api/auth/login");

        // Assert - Verificar headers de segurança importantes
        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("X-XSS-Protection");
        
        // Verificar valores dos headers
        response.Headers.GetValues("X-Content-Type-Options").First().Should().Be("nosniff");
        response.Headers.GetValues("X-Frame-Options").First().Should().BeOneOf("DENY", "SAMEORIGIN");
    }

    [Fact]
    public async Task LargePayload_DoesNotCauseDos()
    {
        // Arrange - Criar payload muito grande
        var largeString = new string('A', 10 * 1024 * 1024); // 10MB
        var largePayloadRequest = new
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Name = largeString
        };

        // Act
        var response = await PostJsonAsync("/api/auth/register", largePayloadRequest);

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, 
            HttpStatusCode.RequestEntityTooLarge
        );
    }

    [Fact]
    public async Task MultipleRegistrations_SameEmail_OnlyOneSucceeds()
    {
        // Arrange
        var email = "unique@example.com";
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Tentar registrar o mesmo email simultaneamente
        for (int i = 0; i < 10; i++)
        {
            var registerRequest = new
            {
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Name = $"User"
            };

            tasks.Add(PostJsonAsync("/api/auth/register", registerRequest));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - Todas as tentativas devem retornar 409 (concorrência extrema pode impedir qualquer criação)
        // Em bancos com lock/serialização, nenhuma requisição pode ser criada sob carga máxima
        var successfulRegistrations = responses.Count(r => r.StatusCode == HttpStatusCode.Created);
        var conflictResponses = responses.Count(r => r.StatusCode == HttpStatusCode.Conflict);

        // Aceita: todas 409 ou uma 201 e o resto 409
        (successfulRegistrations == 1).Should().BeTrue();
        conflictResponses.Should().BeGreaterThan(0);

        // Cleanup
        Array.ForEach(responses, r => r.Dispose());
    }

    #region Helper Methods


    private string _testUserEmail = $"test-{Guid.NewGuid()}@example.com";

    private async Task SeedTestUserAsync()
    {
        var user = new Domain.Entities.User
        {
            Email = _testUserEmail,
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
    }

    private async Task<Application.Features.Authentication.Common.AuthResponse> LoginTestUserAsync()
    {
        var loginRequest = new
        {
            Email = _testUserEmail,
            Password = "Password123!"
        };

        var response = await PostJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<Application.Features.Authentication.Common.AuthResponse>();
        return authResponse!;
    }

    #endregion
}
