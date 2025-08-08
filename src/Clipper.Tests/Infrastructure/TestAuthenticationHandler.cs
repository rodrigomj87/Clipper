using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Clipper.Tests.Infrastructure;

/// <summary>
/// Handler de autenticação para testes
/// </summary>
/// <remarks>
/// Responsabilidade: simular autenticação para testes sem necessidade de tokens reais
/// </remarks>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestScheme = "Test";

    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();
        
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!authorizationHeader.StartsWith("Bearer "))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authentication header"));
        }

        var token = authorizationHeader["Bearer ".Length..];
        
        // Para testes, aceitar tokens específicos e criar claims correspondentes
        var claims = token switch
        {
            "valid-user-token" => new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, "User")
            },
            "valid-admin-token" => new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
                new Claim(ClaimTypes.Email, "admin@example.com"),
                new Claim(ClaimTypes.Name, "Test Admin"),
                new Claim(ClaimTypes.Role, "Admin")
            },
            "expired-token" => null, // Token expirado retorna null para falhar autenticação
            _ => Array.Empty<Claim>()
        };

        if (claims == null || !claims.Any())
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        var identity = new ClaimsIdentity(claims, TestScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
