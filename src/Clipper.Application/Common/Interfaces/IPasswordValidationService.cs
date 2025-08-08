/**
 * @file: IPasswordValidationService.cs
 * @responsibility: Contrato para validação de complexidade de senha
 * @exports: IPasswordValidationService
 * @imports: ValidationResult
 * @layer: Common/Interfaces
 */
using FluentValidation.Results;

namespace Clipper.Application.Common.Interfaces;

public interface IPasswordValidationService
{
    ValidationResult ValidatePassword(string password);
    bool IsPasswordComplex(string password);
}
