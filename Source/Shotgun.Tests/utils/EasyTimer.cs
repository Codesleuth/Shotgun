using System;
using System.Diagnostics;

namespace Shotgun.AcceptanceTests.utils
{
    public class EasyTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        public static EasyTimer StartNew()
        {
            return new EasyTimer();
        }

        /// <summary>
        /// Creates and starts a new timer.
        /// </summary>
        private EasyTimer()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
        }

        /// <summary>
        /// Gets the total elapsed time measured by the current instance, in milliseconds.
        /// </summary>
        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

        /// <summary>
        /// Gets the total elapsed time measured by the current instance.
        /// </summary>
        public TimeSpan Elapsed => _stopwatch.Elapsed;
    }
}