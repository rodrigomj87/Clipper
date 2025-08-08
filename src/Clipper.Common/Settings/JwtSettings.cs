namespace Clipper.Common.Settings;

/// <summary>
/// Configurações para autenticação JWT
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Chave secreta para assinatura do token JWT.
    /// IMPORTANTE: Esta chave NUNCA deve ser hardcoded no código.
    /// Deve ser configurada via User Secrets (desenvolvimento) ou variáveis de ambiente (produção).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emissor do token JWT
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Público-alvo do token JWT
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
    /// Valida se todas as configurações obrigatórias estão preenchidas
    /// </summary>
    public bool IsValid => 
        !string.IsNullOrWhiteSpace(SecretKey) &&
        !string.IsNullOrWhiteSpace(Issuer) &&
        !string.IsNullOrWhiteSpace(Audience) &&
        ExpirationInMinutes > 0 &&
        RefreshTokenExpirationInDays > 0;

    /// <summary>
    /// Valida se a chave secreta tem o tamanho mínimo recomendado (256 bits = 32 bytes)
    /// </summary>
    public bool HasValidSecretKeyLength => 
        !string.IsNullOrWhiteSpace(SecretKey) && 
        System.Text.Encoding.UTF8.GetBytes(SecretKey).Length >= 32;
}
