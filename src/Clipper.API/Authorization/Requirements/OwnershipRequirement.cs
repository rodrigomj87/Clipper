using Microsoft.AspNetCore.Authorization;

namespace Clipper.API.Authorization.Requirements;

/// <summary>
/// Requirement para verificação de propriedade de recursos
/// </summary>
public class OwnershipRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Chave do parâmetro de rota que contém o ID do recurso
    /// </summary>
    public string ResourceIdRouteKey { get; }

    /// <summary>
    /// Tipo de recurso para verificação de propriedade
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// Inicializa um novo requirement de propriedade
    /// </summary>
    /// <param name="resourceIdRouteKey">Chave do parâmetro de rota (padrão: "id")</param>
    /// <param name="resourceType">Tipo do recurso (opcional)</param>
    public OwnershipRequirement(string resourceIdRouteKey = "id", string? resourceType = null)
    {
        ResourceIdRouteKey = resourceIdRouteKey;
        ResourceType = resourceType;
    }
}
