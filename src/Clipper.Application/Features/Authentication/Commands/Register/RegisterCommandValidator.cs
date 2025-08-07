using FluentValidation;

namespace Clipper.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Validador para comando de registro
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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
            .MinimumLength(8)
            .WithMessage("Senha deve ter no mínimo 8 caracteres")
            .MaximumLength(100)
            .WithMessage("Senha deve ter no máximo 100 caracteres")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Senha deve conter pelo menos: 1 letra minúscula, 1 maiúscula e 1 número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Password)
            .WithMessage("Confirmação de senha deve ser igual à senha");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MinimumLength(2)
            .WithMessage("Nome deve ter no mínimo 2 caracteres")
            .MaximumLength(100)
            .WithMessage("Nome deve ter no máximo 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");
    }
}
