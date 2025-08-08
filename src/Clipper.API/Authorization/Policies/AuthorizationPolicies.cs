using Microsoft.AspNetCore.Authorization;

namespace Clipper.API.Authorization.Policies;

/// <summary>
/// Definições de políticas de autorização customizadas
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Policy para usuários autenticados
    /// </summary>
    public const string RequireUser = "RequireUser";
    
    /// <summary>
    /// Policy para administradores
    /// </summary>
    public const string RequireAdmin = "RequireAdmin";
    
    /// <summary>
    /// Policy para verificação de propriedade de recursos
    /// </summary>
    public const string RequireOwnership = "RequireOwnership";
    
    /// <summary>
    /// Policy para usuários ou administradores
    /// </summary>
    public const string RequireUserOrAdmin = "RequireUserOrAdmin";

    /// <summary>
    /// Configura todas as políticas de autorização
    /// </summary>
    /// <param name="options">Opções de autorização</param>
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Policy básica: usuário autenticado
        options.AddPolicy(RequireUser, policy =>
            policy.RequireAuthenticatedUser());

        // Policy para administradores
        options.AddPolicy(RequireAdmin, policy =>
            policy.RequireRole("Admin"));

        // Policy para usuários ou admins
        options.AddPolicy(RequireUserOrAdmin, policy =>
            policy.RequireRole("User", "Admin"));

        // Policy para propriedade de recursos
        options.AddPolicy(RequireOwnership, policy =>
            policy.Requirements.Add(new Requirements.OwnershipRequirement()));

        // Policy padrão
        options.DefaultPolicy = options.GetPolicy(RequireUser)!;
    }
}
