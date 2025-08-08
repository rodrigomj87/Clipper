using Clipper.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Clipper.Tests.Infrastructure;

namespace Clipper.Tests.Infrastructure;

/// <summary>
/// Configuração de programa específica para testes
/// </summary>
public class TestProgram
{
    public static WebApplication CreateTestApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        // Configure test database
        builder.Services.AddDbContext<ClipperDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Add test authentication
        builder.Services.AddAuthentication(TestAuthenticationHandler.TestScheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.TestScheme, options => { });

        // Add authorization
        builder.Services.AddAuthorization();

        // Configure logging for tests  
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
