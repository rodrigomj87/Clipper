using MediatR;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Comando para autenticação de usuário
/// </summary>
public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;
