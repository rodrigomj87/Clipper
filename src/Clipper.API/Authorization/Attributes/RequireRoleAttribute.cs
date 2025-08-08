using Microsoft.AspNetCore.Authorization;

namespace Clipper.API.Authorization.Attributes;

/// <summary>
/// Atributo para exigir role específica
/// </summary>
public class RequireRoleAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Inicializa o atributo de role
    /// </summary>
    /// <param name="role">Role requerida</param>
    public RequireRoleAttribute(string role)
    {
        Roles = role;
    }

    /// <summary>
    /// Inicializa o atributo com múltiplas roles
    /// </summary>
    /// <param name="roles">Roles requeridas (separadas por vírgula)</param>
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}

/// <summary>
/// Atributo para exigir role de administrador
/// </summary>
public class RequireAdminAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Inicializa o atributo de admin
    /// </summary>
    public RequireAdminAttribute()
    {
        Roles = "Admin";
    }
}

/// <summary>
/// Atributo para exigir role de usuário ou admin
/// </summary>
public class RequireUserOrAdminAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Inicializa o atributo de usuário ou admin
    /// </summary>
    public RequireUserOrAdminAttribute()
    {
        Roles = "User,Admin";
    }
}
