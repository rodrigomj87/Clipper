using MediatR;

namespace Clipper.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Comando para logout (invalidação de refresh token)
/// </summary>
public record LogoutCommand(
    string RefreshToken
) : IRequest<bool>;
