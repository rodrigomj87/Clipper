using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Clipper.API.Middleware
{
    /// <summary>
    /// Middleware para rate limiting customizado por endpoint e global
    /// </summary>
public class RateLimitingMiddleware : Clipper.API.Attributes.IRateLimitingService
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly RateLimitSettings _settings;

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache, IOptions<RateLimitSettings> settings)
        {
            _next = next;
            _cache = cache;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            var endpoint = context.Request.Path.ToString().ToLowerInvariant();
            var key = $"rate_limit_{clientId}_{endpoint}";

            var limits = _settings.EndpointLimits.ContainsKey(endpoint)
                ? _settings.EndpointLimits[endpoint]
                : _settings.GlobalLimit;

            if (IsRateLimitExceeded(key, limits))
            {
                context.Response.StatusCode = 429;
                context.Response.Headers["X-RateLimit-Limit"] = limits.RequestsPerMinute.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = "0";
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            IncrementRequestCount(key, limits);
            await _next(context);
        }

        // Implementação para uso no attribute
        public Task<bool> IsRateLimitExceeded(string clientId, int perMinute, int perHour)
        {
            var key = $"rate_limit_{clientId}_custom";
            var count = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });
            if (count >= perMinute)
                return Task.FromResult(true);
            _cache.Set(key, count + 1, TimeSpan.FromMinutes(1));
            return Task.FromResult(false);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userId = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "anonymous";
            return $"{ip}_{userId}";
        }

        private bool IsRateLimitExceeded(string key, IRateLimit limits)
        {
            var now = DateTime.UtcNow;
            var count = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });
            return count >= limits.RequestsPerMinute;
        }

        private void IncrementRequestCount(string key, IRateLimit limits)
        {
            var count = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return 0;
            });
            _cache.Set(key, count + 1, TimeSpan.FromMinutes(1));
        }
    }

    public interface IRateLimit
    {
        int RequestsPerMinute { get; set; }
    }
}
