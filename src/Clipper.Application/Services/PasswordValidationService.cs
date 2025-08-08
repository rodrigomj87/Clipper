/**
 * @file: PasswordValidationService.cs
 * @responsibility: Serviço para validação de complexidade de senha
 * @exports: PasswordValidationService
 * @imports: PasswordRules, ValidationResult, IPasswordValidationService
 * @layer: Services
 */
using FluentValidation.Results;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Common.ValidationRules;

namespace Clipper.Application.Services;

public class PasswordValidationService : IPasswordValidationService
{
    public ValidationResult ValidatePassword(string password)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(password))
        {
            result.Errors.Add(new ValidationFailure("Password", "Senha não pode ser vazia."));
            return result;
        }

        if (password.Length < PasswordRules.MinLength)
            result.Errors.Add(new ValidationFailure("Password", $"Senha deve ter no mínimo {PasswordRules.MinLength} caracteres."));
        if (password.Length > PasswordRules.MaxLength)
            result.Errors.Add(new ValidationFailure("Password", $"Senha deve ter no máximo {PasswordRules.MaxLength} caracteres."));
        if (!PasswordRules.HasUpperCase.IsMatch(password))
            result.Errors.Add(new ValidationFailure("Password", "Senha deve conter pelo menos uma letra maiúscula."));
        if (!PasswordRules.HasLowerCase.IsMatch(password))
            result.Errors.Add(new ValidationFailure("Password", "Senha deve conter pelo menos uma letra minúscula."));
        if (!PasswordRules.HasNumber.IsMatch(password))
            result.Errors.Add(new ValidationFailure("Password", "Senha deve conter pelo menos um número."));
        if (!PasswordRules.HasSpecialChar.IsMatch(password))
            result.Errors.Add(new ValidationFailure("Password", "Senha deve conter pelo menos um caractere especial."));
        if (PasswordRules.CommonPasswords.Any(p => p.Equals(password, StringComparison.OrdinalIgnoreCase)))
            result.Errors.Add(new ValidationFailure("Password", "Senha muito comum ou insegura."));

        return result;
    }

    public bool IsPasswordComplex(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < PasswordRules.MinLength || password.Length > PasswordRules.MaxLength) return false;
        if (!PasswordRules.HasUpperCase.IsMatch(password)) return false;
        if (!PasswordRules.HasLowerCase.IsMatch(password)) return false;
        if (!PasswordRules.HasNumber.IsMatch(password)) return false;
        if (!PasswordRules.HasSpecialChar.IsMatch(password)) return false;
        if (PasswordRules.CommonPasswords.Any(p => p.Equals(password, StringComparison.OrdinalIgnoreCase))) return false;
        return true;
    }
}
