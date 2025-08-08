using Clipper.Application.Features.Authentication.Common;

namespace Clipper.Application.Common.Interfaces;

/// <summary>
/// Interface para serviços de autenticação
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica um usuário com email e senha
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Resposta de autenticação com token e dados do usuário</returns>
    Task<AuthResponse> LoginAsync(string email, string password);

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <param name="name">Nome do usuário</param>
    /// <returns>Resposta de autenticação com token e dados do usuário</returns>
    Task<AuthResponse> RegisterAsync(string email, string password, string name);

    /// <summary>
    /// Renova um token JWT usando refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token válido</param>
    /// <returns>Nova resposta de autenticação com tokens renovados</returns>
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Realiza logout invalidando o refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token a invalidar</param>
    /// <returns>True se logout foi realizado com sucesso</returns>
    Task<bool> LogoutAsync(string refreshToken);
}
