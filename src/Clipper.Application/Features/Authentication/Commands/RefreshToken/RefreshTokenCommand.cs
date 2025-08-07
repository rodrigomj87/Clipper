using MediatR;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Comando para renovação de token JWT
/// </summary>
public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<AuthResponse>;
