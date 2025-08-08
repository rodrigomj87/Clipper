/**
 * @file: SecurityHeadersMiddleware.cs
 * @responsibility: Middleware para adicionar headers de segurança HTTP
 * @exports: SecurityHeadersMiddleware
 * @imports: HttpContext, RequestDelegate
 * @layer: API/Middleware
 */
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Clipper.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        await _next(context);
    }
}
