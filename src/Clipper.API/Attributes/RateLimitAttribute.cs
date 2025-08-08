using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Clipper.API.Attributes
{
    /// <summary>
    /// Attribute para rate limiting customizado em endpoints
    /// </summary>
    public class RateLimitAttribute : ActionFilterAttribute
    {
        public int RequestsPerMinute { get; set; } = 60;
        public int RequestsPerHour { get; set; } = 1000;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rateLimitService = context.HttpContext.RequestServices.GetRequiredService<IRateLimitingService>();
            var clientId = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (await rateLimitService.IsRateLimitExceeded(clientId, RequestsPerMinute, RequestsPerHour))
            {
                context.Result = new StatusCodeResult(429);
                return;
            }
            await next();
        }
    }

    public interface IRateLimitingService
    {
        Task<bool> IsRateLimitExceeded(string clientId, int perMinute, int perHour);
    }
}
