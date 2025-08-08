/**
 * @file: SanitizeInputAttribute.cs
 * @responsibility: Atributo para sanitização e proteção XSS em inputs
 * @exports: SanitizeInputAttribute
 * @imports: ActionFilterAttribute, IXssProtectionService
 * @layer: API/Attributes
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Clipper.Application.Common.Interfaces;

namespace Clipper.API.Attributes;

public class SanitizeInputAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var xssProtection = context.HttpContext.RequestServices.GetService(typeof(IXssProtectionService)) as IXssProtectionService;
        if (xssProtection == null)
        {
            context.Result = new BadRequestObjectResult("Serviço de proteção XSS não disponível");
            return;
        }

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is string input)
            {
                if (xssProtection.ContainsMaliciousContent(input))
                {
                    context.Result = new BadRequestObjectResult("Malicious content detected");
                    return;
                }
            }
        }
        base.OnActionExecuting(context);
    }
}
