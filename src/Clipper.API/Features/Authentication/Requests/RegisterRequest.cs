using System.ComponentModel.DataAnnotations;

namespace Clipper.API.Features.Authentication.Requests;

/// <summary>
/// Request para registro de usuário
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Confirmação da senha
    /// </summary>
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare(nameof(Password), ErrorMessage = "Senha e confirmação devem ser iguais")]
    public string ConfirmPassword { get; init; } = string.Empty;

    /// <summary>
    /// Nome do usuário
    /// </summary>
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
    [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Name { get; init; } = string.Empty;
}
