using Microsoft.AspNetCore.Authorization;

namespace Clipper.API.Authorization.Requirements;

/// <summary>
/// Requirement para verificação de roles específicas
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Roles requeridas para acesso
    /// </summary>
    public string[] RequiredRoles { get; }

    /// <summary>
    /// Se true, usuário deve ter TODAS as roles. Se false, basta ter UMA
    /// </summary>
    public bool RequireAllRoles { get; }

    /// <summary>
    /// Inicializa um novo requirement de role
    /// </summary>
    /// <param name="requiredRoles">Roles necessárias</param>
    /// <param name="requireAllRoles">Se deve ter todas as roles</param>
    public RoleRequirement(string[] requiredRoles, bool requireAllRoles = false)
    {
        RequiredRoles = requiredRoles ?? throw new ArgumentNullException(nameof(requiredRoles));
        RequireAllRoles = requireAllRoles;
    }

    /// <summary>
    /// Inicializa um novo requirement de role com uma única role
    /// </summary>
    /// <param name="requiredRole">Role necessária</param>
    public RoleRequirement(string requiredRole) : this(new[] { requiredRole })
    {
    }
}
