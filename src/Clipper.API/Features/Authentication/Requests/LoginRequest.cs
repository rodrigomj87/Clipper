namespace Clipper.API.Features.Authentication.Requests;

/// <summary>
/// Request para login de usuário
/// </summary>
public record LoginRequest(
    string Email, 
    string Password
);
