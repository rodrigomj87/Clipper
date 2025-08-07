namespace Clipper.Application.Features.Authentication.Common;

/// <summary>
/// Resposta padrão para operações de autenticação
/// </summary>
public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);
