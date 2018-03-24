using System;
using System.Threading.Tasks;

namespace Commonality
{
    /// <summary>
    /// Defines a platform-dependent service to access the time
    /// </summary>
    public interface IClock
    {
        /// <summary>
        /// Current time
        /// </summary>
        DateTime Now { get; set; }

        /// <summary>
        /// Wait for a certain amount of time
        /// </summary>
        /// <remarks>
        /// This is handy for testing, where the implementation can just return
        /// immediately, if needed.
        /// 
        /// It's also useful in situations where we are using a separate clock
        /// peripheral, and want to delay according to that clock.
        /// </remarks>
        /// <param name="t">How long</param>
        /// <returns>Awaitable task</returns>
        Task Delay(TimeSpan t);
    }
}
