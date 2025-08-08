using Microsoft.Extensions.Logging;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Features.Authentication.Common;
using Microsoft.EntityFrameworkCore;
using Clipper.Domain.Entities;
using Clipper.Common.Exceptions;
using BCrypt.Net;

namespace Clipper.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de autenticação
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuário com email e senha
    /// </summary>
    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Tentativa de login para email: {Email}", email);

            // Buscar usuário por email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Usuário não encontrado para email: {Email}", email);
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            // Verificar se usuário está ativo
            if (!user.IsActive)
            {
                _logger.LogWarning("Tentativa de login de usuário inativo: {Email}", email);
                throw new UnauthorizedAccessException("Conta desativada");
            }

            // Verificar senha
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Senha incorreta para usuário: {Email}", email);
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            // Atualizar último login
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Gerar tokens
            var token = _jwtTokenService.GenerateToken(user);
            var refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(user.Id);

            var userDto = new UserDto(
                user.Id,
                user.Email,
                user.Name,
                user.CreatedAt
            );

            _logger.LogInformation("Login realizado com sucesso para usuário: {Email}", email);

            return new AuthResponse(
                token,
                refreshToken,
                DateTime.UtcNow.AddMinutes(60), // Expiração baseada nas configurações JWT
                userDto
            );
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para email: {Email}", email);
            throw new Exception("Erro interno durante autenticação");
        }
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(string email, string password, string name)
    {
        try
        {
            _logger.LogInformation("Tentativa de registro para email: {Email}", email);

            // Verificação explícita de email existente
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                _logger.LogWarning("Tentativa de registro com email já existente: {Email}", email);
                throw new ConflictException("Email já cadastrado no sistema");
            }

            // Criar hash da senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

            // Criar usuário
            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            User createdUser;
            try
            {
                createdUser = await _userRepository.CreateAsync(user);
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                // SQL Server, SQLite, PostgreSQL, MySQL: verifica padrões conhecidos
                if (msg.Contains("UNIQUE") || msg.Contains("duplicate") || msg.Contains("IX_Users_Email") || msg.Contains("Violation of UNIQUE KEY constraint") || msg.Contains("UNIQUE constraint failed") || msg.Contains("Duplicate entry"))
                {
                    _logger.LogWarning("Tentativa de registro com email já existente (via exceção): {Email}", email);
                    throw new ConflictException("Email já cadastrado no sistema");
                }
                _logger.LogError(dbEx, "Erro de banco durante registro para email: {Email}", email);
                throw new Exception("Erro interno durante registro");
            }

            // Gerar tokens
            var token = _jwtTokenService.GenerateToken(createdUser);
            var refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(createdUser.Id);

            var userDto = new UserDto(
                createdUser.Id,
                createdUser.Email,
                createdUser.Name,
                createdUser.CreatedAt
            );

            _logger.LogInformation("Usuário registrado com sucesso: {Email}", email);

            return new AuthResponse(
                token,
                refreshToken,
                DateTime.UtcNow.AddMinutes(60), // Expiração baseada nas configurações JWT
                userDto
            );
        }
        catch (ConflictException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o registro para email: {Email}", email);
            throw new Exception("Erro interno durante registro");
        }
    }

    /// <summary>
    /// Renova um token JWT usando refresh token
    /// </summary>
    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation("Tentativa de refresh token");

            // Buscar o refresh token no banco
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || !storedToken.IsValid)
            {
                _logger.LogWarning("Refresh token inválido ou expirado");
                throw new UnauthorizedAccessException("Refresh token inválido");
            }

            // Buscar o usuário
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Usuário não encontrado ou inativo para refresh token");
                throw new UnauthorizedAccessException("Usuário inválido");
            }

            // Revogar o token atual
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(storedToken);

            // Gerar novos tokens
            var newToken = _jwtTokenService.GenerateToken(user);
            var newRefreshToken = await _jwtTokenService.CreateRefreshTokenAsync(user.Id);

            var userDto = new UserDto(
                user.Id,
                user.Email,
                user.Name,
                user.CreatedAt
            );

            _logger.LogInformation("Refresh token realizado com sucesso para usuário: {Email}", user.Email);

            return new AuthResponse(
                newToken,
                newRefreshToken,
                DateTime.UtcNow.AddMinutes(60), // Expiração baseada nas configurações JWT
                userDto
            );
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante refresh token");
            throw new UnauthorizedAccessException("Token de refresh inválido");
        }
    }

    /// <summary>
    /// Realiza logout invalidando o refresh token
    /// </summary>
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            _logger.LogInformation("Tentativa de logout");

            // Buscar o refresh token no banco
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken != null && !storedToken.IsRevoked)
            {
                // Revogar o token
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(storedToken);
            }

            _logger.LogInformation("Logout realizado com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return false;
        }
    }
}
