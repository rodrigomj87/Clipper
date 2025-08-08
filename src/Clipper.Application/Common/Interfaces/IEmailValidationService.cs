using System.Threading.Tasks;

namespace Clipper.Application.Common.Interfaces;

/// <summary>
/// Interface para serviço de validação de email
/// </summary>
/// <remarks>
/// Responsabilidade: definir contratos para validação de emails e verificação de duplicação
/// </remarks>
public interface IEmailValidationService
{
    /// <summary>
    /// Verifica se o email é único no sistema
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>True se email é único, False se já existe</returns>
    Task<bool> IsEmailUniqueAsync(string email);

    /// <summary>
    /// Verifica se o email possui formato válido
    /// </summary>
    /// <param name="email">Email a ser validado</param>
    /// <returns>True se email tem formato válido</returns>
    Task<bool> IsEmailValidAsync(string email);
}
