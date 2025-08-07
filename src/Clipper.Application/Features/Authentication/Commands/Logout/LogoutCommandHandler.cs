using MediatR;
using Clipper.Application.Common.Interfaces;

namespace Clipper.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Handler para comando de logout
/// </summary>
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executa o comando de logout
    /// </summary>
    /// <param name="request">Comando de logout</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se logout foi realizado com sucesso</returns>
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LogoutAsync(request.RefreshToken);
    }
}
