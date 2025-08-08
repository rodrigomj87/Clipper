/**
 * @file: CorsValidationService.cs
 * @responsibility: implementação da validação de CORS
 * @exports: CorsValidationService
 * @imports: ICorsValidationService, CorsSettings
 * @layer: Infrastructure/Services
 */
using FluentValidation.Results;
using Clipper.Application.Common.Interfaces;
using Clipper.Application.Common.Settings;

namespace Clipper.Infrastructure.Services
{
    public class CorsValidationService : ICorsValidationService
    {
        private readonly CorsSettings _corsSettings;

        public CorsValidationService(CorsSettings corsSettings)
        {
            _corsSettings = corsSettings;
        }

        public bool IsOriginAllowed(string origin)
        {
            return _corsSettings.AllowedOrigins.Contains(origin, System.StringComparer.OrdinalIgnoreCase);
        }

        public bool IsMethodAllowed(string method)
        {
            return _corsSettings.AllowedMethods.Contains(method, System.StringComparer.OrdinalIgnoreCase);
        }

        public ValidationResult ValidateCorsRequest(string origin, string method)
        {
            var result = new ValidationResult();
            if (!IsOriginAllowed(origin))
                result.Errors.Add(new ValidationFailure("Origin", "Origin not allowed"));
            if (!IsMethodAllowed(method))
                result.Errors.Add(new ValidationFailure("Method", "Method not allowed"));
            return result;
        }
    }
}
