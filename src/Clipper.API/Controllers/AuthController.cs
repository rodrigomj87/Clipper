using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Clipper.API.Features.Authentication.Requests;
using Clipper.Application.Features.Authentication.Commands.Login;
using Clipper.Application.Features.Authentication.Commands.Register;
using Clipper.Application.Features.Authentication.Commands.RefreshToken;
using Clipper.Application.Features.Authentication.Commands.Logout;
using Clipper.Application.Features.Authentication.Common;

namespace Clipper.API.Controllers;

/// <summary>
/// Controller responsável pela autenticação de usuários
/// </summary>
[ApiController]
[Route("api/auth")]
[Tags("Authentication")]
public class AuthController : ApiControllerBase
{
    /// <summary>
    /// Autentica um usuário no sistema
    /// </summary>
    /// <param name="request">Dados de login (email e senha)</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Login de usuário",
        Description = "Autentica um usuário com email e senha, retornando JWT token e refresh token"
    )]
    [SwaggerResponse(200, "Login realizado com sucesso", typeof(AuthResponse))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(401, "Email ou senha incorretos")]
    [SwaggerResponse(422, "Erro de validação")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Falha na autenticação",
                Detail = "Email ou senha incorretos"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro durante o login",
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados do novo usuário</param>
    /// <returns>Token JWT e informações do usuário criado</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Registro de usuário",
        Description = "Cria uma nova conta de usuário no sistema"
    )]
    [SwaggerResponse(201, "Usuário criado com sucesso", typeof(AuthResponse))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(409, "Email já está em uso")]
    [SwaggerResponse(422, "Erro de validação")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var command = new RegisterCommand(request.Email, request.Password, request.ConfirmPassword, request.Name);
            var result = await Mediator.Send(command);
            return CreatedAtAction(nameof(Register), result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("já existe"))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Email já está em uso",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro durante o registro",
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Renova o token JWT usando refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Novos tokens JWT e refresh token</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Renovação de token",
        Description = "Gera um novo JWT token usando um refresh token válido"
    )]
    [SwaggerResponse(200, "Token renovado com sucesso", typeof(AuthResponse))]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(401, "Refresh token inválido ou expirado")]
    [SwaggerResponse(422, "Erro de validação")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Refresh token inválido",
                Detail = "O refresh token fornecido é inválido ou expirado"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro durante renovação do token",
                Detail = ex.Message
            });
        }
    }

    /// <summary>
    /// Realiza logout do usuário invalidando o refresh token
    /// </summary>
    /// <param name="request">Refresh token a ser invalidado</param>
    /// <returns>Confirmação do logout</returns>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Logout do usuário",
        Description = "Invalida o refresh token, realizando logout seguro do usuário"
    )]
    [SwaggerResponse(200, "Logout realizado com sucesso")]
    [SwaggerResponse(400, "Dados de entrada inválidos")]
    [SwaggerResponse(401, "Token de autorização inválido")]
    [SwaggerResponse(422, "Erro de validação")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
    {
        try
        {
            var command = new LogoutCommand(request.RefreshToken);
            var success = await Mediator.Send(command);
            
            if (success)
            {
                return Ok(new { message = "Logout realizado com sucesso" });
            }
            
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Falha no logout",
                Detail = "Não foi possível realizar o logout"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro durante o logout",
                Detail = ex.Message
            });
        }
    }
}
