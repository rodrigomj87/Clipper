using Clipper.Application.Features.Authentication.Commands.Login;
using Clipper.Application.Features.Authentication.Commands.Register;
using Clipper.API.Features.Authentication.Requests;

namespace Clipper.API.Features.Authentication.Extensions;

/// <summary>
/// Extensões para mapeamento de DTOs de autenticação
/// </summary>
public static class AuthenticationMappingExtensions
{
    /// <summary>
    /// Converte LoginRequest para LoginCommand
    /// </summary>
    public static LoginCommand ToCommand(this LoginRequest request)
    {
        return new LoginCommand(request.Email, request.Password);
    }

    /// <summary>
    /// Converte RegisterRequest para RegisterCommand
    /// </summary>
    public static RegisterCommand ToCommand(this RegisterRequest request)
    {
        return new RegisterCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.Name
        );
    }
}
