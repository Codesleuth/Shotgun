using System;
using System.Diagnostics;

namespace Shotgun.AcceptanceTests.utils
{
    public class EasyTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Creates and starts a new timer.
        /// </summary>
        public EasyTimer()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
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
        public long ElapsedMilliseconds
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }
    }
}