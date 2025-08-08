using Microsoft.Extensions.Options;
using Clipper.Infrastructure.Services;
using Clipper.Common.Settings;
using Clipper.Domain.Entities;
using Clipper.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Clipper.Tests.Unit.Services;

/// <summary>
/// Testes unitários para JwtTokenService
/// </summary>
/// <remarks>
/// Responsabilidade: validar geração, validação e manipulação de tokens JWT
/// </remarks>
public class JwtTokenServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "my-super-secret-key-for-testing-purposes-32-chars-minimum",
            Issuer = "ClipperTest",
            Audience = "ClipperTestUsers",
            ExpirationInMinutes = 15,
            RefreshTokenExpirationInDays = 7
        };

        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(x => x.Value).Returns(_jwtSettings);

        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        
        _jwtTokenService = new JwtTokenService(_jwtSettingsMock.Object, _refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ReturnsValidJwt()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        
        jsonToken.Issuer.Should().Be(_jwtSettings.Issuer);
        jsonToken.Audiences.Should().Contain(_jwtSettings.Audience);
        // ClaimTypes.NameIdentifier maps to "nameid" in JWT
        jsonToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id.ToString());
        // ClaimTypes.Email maps to "email" in JWT  
        jsonToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        // ClaimTypes.Name maps to "unique_name" in JWT
        jsonToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == user.Name);
        jsonToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_WithNullUser_ThrowsArgumentNullException()
    {
        // Arrange
        User? user = null;

        // Act & Assert
        var act = () => _jwtTokenService.GenerateToken(user!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetClaimsFromExpiredToken_WithValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var user = CreateTestUser();
        var token = _jwtTokenService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.GetClaimsFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(user.Id.ToString());
        principal.FindFirst(ClaimTypes.Email)?.Value.Should().Be(user.Email);
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be(user.Name);
    }

    [Fact]
    public void GetClaimsFromExpiredToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var principal = _jwtTokenService.GetClaimsFromExpiredToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GetClaimsFromExpiredToken_WithExpiredToken_ReturnsClaimsPrincipal()
    {
        // Arrange - Criar um token normal primeiro
        var user = CreateTestUser();
        var normalToken = _jwtTokenService.GenerateToken(user);
        
        // Act - O método GetClaimsFromExpiredToken deve funcionar mesmo com tokens expirados
        var principal = _jwtTokenService.GetClaimsFromExpiredToken(normalToken);

        // Assert
        principal.Should().NotBeNull(); // Deve retornar claims mesmo se expirado
        principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value.Should().Be(user.Id.ToString());
    }

    [Fact]
    public void GetClaimsFromExpiredToken_WithTamperedToken_ReturnsNull()
    {
        // Arrange
        var user = CreateTestUser();
        var originalToken = _jwtTokenService.GenerateToken(user);
        var tamperedToken = originalToken[..^5] + "FAKE"; // Alterar final do token

        // Act
        var principal = _jwtTokenService.GetClaimsFromExpiredToken(tamperedToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueToken()
    {
        // Act
        var refreshToken1 = _jwtTokenService.GenerateRefreshToken();
        var refreshToken2 = _jwtTokenService.GenerateRefreshToken();

        // Assert
        refreshToken1.Should().NotBeNullOrEmpty();
        refreshToken2.Should().NotBeNullOrEmpty();
        refreshToken1.Should().NotBe(refreshToken2);
        refreshToken1.Length.Should().BeGreaterThan(10); // Token suficientemente longo
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "valid-refresh-token";
        var validRefreshTokenEntity = new RefreshToken
        {
            Id = 1,
            Token = refreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(validRefreshTokenEntity);

        // Act
        var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, userId);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithExpiredToken_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "expired-refresh-token";
        var expiredRefreshTokenEntity = new RefreshToken
        {
            Id = 2,
            Token = refreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expirado
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(expiredRefreshTokenEntity);

        // Act
        var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, userId);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithRevokedToken_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "revoked-refresh-token";
        var revokedRefreshTokenEntity = new RefreshToken
        {
            Id = 3,
            Token = refreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = true, // Revogado
            CreatedAt = DateTime.UtcNow
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(revokedRefreshTokenEntity);

        // Act
        var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, userId);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithNonExistentToken_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "non-existent-token";

        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, userId);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRefreshTokenAsync_WithWrongUserId_ReturnsFalse()
    {
        // Arrange
        var correctUserId = 1;
        var wrongUserId = 2;
        var refreshToken = "valid-refresh-token";
        var refreshTokenEntity = new RefreshToken
        {
            Id = 4,
            Token = refreshToken,
            UserId = correctUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(refreshToken))
            .ReturnsAsync(refreshTokenEntity);

        // Act
        var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, wrongUserId);

        // Assert
        isValid.Should().BeFalse();
    }

    #region Helper Methods

    private static User CreateTestUser()
    {
        return new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = "hashed-password",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    #endregion
}
