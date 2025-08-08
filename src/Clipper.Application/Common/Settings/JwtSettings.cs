namespace Clipper.Application.Common.Settings;

/// <summary>
/// Configurações do JWT para autenticação
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Chave secreta para assinatura do JWT (mínimo 256 bits)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emissor do token (quem criou o token)
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiência do token (para quem o token é destinado)
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração do token em minutos
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Tempo de expiração do refresh token em dias
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;

    /// <summary>
    /// Valida se as configurações estão corretas
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SecretKey) &&
               SecretKey.Length >= 32 && // Mínimo 256 bits
               !string.IsNullOrWhiteSpace(Issuer) &&
               !string.IsNullOrWhiteSpace(Audience) &&
               ExpirationInMinutes > 0 &&
               RefreshTokenExpirationInDays > 0;
    }
}
