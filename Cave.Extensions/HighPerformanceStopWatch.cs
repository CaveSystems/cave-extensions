using System;
using System.Diagnostics;

namespace Cave
{
    /// <summary>
    /// Implements the <see cref="IStopWatch"/> interface on top of the <see cref="Stopwatch"/> class.
    /// </summary>
    [DebuggerDisplay("{Elapsed}")]
    public sealed class HighPerformanceStopWatch : IStopWatch
    {
        /// <summary>
        /// Starts a new stopwatch.
        /// </summary>
        /// <returns>The new started stopwatch.</returns>
        public static HighPerformanceStopWatch StartNew()
        {
            var result = new HighPerformanceStopWatch();
            result.Start();
            return result;
        }

        Stopwatch stopwatch;
        TimeSpan elapsed;

        /// <summary>Initializes a new instance of the <see cref="HighPerformanceStopWatch"/> class.</summary>
        public HighPerformanceStopWatch()
        {
            Reset();
        }

        /// <summary>
        /// Checks for equality with another stopwatch.
        /// </summary>
        /// <param name="obj">The other stopwatch.</param>
        /// <returns>True if the stopwatches are equal.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as IStopWatch;
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return
                other.StartDateTime == StartDateTime &&
                other.IsRunning == IsRunning;
        }

        /// <summary>
        /// Gets the hashcode for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Starts the <see cref="IStopWatch"/> (to restart a <see cref="IStopWatch"/> use <see cref="Reset"/> first!).
        /// </summary>
        public void Start()
        {
            if (stopwatch != null)
            {
                throw new InvalidOperationException(string.Format("Timer already running!"));
            }
#if !NETSTANDARD13
            System.Threading.Thread.BeginThreadAffinity();
#endif
            stopwatch = new Stopwatch();
            StartDateTime = DateTime.UtcNow;
            stopwatch.Start();
        }

        /// <summary>
        /// Resets the <see cref="IStopWatch"/> (can be used even if the <see cref="IStopWatch"/> is running).
        /// </summary>
        public void Reset()
        {
            if (stopwatch != null)
            {
                stopwatch.Reset();
                stopwatch.Start();
            }
            elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Stops the <see cref="IStopWatch"/>.
        /// </summary>
        public void Stop()
        {
            if (stopwatch == null)
            {
                throw new InvalidOperationException(string.Format("Timer not running!"));
            }

            elapsed = stopwatch.Elapsed;
            stopwatch = null;
#if !NETSTANDARD13
            System.Threading.Thread.EndThreadAffinity();
#endif
        }

        /// <summary>Waits until the specified <see cref="Elapsed" /> time is reached.</summary>
        /// <param name="elapsed">The elapsed timespan.</param>
        /// <exception cref="System.Exception">StopWatch is not running!.</exception>
        public void Wait(TimeSpan elapsed)
        {
            while (true)
            {
                if (!IsRunning)
                {
                    throw new Exception("StopWatch is not running!");
                }

                TimeSpan waitTime = elapsed - Elapsed;
                if (waitTime > TimeSpan.Zero)
                {
#if NETSTANDARD13
                    System.Threading.Tasks.Task.Delay(waitTime).Wait();
#else
                    System.Threading.Thread.Sleep(waitTime);
#endif
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        public TimeSpan Elapsed => stopwatch == null ? elapsed : stopwatch.Elapsed;

        /// <summary>
        /// Gets the elapsed time in milli seconds.
        /// </summary>
        public long ElapsedMilliSeconds => Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>Gets the elapsed time in seconds.</summary>
        public double ElapsedSeconds => Elapsed.Ticks / (double)TimeSpan.TicksPerSecond;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IStopWatch"/> is running or not.
        /// </summary>
        public bool IsRunning => stopwatch != null;

        /// <summary>
        /// Gets the resolution of the <see cref="IStopWatch"/> in seconds.
        /// </summary>
        public TimeSpan Resolution => StopWatch.CheckResolution(StartNew());

        /// <summary>
        /// Gets the frequency of the <see cref="IStopWatch"/> in HZ.
        /// </summary>
        public long Frequency => (long)(TimeSpan.TicksPerSecond / (double)Resolution.Ticks);

        /// <summary>
        /// Gets the <see cref="DateTime"/> (utc) value at start of the <see cref="IStopWatch"/>.
        /// </summary>
        public DateTime StartDateTime { get; private set; }
    }
}
