using System.Collections.Generic;

namespace Clipper.Application.Common.Settings
{
    /// <summary>
    /// Configurações de rate limit por endpoint e global
    /// </summary>
    public class RateLimitSettings
    {
        public Dictionary<string, EndpointRateLimit> EndpointLimits { get; set; } = new();
        public GlobalRateLimit GlobalLimit { get; set; } = new();
    }

    public class EndpointRateLimit : IRateLimit
    {
        public int RequestsPerMinute { get; set; }
        public int RequestsPerHour { get; set; }
        public int RequestsPerDay { get; set; }
        public int BurstSize { get; set; }
    }

    public class GlobalRateLimit : IRateLimit
    {
        public int RequestsPerMinute { get; set; } = 60;
        public int RequestsPerHour { get; set; } = 1000;
    }

    public interface IRateLimit
    {
        int RequestsPerMinute { get; set; }
    }
}
