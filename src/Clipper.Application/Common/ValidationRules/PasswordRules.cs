/**
 * @file: PasswordRules.cs
 * @responsibility: Regras e regex para validação de complexidade de senha
 * @exports: PasswordRules
 * @imports: Regex
 * @layer: Common/ValidationRules
 */
using System.Text.RegularExpressions;

namespace Clipper.Application.Common.ValidationRules;

public static class PasswordRules
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    public static readonly Regex HasUpperCase = new(@"[A-Z]");
    public static readonly Regex HasLowerCase = new(@"[a-z]");
    public static readonly Regex HasNumber = new(@"\d");
    public static readonly Regex HasSpecialChar = new(@"[!@#$%^&*(),.?\"":{}|<>]");

    public static readonly string[] CommonPasswords =
    {
        "123456", "password", "123456789", "12345", "12345678", "qwerty", "abc123", "111111", "123123", "senha", "admin"
        // Adicionar mais conforme necessário
    };
}
