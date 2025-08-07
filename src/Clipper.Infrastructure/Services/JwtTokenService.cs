using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Clipper.Application.Common.Interfaces;
using Clipper.Common.Settings;
using Clipper.Domain.Entities;

namespace Clipper.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de tokens JWT
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings, IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _refreshTokenRepository = refreshTokenRepository;
    }

    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new("user_id", user.Id.ToString()),
            new("email", user.Email),
            new("name", user.Name)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gera um refresh token aleatório
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Extrai claims de um token JWT expirado
    /// </summary>
    public ClaimsPrincipal? GetClaimsFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Não validar expiração para permitir refresh
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            // Verificar se o token é realmente um JWT
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Cria e armazena um refresh token para o usuário
    /// </summary>
    public async Task<string> CreateRefreshTokenAsync(int userId)
    {
        var refreshTokenValue = GenerateRefreshToken();
        
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            IsRevoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        return refreshTokenValue;
    }

    /// <summary>
    /// Valida se um refresh token é válido para o usuário
    /// </summary>
    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        return token?.IsValid == true && token.UserId == userId;
    }
}
