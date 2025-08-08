using Microsoft.AspNetCore.Authorization;
using Clipper.API.Authorization.Policies;

namespace Clipper.API.Authorization.Attributes;

/// <summary>
/// Atributo para exigir propriedade sobre um recurso
/// </summary>
public class RequireOwnershipAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Inicializa o atributo de propriedade
    /// </summary>
    /// <param name="resourceIdRouteKey">Chave do parâmetro de rota (padrão: "id")</param>
    /// <param name="resourceType">Tipo do recurso (opcional)</param>
    public RequireOwnershipAttribute(string resourceIdRouteKey = "id", string? resourceType = null)
    {
        Policy = AuthorizationPolicies.RequireOwnership;
        
        // Armazenar parâmetros customizados para uso posterior
        // Nota: AuthorizeAttribute não suporta parâmetros customizados nativamente
        // Por isso criamos uma versão mais avançada se necessário
    }
}

/// <summary>
/// Atributo avançado para propriedade com parâmetros customizados
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireOwnershipAdvancedAttribute : Attribute, IAuthorizeData
{
    /// <summary>
    /// Chave do parâmetro de rota que contém o ID do recurso
    /// </summary>
    public string ResourceIdRouteKey { get; }

    /// <summary>
    /// Tipo do recurso para verificação
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// Policy a ser utilizada
    /// </summary>
    public string? Policy { get; set; }

    /// <summary>
    /// Roles permitidas
    /// </summary>
    public string? Roles { get; set; }

    /// <summary>
    /// Esquemas de autenticação
    /// </summary>
    public string? AuthenticationSchemes { get; set; }

    /// <summary>
    /// Inicializa o atributo avançado de propriedade
    /// </summary>
    /// <param name="resourceIdRouteKey">Chave do parâmetro de rota</param>
    /// <param name="resourceType">Tipo do recurso</param>
    public RequireOwnershipAdvancedAttribute(string resourceIdRouteKey = "id", string? resourceType = null)
    {
        ResourceIdRouteKey = resourceIdRouteKey;
        ResourceType = resourceType;
        Policy = AuthorizationPolicies.RequireOwnership;
    }
}
