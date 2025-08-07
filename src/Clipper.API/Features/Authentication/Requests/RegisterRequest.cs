namespace Clipper.API.Features.Authentication.Requests;

/// <summary>
/// Request para registro de usuário
/// </summary>
public record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string Name
);
