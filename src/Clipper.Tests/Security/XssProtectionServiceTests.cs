/**
 * @file: XssProtectionServiceTests.cs
 * @responsibility: Testes unitários para sanitização e detecção XSS
 * @exports: XssProtectionServiceTests
 * @imports: XssProtectionService
 * @layer: Tests/Security
 */
using Xunit;
using Clipper.Application.Services;

namespace Clipper.Tests.Security;

public class XssProtectionServiceTests
{
    private readonly XssProtectionService _service = new();

    [Theory]
    [InlineData("<script>alert('xss')</script>", true)]
    [InlineData("javascript:alert('xss')", true)]
    [InlineData("onload=alert('xss')", true)]
    [InlineData("onerror=alert('xss')", true)]
    [InlineData("expression(alert('xss'))", true)]
    [InlineData("normal text", false)]
    [InlineData("<b>bold</b>", false)]
    public void XssAttack_InUserInput_IsSanitized(string input, bool expected)
    {
        var result = _service.ContainsMaliciousContent(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>", "&lt;script&gt;alert(&#x27;xss&#x27;)&lt;/script&gt;")]
    [InlineData("normal text", "normal text")]
    public void SanitizeInput_EncodesHtml(string input, string expected)
    {
        var result = _service.SanitizeInput(input);
        Assert.Equal(expected, result);
    }
}
