using Microsoft.AspNetCore.Authorization;
using Clipper.API.Authorization.Requirements;
using Clipper.Common.Extensions;
using System.Security.Claims;

namespace Clipper.API.Authorization.Handlers;

/// <summary>
/// Handler para verificação de propriedade de recursos
/// </summary>
public class OwnershipAuthorizationHandler : AuthorizationHandler<OwnershipRequirement>
{
    private readonly ILogger<OwnershipAuthorizationHandler> _logger;

    public OwnershipAuthorizationHandler(ILogger<OwnershipAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Verifica se o usuário tem propriedade sobre o recurso
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        OwnershipRequirement requirement)
    {
        try
        {
            // Verificar se usuário está autenticado
            if (!context.User.Identity?.IsAuthenticated == true)
            {
                _logger.LogWarning("Usuário não autenticado tentando acessar recurso protegido");
                context.Fail();
                return Task.CompletedTask;
            }

            // Admins têm acesso a tudo
            if (context.User.IsInRole("Admin"))
            {
                _logger.LogInformation("Admin acessando recurso - acesso liberado");
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Obter ID do usuário
            var userId = context.User.GetUserId();
            if (userId <= 0)
            {
                _logger.LogWarning("ID do usuário não encontrado nos claims ou é inválido");
                context.Fail();
                return Task.CompletedTask;
            }

            // Verificar se temos acesso ao contexto HTTP
            if (context.Resource is not Microsoft.AspNetCore.Http.DefaultHttpContext httpContext)
            {
                _logger.LogWarning("Contexto HTTP não disponível para verificação de propriedade");
                context.Fail();
                return Task.CompletedTask;
            }

            // Obter ID do recurso da rota
            if (!httpContext.Request.RouteValues.TryGetValue(requirement.ResourceIdRouteKey, out var resourceIdValue))
            {
                _logger.LogWarning("Parâmetro de rota '{RouteKey}' não encontrado", requirement.ResourceIdRouteKey);
                context.Fail();
                return Task.CompletedTask;
            }

            // Validar se o ID do recurso é um número válido
            if (!int.TryParse(resourceIdValue?.ToString(), out var resourceId))
            {
                _logger.LogWarning("ID do recurso '{ResourceId}' não é um número válido", resourceIdValue);
                context.Fail();
                return Task.CompletedTask;
            }

            // TODO: Implementar verificação específica por tipo de recurso
            // Por enquanto, verificamos se o usuário é o proprietário comparando IDs
            // Em implementações futuras, isso será feito via repositórios específicos
            if (IsUserOwnerOfResource(userId, resourceId, requirement.ResourceType))
            {
                _logger.LogInformation("Usuário {UserId} tem propriedade sobre recurso {ResourceId}", userId, resourceId);
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning("Usuário {UserId} não tem propriedade sobre recurso {ResourceId}", userId, resourceId);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante verificação de propriedade");
            context.Fail();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se o usuário é proprietário do recurso
    /// TODO: Implementar verificação específica por tipo de recurso usando repositórios
    /// </summary>
    private bool IsUserOwnerOfResource(int userId, int resourceId, string? resourceType)
    {
        // IMPLEMENTAÇÃO TEMPORÁRIA: Para usuários, o próprio ID é o recurso
        // Em implementações futuras, isso será feito via repositórios específicos:
        // - Para Clips: verificar se clip.UserId == userId
        // - Para Channels: verificar se channel.OwnerId == userId
        // - Para Videos: verificar se video.UploadedBy == userId
        
        switch (resourceType?.ToLowerInvariant())
        {
            case "user":
                return userId == resourceId;
            
            case "clip":
            case "channel":
            case "video":
            default:
                // Por enquanto, implementação básica
                // TODO: Integrar com repositórios específicos
                _logger.LogInformation("Verificação de propriedade básica para tipo '{ResourceType}'", resourceType);
                return userId == resourceId;
        }
    }
}
