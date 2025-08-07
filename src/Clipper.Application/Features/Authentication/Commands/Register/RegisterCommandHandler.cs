using MediatR;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Handler para comando de registro de usuário
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executa o comando de registro
    /// </summary>
    /// <param name="request">Comando de registro</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resposta de autenticação</returns>
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(request.Email, request.Password, request.Name);
    }
}
