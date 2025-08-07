using FluentValidation;

namespace Clipper.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Validador para comando de refresh token
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token é obrigatório")
            .MinimumLength(10)
            .WithMessage("Refresh token inválido")
            .MaximumLength(500)
            .WithMessage("Refresh token muito longo");
    }
}
