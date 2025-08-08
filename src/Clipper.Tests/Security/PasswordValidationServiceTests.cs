/**
 * @file: PasswordValidationServiceTests.cs
 * @responsibility: Testes unitários para validação de complexidade de senha
 * @exports: PasswordValidationServiceTests
 * @imports: PasswordValidationService, PasswordRules
 * @layer: Tests/Security
 */
using Xunit;
using Clipper.Application.Services;
using Clipper.Application.Common.ValidationRules;

namespace Clipper.Tests.Security;

public class PasswordValidationServiceTests
{
    private readonly PasswordValidationService _service = new();

    [Theory]
    [InlineData("123456", false)]
    [InlineData("password", false)]
    [InlineData("Senha123", false)]
    [InlineData("Abcdef1!", true)]
    [InlineData("A1!bcdefg", true)]
    [InlineData("A!1bcdefg", true)]
    [InlineData("A!1bcdefg@", true)]
    [InlineData("", false)]
    [InlineData("short1A!", true)]
    [InlineData("NoSpecialChar1A", false)]
    [InlineData("nouppercase1!", false)]
    [InlineData("NOLOWERCASE1!", false)]
    public void PasswordComplexity_WeakPasswords_AreRejected(string password, bool expected)
    {
        var result = _service.IsPasswordComplex(password);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PasswordValidation_ReturnsErrorsForWeakPassword()
    {
        var result = _service.ValidatePassword("123456");
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("comum"));
    }
}
