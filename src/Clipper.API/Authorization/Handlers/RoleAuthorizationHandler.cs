using Microsoft.AspNetCore.Authorization;
using Clipper.API.Authorization.Requirements;
using System.Security.Claims;

namespace Clipper.API.Authorization.Handlers;

/// <summary>
/// Handler para verificação de roles customizadas
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly ILogger<RoleAuthorizationHandler> _logger;

    public RoleAuthorizationHandler(ILogger<RoleAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Verifica se o usuário possui as roles necessárias
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        RoleRequirement requirement)
    {
        try
        {
            // Verificar se usuário está autenticado
            if (!context.User.Identity?.IsAuthenticated == true)
            {
                _logger.LogWarning("Usuário não autenticado tentando acessar recurso que requer roles");
                context.Fail();
                return Task.CompletedTask;
            }

            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            _logger.LogDebug("Usuário possui roles: {UserRoles}", string.Join(", ", userRoles));
            _logger.LogDebug("Roles necessárias: {RequiredRoles}, RequireAll: {RequireAll}", 
                string.Join(", ", requirement.RequiredRoles), requirement.RequireAllRoles);

            bool hasRequiredRoles;

            if (requirement.RequireAllRoles)
            {
                // Usuário deve ter TODAS as roles
                hasRequiredRoles = requirement.RequiredRoles.All(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            }
            else
            {
                // Usuário deve ter pelo menos UMA das roles
                hasRequiredRoles = requirement.RequiredRoles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            }

            if (hasRequiredRoles)
            {
                _logger.LogInformation("Usuário possui roles necessárias para acesso");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Usuário não possui roles necessárias para acesso");
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante verificação de roles");
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
