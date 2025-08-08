using System;
using System.Threading.Tasks;

namespace Clipper.Application.Common.Interfaces
{
    /// <summary>
    /// Interface para proteção contra brute force
    /// </summary>
    public interface IBruteForceProtectionService
    {
        Task<bool> IsBlocked(string identifier);
        Task RecordFailedAttempt(string identifier);
        Task RecordSuccessfulAttempt(string identifier);
        Task<TimeSpan?> GetLockoutDuration(string identifier);
    }
}
