using MediatR;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Handler para comando de login de usuário
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executa o comando de login
    /// </summary>
    /// <param name="request">Comando de login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resposta de autenticação</returns>
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request.Email, request.Password);
    }
}
