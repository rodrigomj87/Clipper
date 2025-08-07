namespace Clipper.Application.Features.Authentication.Common;

/// <summary>
/// DTO de usuário para resposta de autenticação
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string Name,
    DateTime CreatedAt
);
