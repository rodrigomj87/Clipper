using Clipper.Domain.Entities;

namespace Clipper.Application.Common.Interfaces;

/// <summary>
/// Interface para repositório de refresh tokens
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Busca um refresh token pelo valor do token
    /// </summary>
    /// <param name="token">Valor do token</param>
    /// <returns>RefreshToken encontrado ou null</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>
    /// Busca todos os refresh tokens válidos de um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Lista de refresh tokens válidos</returns>
    Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(int userId);

    /// <summary>
    /// Cria um novo refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token a criar</param>
    /// <returns>Refresh token criado</returns>
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);

    /// <summary>
    /// Atualiza um refresh token existente
    /// </summary>
    /// <param name="refreshToken">Refresh token a atualizar</param>
    /// <returns>Refresh token atualizado</returns>
    Task<RefreshToken> UpdateAsync(RefreshToken refreshToken);

    /// <summary>
    /// Revoga todos os refresh tokens de um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Número de tokens revogados</returns>
    Task<int> RevokeAllUserTokensAsync(int userId);

    /// <summary>
    /// Remove tokens expirados do banco de dados
    /// </summary>
    /// <returns>Número de tokens removidos</returns>
    Task<int> CleanupExpiredTokensAsync();
}
