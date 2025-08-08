using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Clipper.API.Features.Authentication.Requests;
using Clipper.Application.Features.Authentication.Common;
using Clipper.Domain.Entities;
using Clipper.Infrastructure.Data;
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
            Email = _lastTestUserEmail!,
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
        // Use a new scope to get fresh context like the app does
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ClipperDbContext>();
        
        var allUsers = await dbContext.Users.ToListAsync();
        allUsers.Count.Should().BeGreaterThan(0, "Should have at least one user in database");
        
        var userInDb = await dbContext.Users.FindAsync(authResponse.User.Id);
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
            Email = _lastTestUserEmail!, // Email já existe
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
        SetAuthorizationHeader("valid-user-token");
        
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
        var uniqueEmail = $"test-{Guid.NewGuid()}@example.com";
        var user = new User
        {
            Email = uniqueEmail,
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync();
        // Salva o e-mail para uso nos testes
        _lastTestUserEmail = uniqueEmail;
    }

    /// <summary>
    /// Faz login com o usuário de teste e retorna a resposta
    /// </summary>
    private async Task<AuthResponse> LoginTestUserAsync()
    {
        var loginRequest = new LoginRequest
        {
            Email = _lastTestUserEmail ?? "test@example.com",
            Password = "Password123!"
        };

        var response = await PostJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!;
    }
    // Campo privado para armazenar o e-mail do último usuário de teste criado
    private string? _lastTestUserEmail;
}
    

    #endregion
    
