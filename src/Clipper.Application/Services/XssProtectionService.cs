/**
 * @file: XssProtectionService.cs
 * @responsibility: Implementação de proteção XSS e sanitização de input
 * @exports: XssProtectionService
 * @imports: IXssProtectionService, HtmlEncoder, Regex
 * @layer: Services
 */
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Clipper.Application.Common.Interfaces;

namespace Clipper.Application.Services;

public class XssProtectionService : IXssProtectionService
{
    public string SanitizeInput(string input)
    {
        return HtmlEncoder.Default.Encode(input);
    }

    public bool ContainsMaliciousContent(string input)
    {
        var maliciousPatterns = new[]
        {
            "<script[^>]*>.*?</script>",
            "javascript:",
            "onload=",
            "onerror=",
            "expression\\("
        };
        return maliciousPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
    }
}
