using Microsoft.EntityFrameworkCore;
using Clipper.Application.Common.Interfaces;
using Clipper.Domain.Entities;
using Clipper.Infrastructure.Data;

namespace Clipper.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de refresh tokens
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ClipperDbContext _context;

    public RefreshTokenRepository(ClipperDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca um refresh token pelo valor do token
    /// </summary>
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsDeleted);
    }

    /// <summary>
    /// Busca todos os refresh tokens válidos de um usuário
    /// </summary>
    public async Task<IEnumerable<RefreshToken>> GetValidTokensByUserIdAsync(int userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && 
                        !rt.IsRevoked && 
                        rt.ExpiresAt > DateTime.UtcNow && 
                        !rt.IsDeleted)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Cria um novo refresh token
    /// </summary>
    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    /// <summary>
    /// Atualiza um refresh token existente
    /// </summary>
    public async Task<RefreshToken> UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    /// <summary>
    /// Revoga todos os refresh tokens de um usuário
    /// </summary>
    public async Task<int> RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsDeleted)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove tokens expirados do banco de dados
    /// </summary>
    public async Task<int> CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        return await _context.SaveChangesAsync();
    }
}
