using Clipper.Infrastructure.Data;
using Clipper.Infrastructure.Configuration;
using Clipper.API.Extensions;
using Clipper.Common.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// Configure MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Clipper.Application.Features.Authentication.Commands.Login.LoginCommand).Assembly);
});

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
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

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

app.UseHttpsRedirection();

// Configure CORS
app.UseCors("AllowAngular");

// IMPORTANTE: Ordem correta do middleware
app.UseAuthentication(); // Deve vir antes de UseAuthorization
app.UseJwtMiddleware(); // Middleware customizado JWT
app.UseAuthorization();

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
