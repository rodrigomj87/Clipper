using System.Security.Claims;
using Clipper.Domain.Entities;

namespace Clipper.Application.Common.Interfaces;

/// <summary>
/// Interface para serviços de geração e validação de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    /// <param name="user">Usuário para gerar o token</param>
    /// <returns>Token JWT assinado</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Gera um refresh token aleatório
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extrai claims de um token JWT expirado
    /// </summary>
    /// <param name="token">Token JWT expirado</param>
    /// <returns>ClaimsPrincipal ou null se inválido</returns>
    ClaimsPrincipal? GetClaimsFromExpiredToken(string token);

    /// <summary>
    /// Cria e armazena um refresh token para o usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Refresh token gerado</returns>
    Task<string> CreateRefreshTokenAsync(int userId);

    /// <summary>
    /// Valida se um refresh token é válido para o usuário
    /// </summary>
    /// <param name="refreshToken">Refresh token a validar</param>
    /// <param name="userId">ID do usuário</param>
    /// <returns>True se válido, false caso contrário</returns>
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId);
}
