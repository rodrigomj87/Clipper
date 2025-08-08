using FluentValidation;

namespace Clipper.Application.Features.Authentication.Commands.Logout;

/// <summary>
/// Validador para comando de logout
/// </summary>
public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token é obrigatório para logout")
            .MinimumLength(10)
            .WithMessage("Refresh token inválido");
    }
}
