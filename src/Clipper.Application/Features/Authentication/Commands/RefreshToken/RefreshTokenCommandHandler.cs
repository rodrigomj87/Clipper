using MediatR;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Handler para comando de refresh token
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Executa o comando de refresh token
    /// </summary>
    /// <param name="request">Comando de refresh token</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resposta de autenticação com tokens renovados</returns>
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshTokenAsync(request.RefreshToken);
    }
}
