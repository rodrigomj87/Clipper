/**
 * @file: IXssProtectionService.cs
 * @responsibility: Contrato para proteção XSS e sanitização de input
 * @exports: IXssProtectionService
 * @imports: -
 * @layer: Common/Interfaces
 */
namespace Clipper.Application.Common.Interfaces;

public interface IXssProtectionService
{
    string SanitizeInput(string input);
    bool ContainsMaliciousContent(string input);
}
