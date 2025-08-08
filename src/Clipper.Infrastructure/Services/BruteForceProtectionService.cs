using Clipper.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Clipper.Infrastructure.Services
{
    public class BruteForceProtectionService : IBruteForceProtectionService
    {
        private readonly IMemoryCache _cache;
        private readonly int _maxFailedAttempts = 5;
        private readonly int _lockoutDurationMinutes = 15;

        public BruteForceProtectionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsBlocked(string identifier)
        {
            var key = $"brute_force_{identifier}";
            if (_cache.TryGetValue(key, out BruteForceAttempt attempt))
            {
                if (attempt.IsBlocked && attempt.BlockedUntil > DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task RecordFailedAttempt(string identifier)
        {
            var key = $"brute_force_{identifier}";
            var attempt = _cache.Get<BruteForceAttempt>(key) ?? new BruteForceAttempt();
            attempt.FailedAttempts++;
            attempt.LastAttempt = DateTime.UtcNow;
            if (attempt.FailedAttempts >= _maxFailedAttempts)
            {
                attempt.IsBlocked = true;
                attempt.BlockedUntil = DateTime.UtcNow.AddMinutes(_lockoutDurationMinutes);
            }
            _cache.Set(key, attempt, TimeSpan.FromHours(1));
        }

        public async Task RecordSuccessfulAttempt(string identifier)
        {
            var key = $"brute_force_{identifier}";
            _cache.Remove(key);
        }

        public async Task<TimeSpan?> GetLockoutDuration(string identifier)
        {
            var key = $"brute_force_{identifier}";
            if (_cache.TryGetValue(key, out BruteForceAttempt attempt))
            {
                if (attempt.IsBlocked && attempt.BlockedUntil > DateTime.UtcNow)
                {
                    return attempt.BlockedUntil - DateTime.UtcNow;
                }
            }
            return null;
        }
    }

    public class BruteForceAttempt
    {
        public int FailedAttempts { get; set; }
        public DateTime LastAttempt { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime BlockedUntil { get; set; }
    }
}
