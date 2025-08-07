using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clipper.API.Authorization.Attributes;
using Clipper.Common.Extensions;

namespace Clipper.API.Controllers;

/// <summary>
/// Controller de teste para demonstrar o sistema de autorização
/// </summary>
[Tags("Test Authorization")]
public class TestAuthorizationController : ApiControllerBase
{
    /// <summary>
    /// Endpoint público - sem autenticação
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public ActionResult<object> PublicEndpoint()
    {
        return Ok(new { message = "Este endpoint é público", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Endpoint que requer usuário autenticado
    /// </summary>
    [HttpGet("user")]
    [RequireRole("User")]
    public ActionResult<object> UserEndpoint()
    {
        var userId = User.GetUserId();
        var userEmail = User.GetUserEmail();

        return Ok(new 
        { 
            message = "Você está autenticado como usuário", 
            userId = userId,
            userEmail = userEmail,
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Endpoint que requer administrador
    /// </summary>
    [HttpGet("admin")]
    [RequireAdmin]
    public ActionResult<object> AdminEndpoint()
    {
        var userId = User.GetUserId();
        var userEmail = User.GetUserEmail();

        return Ok(new 
        { 
            message = "Você é um administrador", 
            userId = userId,
            userEmail = userEmail,
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Endpoint que demonstra verificação de propriedade
    /// </summary>
    [HttpGet("user/{id}")]
    [RequireOwnership("id", "user")]
    public ActionResult<object> UserOwnershipEndpoint(int id)
    {
        var currentUserId = User.GetUserId();
        var userEmail = User.GetUserEmail();

        return Ok(new 
        { 
            message = "Você tem acesso aos seus próprios dados", 
            requestedUserId = id,
            currentUserId = currentUserId,
            userEmail = userEmail,
            isOwner = currentUserId == id,
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Endpoint que aceita usuário ou admin
    /// </summary>
    [HttpGet("user-or-admin")]
    [RequireUserOrAdmin]
    public ActionResult<object> UserOrAdminEndpoint()
    {
        var userId = User.GetUserId();
        var userEmail = User.GetUserEmail();
        var isAdmin = User.IsAdmin();
        var roles = User.GetClaimValues("role").ToList();

        return Ok(new 
        { 
            message = "Você é usuário ou administrador", 
            userId = userId,
            userEmail = userEmail,
            isAdmin = isAdmin,
            roles = roles,
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Endpoint para testar informações do usuário atual
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> MeEndpoint()
    {
        var userId = User.GetUserId();
        var userEmail = User.GetUserEmail();
        var userName = User.GetUserName();
        var isAuthenticated = User.IsAuthenticated();
        var isAdmin = User.IsAdmin();
        var roles = User.GetClaimValues("role").ToList();
        var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new 
        { 
            message = "Informações do usuário atual",
            userId = userId,
            userEmail = userEmail,
            userName = userName,
            isAuthenticated = isAuthenticated,
            isAdmin = isAdmin,
            roles = roles,
            claims = allClaims,
            timestamp = DateTime.UtcNow 
        });
    }
}
