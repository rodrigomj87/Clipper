using MediatR;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Comando para registro de novo usuário
/// </summary>
public record RegisterCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string Name
) : IRequest<AuthResponse>;
