using FluentValidation;

namespace Clipper.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Validador para comando de login
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email deve ter formato válido")
            .MaximumLength(200)
            .WithMessage("Email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Senha é obrigatória")
            .MinimumLength(6)
            .WithMessage("Senha deve ter no mínimo 6 caracteres")
            .MaximumLength(100)
            .WithMessage("Senha deve ter no máximo 100 caracteres");
    }
}
