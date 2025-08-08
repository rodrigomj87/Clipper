using System.Security.Claims;
using Clipper.Common.Extensions;

namespace Clipper.API.Middleware;

/// <summary>
/// Middleware customizado para enriquecer informações do JWT
/// </summary>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Processa a requisição enriquecendo informações do usuário
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Enriquecer informações do usuário se autenticado
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await EnrichUserClaimsAsync(context);
            }

            // Log de tentativas de acesso para endpoints protegidos
            if (IsProtectedEndpoint(context))
            {
                LogAccessAttempt(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no middleware JWT customizado");
        }

        await _next(context);
    }

    /// <summary>
    /// Enriquece as claims do usuário com informações adicionais
    /// </summary>
    private Task EnrichUserClaimsAsync(HttpContext context)
    {
        try
        {
            var userId = context.User.GetUserId();
            var userEmail = context.User.GetUserEmail();

            if (userId > 0)
            {
                // Adicionar informações extras nos items do contexto para uso posterior
                context.Items["UserId"] = userId;
                context.Items["UserEmail"] = userEmail;
                context.Items["IsAuthenticated"] = true;

                // Log para auditoria
                _logger.LogDebug("Usuário autenticado: {UserId} ({Email})", userId, userEmail);

                // TODO: Aqui poderíamos buscar informações adicionais do usuário
                // como permissões específicas, configurações, etc.
                // await LoadAdditionalUserInfoAsync(context, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao enriquecer claims do usuário");
        }
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se o endpoint requer autenticação
    /// </summary>
    private bool IsProtectedEndpoint(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var hasAuthorize = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;
        var hasAllowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null;

        return hasAuthorize && !hasAllowAnonymous;
    }

    /// <summary>
    /// Registra tentativas de acesso para auditoria
    /// </summary>
    private void LogAccessAttempt(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            var userEmail = context.User.GetUserEmail();

            _logger.LogInformation(
                "Acesso autorizado: {Method} {Path} - Usuário: {UserId} ({Email}) - IP: {IP} - UserAgent: {UserAgent}",
                method, path, userId, userEmail, ipAddress, userAgent);
        }
        else
        {
            _logger.LogWarning(
                "Tentativa de acesso não autorizado: {Method} {Path} - IP: {IP} - UserAgent: {UserAgent}",
                method, path, ipAddress, userAgent);
        }
    }

    /// <summary>
    /// Carrega informações adicionais do usuário
    /// TODO: Implementar quando necessário
    /// </summary>
    private Task LoadAdditionalUserInfoAsync(HttpContext context, int userId)
    {
        // Implementação futura:
        // - Carregar permissões específicas do usuário
        // - Verificar status da conta
        // - Carregar configurações personalizadas
        // - Cache de informações frequentemente acessadas

        return Task.CompletedTask;
    }
}
