using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Clipper.API.Features.Authentication.Requests;
using Clipper.Application.Features.Authentication.Common;
using Clipper.Domain.Entities;
using Clipper.Tests.Infrastructure;

namespace Clipper.Tests.Integration.Authentication;

/// <summary>
/// Testes de integração para o AuthController
/// </summary>
/// <remarks>
/// Responsabilidade: validar endpoints de autenticação funcionando end-to-end
/// </remarks>
public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(loginRequest.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid-email",
            Password = ""
        };

        // Act
        var response = await PostJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithValidData_CreatesUserAndReturnsToken()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Name = "New User"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.User.Should().NotBeNull();
        authResponse.User.Email.Should().Be(registerRequest.Email);
        authResponse.User.Name.Should().Be(registerRequest.Name);

        // Verify user was created in database
        var userInDb = await DbContext.Users.FindAsync(authResponse.User.Id);
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be(registerRequest.Email);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        await SeedTestUserAsync();
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com", // Email já existe
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Name = "Duplicate User"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "invalid-email",
            Password = "123", // Muito curta
            ConfirmPassword = "different", // Não confere
            Name = ""
        };

        // Act
        var response = await PostJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewToken()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginResponse = await LoginTestUserAsync();
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = loginResponse.RefreshToken
        };

        // Act
        var response = await PostJsonAsync("/api/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
        authResponse.Token.Should().NotBe(loginResponse.Token); // Novo token
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest
        {
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        await SeedTestUserAsync();
        var loginResponse = await LoginTestUserAsync();
        SetAuthorizationHeader(loginResponse.Token);
        
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = loginResponse.RefreshToken
        };

        // Act
        var response = await PostJsonAsync("/api/auth/logout", logoutRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Logout_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var logoutRequest = new LogoutRequest
        {
            RefreshToken = "some-refresh-token"
        };

        // Act
        var response = await PostJsonAsync("/api/auth/logout", logoutRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #region Helper Methods

    /// <summary>
    /// Cria um usuário de teste no banco
    /// </summary>
    private async Task SeedTestUserAsync()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // Hash da senha
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Faz login com o usuário de teste e retorna a resposta
    /// </summary>
    private async Task<AuthResponse> LoginTestUserAsync()
    {
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var response = await PostJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!;
    }

    #endregion
}
