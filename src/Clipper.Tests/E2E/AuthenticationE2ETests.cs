using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Clipper.Tests.Infrastructure;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Tests.E2E;

/// <summary>
/// Testes End-to-End para fluxos completos de autenticação
/// </summary>
/// <remarks>
/// Responsabilidade: validar cenários reais de uso do sistema de autenticação
/// </remarks>
public class AuthenticationE2ETests : IntegrationTestBase
{
    public AuthenticationE2ETests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task FullAuthenticationFlow_RegisterLoginRefreshLogout_WorksCorrectly()
    {
        // Arrange
        var userEmail = $"e2etest{Guid.NewGuid()}@example.com";
        var userName = "E2E Test User";
        var password = "Password123!";

        // Step 1: Register
        var registerRequest = new
        {
            Email = userEmail,
            Password = password,
            ConfirmPassword = password,
            Name = userName
        };

        var registerResponse = await PostJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var registerAuthResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        registerAuthResponse.Should().NotBeNull();
        registerAuthResponse!.User.Email.Should().Be(userEmail);
        registerAuthResponse.User.Name.Should().Be(userName);
        registerAuthResponse.Token.Should().NotBeNullOrEmpty();
        registerAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();

        var initialAccessToken = registerAuthResponse.Token;
        var initialRefreshToken = registerAuthResponse.RefreshToken;

        // Step 2: Use access token to access protected resource
        SetAuthorizationHeader(initialAccessToken);
        var protectedResponse = await Client.GetAsync("/api/test-authorization/protected");
        protectedResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Login with same credentials
        var loginRequest = new
        {
            Email = userEmail,
            Password = password
        };

        var loginResponse = await PostJsonAsync("/api/auth/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginAuthResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loginAuthResponse.Should().NotBeNull();
        loginAuthResponse!.User.Email.Should().Be(userEmail);
        loginAuthResponse.Token.Should().NotBeNullOrEmpty();
        loginAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
        loginAuthResponse.Token.Should().NotBe(initialAccessToken); // New token

        // Step 4: Refresh token
        var refreshRequest = new
        {
            RefreshToken = loginAuthResponse.RefreshToken
        };

        var refreshResponse = await PostJsonAsync("/api/auth/refresh-token", refreshRequest);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        refreshAuthResponse.Should().NotBeNull();
        refreshAuthResponse!.Token.Should().NotBeNullOrEmpty();
        refreshAuthResponse.RefreshToken.Should().NotBeNullOrEmpty();
        refreshAuthResponse.Token.Should().NotBe(loginAuthResponse.Token); // New token again

        // Step 5: Use new access token
        SetAuthorizationHeader(refreshAuthResponse.Token);
        var protectedResponse2 = await Client.GetAsync("/api/test-authorization/protected");
        protectedResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 6: Logout
        var logoutRequest = new
        {
            RefreshToken = refreshAuthResponse.RefreshToken
        };

        var logoutResponse = await PostJsonAsync("/api/auth/logout", logoutRequest);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 7: Verify refresh token is invalidated
        var invalidRefreshRequest = new
        {
            RefreshToken = refreshAuthResponse.RefreshToken
        };

        var invalidRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", invalidRefreshRequest);
        invalidRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MultipleUsers_ConcurrentAuthentication_WorksCorrectly()
    {
        // Arrange
        var userCount = 5;
        var users = new List<(string email, string name, string password)>();
        
        for (int i = 0; i < userCount; i++)
        {
            users.Add((
                email: $"concurrentuser{i}{Guid.NewGuid()}@example.com",
                name: $"Concurrent User {i}",
                password: "Password123!"
            ));
        }

        // Step 1: Register all users concurrently
        var registerTasks = users.Select(user => PostJsonAsync("/api/auth/register", new
        {
            Email = user.email,
            Password = user.password,
            ConfirmPassword = user.password,
            Name = user.name
        })).ToArray();

        var registerResponses = await Task.WhenAll(registerTasks);

        // Verify all registrations succeeded
        foreach (var response in registerResponses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Step 2: Login all users concurrently
        var loginTasks = users.Select(user => PostJsonAsync("/api/auth/login", new
        {
            Email = user.email,
            Password = user.password
        })).ToArray();

        var loginResponses = await Task.WhenAll(loginTasks);

        // Verify all logins succeeded and get auth responses
        var authResponses = new List<AuthResponse>();
        for (int i = 0; i < loginResponses.Length; i++)
        {
            loginResponses[i].StatusCode.Should().Be(HttpStatusCode.OK);
            var authResponse = await loginResponses[i].Content.ReadFromJsonAsync<AuthResponse>();
            authResponse.Should().NotBeNull();
            authResponse!.User.Email.Should().Be(users[i].email);
            authResponses.Add(authResponse);
        }

        // Step 3: Each user accesses protected resources concurrently
        var clients = new List<HttpClient>();
        for (int i = 0; i < userCount; i++)
        {
            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponses[i].Token);
            clients.Add(client);
        }

        var protectedAccessTasks = clients.Select(client => 
            client.GetAsync("/api/test-authorization/protected")).ToArray();

        var protectedResponses = await Task.WhenAll(protectedAccessTasks);

        // Verify all protected accesses succeeded
        foreach (var response in protectedResponses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Step 4: Refresh tokens concurrently
        var refreshTasks = authResponses.Select(auth => PostJsonAsync("/api/auth/refresh-token", new
        {
            RefreshToken = auth.RefreshToken
        })).ToArray();

        var refreshResponses = await Task.WhenAll(refreshTasks);

        // Verify all refreshes succeeded
        foreach (var response in refreshResponses)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Cleanup
        Array.ForEach(registerResponses, r => r.Dispose());
        Array.ForEach(loginResponses, r => r.Dispose());
        Array.ForEach(protectedResponses, r => r.Dispose());
        Array.ForEach(refreshResponses, r => r.Dispose());
        clients.ForEach(c => c.Dispose());
    }

    [Fact]
    public async Task UserJourney_RegistrationToResourceAccess_CompletesSuccessfully()
    {
        // Arrange
        var userEmail = $"journey{Guid.NewGuid()}@example.com";
        var password = "StrongPassword123!";

        // Journey Step 1: New user registers
        var registerRequest = new
        {
            Email = userEmail,
            Password = password,
            ConfirmPassword = password,
            Name = "Journey User"
        };

        var registerResponse = await PostJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var registerAuthResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        var accessToken = registerAuthResponse!.Token;

        // Journey Step 2: User immediately accesses their profile (protected)
        SetAuthorizationHeader(accessToken);
        var profileResponse = await Client.GetAsync("/api/test-authorization/user-info");
        profileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Journey Step 3: User tries to access admin resource (should fail)
        var adminResponse = await Client.GetAsync("/api/test-authorization/admin-only");
        adminResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Journey Step 4: User accesses their own resources (ownership)
        var userId = registerAuthResponse.User.Id;
        var ownershipResponse = await Client.GetAsync($"/api/test-authorization/ownership/{userId}");
        ownershipResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Journey Step 5: User tries to access other user's resources (should fail)
        var otherUserId = Guid.NewGuid();
        var otherOwnershipResponse = await Client.GetAsync($"/api/test-authorization/ownership/{otherUserId}");
        otherOwnershipResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        // Journey Step 6: Token expires simulation (refresh)
        var refreshRequest = new
        {
            RefreshToken = registerAuthResponse.RefreshToken
        };

        var refreshResponse = await PostJsonAsync("/api/auth/refresh-token", refreshRequest);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshAuthResponse = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        var newAccessToken = refreshAuthResponse!.Token;

        // Journey Step 7: User continues using new token
        SetAuthorizationHeader(newAccessToken);
        var continuedAccessResponse = await Client.GetAsync("/api/test-authorization/protected");
        continuedAccessResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Journey Step 8: User logs out
        var logoutRequest = new
        {
            RefreshToken = refreshAuthResponse.RefreshToken
        };

        var logoutResponse = await PostJsonAsync("/api/auth/logout", logoutRequest);
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Journey Step 9: User tries to access protected resource after logout (should fail)
        var postLogoutResponse = await Client.GetAsync("/api/test-authorization/protected");
        postLogoutResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TokenLifecycle_ExpirationAndRenewal_HandledCorrectly()
    {
        // Arrange
        await SeedTestUserAsync();
        
        // Step 1: Initial login
        var loginResponse = await LoginTestUserAsync();
        var initialTokens = loginResponse;

        // Step 2: Use token immediately (should work)
        SetAuthorizationHeader(initialTokens.Token);
        var immediateResponse = await Client.GetAsync("/api/test-authorization/protected");
        immediateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Refresh token to get new tokens
        var firstRefresh = await RefreshTokenAsync(initialTokens.RefreshToken);
        firstRefresh.Should().NotBeNull();
        firstRefresh!.Token.Should().NotBe(initialTokens.Token);
        firstRefresh.RefreshToken.Should().NotBe(initialTokens.RefreshToken);

        // Step 4: Use new access token (should work)
        SetAuthorizationHeader(firstRefresh.Token);
        var newTokenResponse = await Client.GetAsync("/api/test-authorization/protected");
        newTokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 5: Try to reuse old refresh token (should fail)
        var oldRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", new
        {
            RefreshToken = initialTokens.RefreshToken
        });
        oldRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Step 6: Multiple refresh cycles
        var currentTokens = firstRefresh;
        for (int i = 0; i < 3; i++)
        {
            var nextTokens = await RefreshTokenAsync(currentTokens.RefreshToken);
            nextTokens.Should().NotBeNull();
            nextTokens!.Token.Should().NotBe(currentTokens.Token);
            
            // Test new token works
            SetAuthorizationHeader(nextTokens.Token);
            var testResponse = await Client.GetAsync("/api/test-authorization/protected");
            testResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            currentTokens = nextTokens;
        }

        // Step 7: Final logout invalidates current refresh token
        var finalLogoutResponse = await PostJsonAsync("/api/auth/logout", new
        {
            RefreshToken = currentTokens.RefreshToken
        });
        finalLogoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 8: Verify refresh token is invalidated
        var postLogoutRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", new
        {
            RefreshToken = currentTokens.RefreshToken
        });
        postLogoutRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ErrorRecovery_SystemHandlesFailuresGracefully()
    {
        // Arrange
        var userEmail = $"recovery{Guid.NewGuid()}@example.com";

        // Scenario 1: Login with non-existent user
        var invalidLoginResponse = await PostJsonAsync("/api/auth/login", new
        {
            Email = userEmail,
            Password = "Password123!"
        });
        invalidLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Scenario 2: Register user
        var registerResponse = await PostJsonAsync("/api/auth/register", new
        {
            Email = userEmail,
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Name = "Recovery User"
        });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Scenario 3: Try to register same user again (should fail gracefully)
        var duplicateRegisterResponse = await PostJsonAsync("/api/auth/register", new
        {
            Email = userEmail,
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Name = "Duplicate User"
        });
        duplicateRegisterResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Scenario 4: Login with wrong password
        var wrongPasswordResponse = await PostJsonAsync("/api/auth/login", new
        {
            Email = userEmail,
            Password = "WrongPassword!"
        });
        wrongPasswordResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Scenario 5: Login with correct password (should work)
        var correctLoginResponse = await PostJsonAsync("/api/auth/login", new
        {
            Email = userEmail,
            Password = "Password123!"
        });
        correctLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Scenario 6: Use invalid refresh token
        var invalidRefreshResponse = await PostJsonAsync("/api/auth/refresh-token", new
        {
            RefreshToken = "invalid-refresh-token"
        });
        invalidRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Scenario 7: Access protected resource without token
        ClearAuthorizationHeader();
        var noTokenResponse = await Client.GetAsync("/api/test-authorization/protected");
        noTokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // System should remain stable and functional throughout all error scenarios
        var healthCheckResponse = await Client.GetAsync("/api/test-authorization/public");
        healthCheckResponse.StatusCode.Should().Be(HttpStatusCode.OK);
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

    private async Task<AuthResponse> LoginTestUserAsync()
    {
        var loginRequest = new
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var response = await PostJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!;
    }

    private async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var refreshRequest = new
        {
            RefreshToken = refreshToken
        };

        var response = await PostJsonAsync("/api/auth/refresh-token", refreshRequest);
        
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    #endregion
}
