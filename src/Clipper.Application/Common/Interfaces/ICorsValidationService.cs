/**
 * @file: ICorsValidationService.cs
 * @responsibility: interface para validação de CORS
 * @exports: ICorsValidationService
 * @imports: CorsSettings
 * @layer: Common/Interfaces
 */
using FluentValidation.Results;
using Clipper.Application.Common.Settings;

namespace Clipper.Application.Common.Interfaces
{
    public interface ICorsValidationService
    {
        bool IsOriginAllowed(string origin);
        bool IsMethodAllowed(string method);
        ValidationResult ValidateCorsRequest(string origin, string method);
    }
}
