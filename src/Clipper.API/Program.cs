using Clipper.Infrastructure.Data;
using Clipper.Infrastructure.Configuration;
using Clipper.API.Extensions;
using Clipper.Common.Settings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Adicionar Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Helper para extrair IP do header X-Forwarded-For ou RemoteIpAddress
    string GetClientIp(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
        {
            var ip = forwarded.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(ip))
                return ip;
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    // Política global: 100 requisições por minuto por IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: GetClientIp(httpContext),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );

    // Política específica para login: 5 requisições por minuto por IP
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: GetClientIp(context),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
        )
    );
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});

// Configurar e validar JwtSettings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

// Validação crítica de segurança (exceto em ambiente de teste)
if (!builder.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    if (!jwtSettings.IsValid)
    {
        throw new InvalidOperationException(
            "Configurações JWT inválidas. Verifique se SecretKey, Issuer e Audience estão configurados corretamente.");
    }

    if (!jwtSettings.HasValidSecretKeyLength)
    {
        throw new InvalidOperationException(
            "A chave secreta JWT deve ter pelo menos 256 bits (32 caracteres). Use User Secrets ou variáveis de ambiente.");
    }
}

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Adicionar filtro global de exceções
    options.Filters.Add<Clipper.API.Filters.ApiExceptionFilterAttribute>();
});

// Registrar IMemoryCache
builder.Services.AddMemoryCache();

// Registrar RateLimitSettings
builder.Services.Configure<Clipper.Application.Common.Settings.RateLimitSettings>(builder.Configuration.GetSection("RateLimitSettings"));

// Registrar BruteForceProtectionService
builder.Services.AddSingleton<Clipper.Application.Common.Interfaces.IBruteForceProtectionService, Clipper.Infrastructure.Services.BruteForceProtectionService>();

// Registrar RateLimitingMiddleware (se for usado como middleware global)
// Registrar RateLimitingMiddleware como IRateLimitingService para uso no attribute
builder.Services.AddSingleton<Clipper.API.Attributes.IRateLimitingService, Clipper.API.Middleware.RateLimitingMiddleware>();

// Adicionar serviços de segurança (XSS, PasswordValidation)
builder.Services.AddSecurityServices();

// Configure MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Clipper.Application.Features.Authentication.Commands.Login.LoginCommand).Assembly);
});

// Add pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Clipper.Application.Common.Behaviors.ValidationBehavior<,>));

// Configure FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Clipper.Application.Features.Authentication.Commands.Login.LoginCommand).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clipper API", Version = "v1" });
    
    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Entity Framework
builder.Services.AddDbContext<ClipperDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registrar CorsSettings
builder.Services.Configure<Clipper.Application.Common.Settings.CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
// Registrar políticas CORS via extensão modular
builder.Services.AddCorsPolicy(builder.Configuration);

// Configure Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clipper API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

// Middleware global de rate limiting custom
app.UseMiddleware<Clipper.API.Middleware.RateLimitingMiddleware>();

// Middleware de Rate Limiting
app.UseRateLimiter();

app.UseHttpsRedirection();
// Middleware de headers de segurança (global)
app.UseMiddleware<Clipper.API.Middleware.SecurityHeadersMiddleware>();

// Aplicar política CORS conforme ambiente
Clipper.API.Configuration.CorsConfiguration.ConfigureCors(app, app.Environment);

// IMPORTANTE: Ordem correta do middleware
app.UseAuthentication(); // Deve vir antes de UseAuthorization
app.UseJwtMiddleware(); // Middleware customizado JWT
app.UseAuthorization();

// Mapear controllers com política de rate limiting para login


// Mapear os demais controllers normalmente
app.MapControllers();

// Endpoint de teste para verificar autenticação
app.MapGet("/test-auth", () => "Você está autenticado!")
    .RequireAuthorization()
    .WithName("TestAuth")
    .WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

// Tornar Program acessível para testes de integração
public partial class Program { }

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
