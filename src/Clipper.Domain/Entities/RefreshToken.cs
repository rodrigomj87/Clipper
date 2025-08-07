using Clipper.Domain.Common;

namespace Clipper.Domain.Entities;

/// <summary>
/// Representa um refresh token para autenticação
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Token string
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Data de expiração do token
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indica se o token foi revogado
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// Data em que o token foi revogado
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// ID do usuário proprietário do token
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Usuário proprietário do token
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Verifica se o token ainda é válido
    /// </summary>
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow && !IsDeleted;
}
