using System.Security.Claims;

namespace Clipper.Common.Extensions;

/// <summary>
/// Extensões para trabalhar com Claims do usuário autenticado
/// </summary>
public static class ClaimsExtensions
{
    /// <summary>
    /// Obtém o ID do usuário a partir dos claims
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <returns>ID do usuário ou 0 se não encontrado</returns>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            // Tentar claims alternativos
            userIdClaim = principal.FindFirst("sub")?.Value ?? 
                         principal.FindFirst("userId")?.Value;
        }

        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Obtém o email do usuário a partir dos claims
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <returns>Email do usuário ou string vazia se não encontrado</returns>
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ??
               principal.FindFirst(ClaimTypes.Name)?.Value ??
               principal.FindFirst("email")?.Value ??
               string.Empty;
    }

    /// <summary>
    /// Obtém o nome do usuário a partir dos claims
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <returns>Nome do usuário ou string vazia se não encontrado</returns>
    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.GivenName)?.Value ??
               principal.FindFirst("name")?.Value ??
               principal.FindFirst(ClaimTypes.Name)?.Value ??
               string.Empty;
    }

    /// <summary>
    /// Verifica se o usuário possui uma role específica
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="role">Role a verificar</param>
    /// <returns>True se o usuário possui a role</returns>
    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return principal.HasClaim(ClaimTypes.Role, role) ||
               principal.IsInRole(role); // Método nativo do framework
    }

    /// <summary>
    /// Verifica se o usuário possui alguma das roles especificadas
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="roles">Roles a verificar</param>
    /// <returns>True se o usuário possui pelo menos uma das roles</returns>
    public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            return false;

        return roles.Any(role => principal.IsInRole(role));
    }

    /// <summary>
    /// Verifica se o usuário possui todas as roles especificadas
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="roles">Roles a verificar</param>
    /// <returns>True se o usuário possui todas as roles</returns>
    public static bool IsInAllRoles(this ClaimsPrincipal principal, params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            return false;

        return roles.All(role => principal.IsInRole(role));
    }

    /// <summary>
    /// Verifica se o usuário possui um claim específico
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="claimType">Tipo do claim</param>
    /// <param name="claimValue">Valor do claim</param>
    /// <returns>True se o usuário possui o claim</returns>
    public static bool HasClaim(this ClaimsPrincipal principal, string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            return false;

        return principal.HasClaim(claimType, claimValue);
    }

    /// <summary>
    /// Obtém o valor de um claim específico
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="claimType">Tipo do claim</param>
    /// <returns>Valor do claim ou string vazia se não encontrado</returns>
    public static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return string.Empty;

        return principal.FindFirst(claimType)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Obtém todos os valores de um tipo de claim
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <param name="claimType">Tipo do claim</param>
    /// <returns>Lista com todos os valores do claim</returns>
    public static IEnumerable<string> GetClaimValues(this ClaimsPrincipal principal, string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return Enumerable.Empty<string>();

        return principal.FindAll(claimType).Select(c => c.Value);
    }

    /// <summary>
    /// Verifica se o usuário é administrador
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <returns>True se o usuário é admin</returns>
    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("Admin") || principal.IsInRole("Administrator");
    }

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    /// <param name="principal">Principal do usuário</param>
    /// <returns>True se o usuário está autenticado</returns>
    public static bool IsAuthenticated(this ClaimsPrincipal principal)
    {
        return principal.Identity?.IsAuthenticated == true;
    }
}
