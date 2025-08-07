namespace Clipper.Application.Features.Authentication.Common;

/// <summary>
/// DTO de usuário para resposta de autenticação
/// </summary>
public record UserDto(
    int Id,
    string Email,
    string Name,
    DateTime CreatedAt
);
