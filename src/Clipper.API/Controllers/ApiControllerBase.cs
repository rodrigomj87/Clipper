using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Clipper.API.Controllers;

/// <summary>
/// Controller base com configurações comuns para todos os controllers da API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// Instância do MediatR para envio de commands e queries
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
