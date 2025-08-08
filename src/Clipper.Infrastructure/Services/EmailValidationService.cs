using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Clipper.Application.Common.Interfaces;

namespace Clipper.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de validação de email
/// </summary>
/// <remarks>
/// Responsabilidade: implementar lógica de validação de emails e verificação de duplicação
/// </remarks>
public class EmailValidationService : IEmailValidationService
{
    private readonly IUserRepository _userRepository;
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public EmailValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Verifica se o email é único no sistema
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>True se email é único, False se já existe</returns>
    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Retorna true se email NÃO existe (é único)
        return !(await _userRepository.EmailExistsAsync(email));
    }

    /// <summary>
    /// Verifica se o email possui formato válido
    /// </summary>
    /// <param name="email">Email a ser validado</param>
    /// <returns>True se email tem formato válido</returns>
    public async Task<bool> IsEmailValidAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Validação básica de formato usando regex
        return await Task.FromResult(EmailRegex.IsMatch(email));
    }
}
