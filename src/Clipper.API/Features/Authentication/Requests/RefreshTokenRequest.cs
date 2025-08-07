using System.ComponentModel.DataAnnotations;

namespace Clipper.API.Features.Authentication.Requests;

/// <summary>
/// Request para renovação de token via refresh token
/// </summary>
public record RefreshTokenRequest
{
    /// <summary>
    /// Refresh token usado para gerar um novo JWT
    /// </summary>
    [Required(ErrorMessage = "Refresh token é obrigatório")]
    public string RefreshToken { get; init; } = string.Empty;
}
