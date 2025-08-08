/**
 * @file: PasswordComplexityAttribute.cs
 * @responsibility: Validação de complexidade de senha via DataAnnotation
 * @exports: PasswordComplexityAttribute
 * @imports: ValidationAttribute, PasswordValidationService
 * @layer: API/Attributes
 */
using System.ComponentModel.DataAnnotations;
using Clipper.Application.Services;

namespace Clipper.API.Attributes;

public class PasswordComplexityAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string password)
            return new ValidationResult("Senha inválida.");

        var passwordService = (PasswordValidationService)validationContext.GetService(typeof(PasswordValidationService));
        if (passwordService == null)
            return new ValidationResult("Serviço de validação de senha não disponível.");

        var result = passwordService.ValidatePassword(password);
        if (!result.IsValid)
            return new ValidationResult(string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

        return ValidationResult.Success!;
    }
}
