using System.ComponentModel.DataAnnotations;

namespace Clipper.API.Features.Authentication.Requests;

/// <summary>
/// Request para logout do usuário
/// </summary>
public record LogoutRequest
{
    /// <summary>
    /// Refresh token a ser invalidado
    /// </summary>
    [Required(ErrorMessage = "Refresh token é obrigatório")]
    public string RefreshToken { get; init; } = string.Empty;
}
