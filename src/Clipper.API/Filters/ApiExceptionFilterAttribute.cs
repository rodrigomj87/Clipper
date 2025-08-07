using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace Clipper.API.Filters;

/// <summary>
/// Filtro para tratamento de exceções da API com foco em autenticação
/// </summary>
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Erro não tratado na API: {Message}", context.Exception.Message);

        var problem = context.Exception switch
        {
            ValidationException validationEx => new ValidationProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Erro de validação",
                Detail = validationEx.Message
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Não autorizado",
                Detail = "Acesso negado. Verifique suas credenciais."
            },
            ArgumentException argEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Parâmetro inválido",
                Detail = argEx.Message
            },
            InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("já existe") => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflito",
                Detail = invalidOpEx.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro interno do servidor",
                Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde."
            }
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = problem.Status
        };

        context.ExceptionHandled = true;
    }
}
