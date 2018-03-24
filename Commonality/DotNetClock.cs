using System;
using System.Threading.Tasks;

namespace Commonality
{
    /// <summary>
    /// Provides a standard dotnet 'DateTime.Now' in the IClock interface, with the ability to
    /// set the time for internal use only. (Doesn't affect the system clock)
    /// </summary>
    /// <remarks>
    /// This is helpful both for testing, and for a system where we use an external clock
    /// </remarks>
    public class Clock : IClock
    {
        /// <summary>
        /// Current time
        /// </summary>
        /// <remarks>
        /// Note that you can also set this, in which case all future gets will be offset by this amount.
        /// This offset is persisted, so next time the app is launched, this offset will remain.
        /// </remarks>
        public DateTime Now
        {
            get
            {
                if (!Offset.HasValue)
                {
                    Offset = TimeSpan.FromTicks(long.Parse( Settings?.GetKey("Clock.Offset") ?? "0"));
                }
                return DateTime.Now + Offset.Value;
            }
            set
            {
                Offset = value - DateTime.Now;
                Settings?.SetKey("Clock.Offset", Offset.Value.Ticks.ToString());
            }
        }

        /// <summary>
        /// Service locator to find the settings.
        /// </summary>
        /// <remarks>
        /// If no settings service is found, offset is not persisted to settings.
        /// </remarks>
        private ISettings Settings => Service.TryGet<ISettings>();

        /// <summary>
        /// Amount to offset internal time from the local time
        /// </summary>
        private TimeSpan? Offset;

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
        public Task Delay(TimeSpan t) => Task.Delay(t);
    }
}
