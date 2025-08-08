using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Clipper.Tests.Infrastructure;

namespace Clipper.Tests.Integration.Authorization;

/// <summary>
/// Testes de integração para autorização e middleware JWT
/// </summary>
/// <remarks>
/// Responsabilidade: validar funcionamento do middleware de autorização e policies
/// </remarks>
public class AuthorizationTests : IntegrationTestBase
{
    public AuthorizationTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/test-authorization/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithInvalidToken_Returns401()
    {
        // Arrange
        SetAuthorizationHeader("invalid-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithExpiredToken_Returns401()
    {
        // Arrange
        SetAuthorizationHeader("expired-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/protected");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_WithUserToken_Returns403()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/admin-only");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AdminEndpoint_WithAdminToken_Returns200()
    {
        // Arrange
        SetAuthorizationHeader("valid-admin-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/admin-only");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AdminEndpoint_WithoutToken_Returns401()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/test-authorization/admin-only");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OwnershipEndpoint_WithOwnerToken_Returns200()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");
        var resourceId = 1; // Resource ID genérico para teste

        // Act
        var response = await Client.GetAsync($"/api/test-authorization/ownership/{resourceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task OwnershipEndpoint_WithDifferentUser_Returns403()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");
        var resourceId = 999; // Recurso que não pertence ao usuário

        // Act
        var response = await Client.GetAsync($"/api/test-authorization/ownership/{resourceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task OwnershipEndpoint_WithAdminToken_Returns200()
    {
        // Arrange - Admin pode acessar qualquer recurso
        SetAuthorizationHeader("valid-admin-token");
        var resourceId = 1;

        // Act
        var response = await Client.GetAsync($"/api/test-authorization/ownership/{resourceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublicEndpoint_WithoutToken_Returns200()
    {
        // Arrange
        ClearAuthorizationHeader();

        // Act
        var response = await Client.GetAsync("/api/test-authorization/public");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PublicEndpoint_WithToken_Returns200()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/public");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task JwtMiddleware_ProcessesTokensCorrectly()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/user-info");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var userInfo = await response.Content.ReadFromJsonAsync<object>();
        userInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task CorsPolicy_AllowsCrossOriginRequests()
    {
        // Arrange
        Client.DefaultRequestHeaders.Add("Origin", "http://localhost:3000");

        // Act
        var response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Options, "/api/auth/login"));

        // Assert
        response.Headers.Should().ContainKey("Access-Control-Allow-Origin");
    }

    [Fact]
    public async Task RateLimiting_BlocksExcessiveRequests()
    {
        // Arrange
        var loginRequest = new { Email = "test@example.com", Password = "password" };
        var responses = new List<HttpResponseMessage>();

        // Act - Fazer muitas requisições rapidamente
        for (int i = 0; i < 20; i++)
        {
            var response = await PostJsonAsync("/api/auth/login", loginRequest);
            responses.Add(response);
        }

        // Assert - Algumas requisições devem ser bloqueadas
        var tooManyRequestsResponses = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        tooManyRequestsResponses.Should().BeGreaterThan(0);

        // Cleanup
        responses.ForEach(r => r.Dispose());
    }

    [Fact]
    public async Task SecurityHeaders_ArePresent()
    {
        // Arrange
        SetAuthorizationHeader("valid-user-token");

        // Act
        var response = await Client.GetAsync("/api/test-authorization/protected");

        // Assert
        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("X-XSS-Protection");
        // Outros headers de segurança conforme configuração
    }
}
