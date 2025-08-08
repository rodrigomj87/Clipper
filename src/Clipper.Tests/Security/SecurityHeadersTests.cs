/**
 * @file: SecurityHeadersTests.cs
 * @responsibility: Testes unitários para middleware de headers de segurança
 * @exports: SecurityHeadersTests
 * @imports: WebApplicationFactory, HttpClient
 * @layer: Tests/Security
 */
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;

namespace Clipper.Tests.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class SecurityHeadersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(WebApplicationFactory<Program> factory)
    {
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    {"JwtSettings:SecretKey", "my-super-secret-key-for-testing-purposes-32-chars-minimum"},
                    {"JwtSettings:Issuer", "ClipperTest"},
                    {"JwtSettings:Audience", "ClipperTestUsers"},
                    {"JwtSettings:ExpirationInMinutes", "15"},
                    {"JwtSettings:RefreshTokenExpirationInDays", "7"}
                };
                config.AddInMemoryCollection(dict);
            });
            builder.ConfigureServices(services =>
            {
                var jwtSettings = new Clipper.Common.Settings.JwtSettings
                {
                    SecretKey = "my-super-secret-key-for-testing-purposes-32-chars-minimum",
                    Issuer = "ClipperTest",
                    Audience = "ClipperTestUsers",
                    ExpirationInMinutes = 15,
                    RefreshTokenExpirationInDays = 7
                };
                services.AddSingleton<Microsoft.Extensions.Options.IOptions<Clipper.Common.Settings.JwtSettings>>(new Microsoft.Extensions.Options.OptionsWrapper<Clipper.Common.Settings.JwtSettings>(jwtSettings));
            });
        });
        _client = customFactory.CreateClient();
    }

    [Fact]
    public async Task SecurityHeaders_ArePresent()
    {
        var response = await _client.GetAsync("/api/auth/login");
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.True(response.Headers.Contains("X-XSS-Protection"));
        Assert.True(response.Headers.Contains("Strict-Transport-Security"));
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        Assert.True(response.Headers.Contains("Referrer-Policy"));
    }
}
