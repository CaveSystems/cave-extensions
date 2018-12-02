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
        Stopwatch m_Stopwatch;
        TimeSpan m_Elapsed;

        /// <summary>
        /// Starts a new stopwatch
        /// </summary>
        /// <returns></returns>
        public static HighPerformanceStopWatch StartNew()
        {
            HighPerformanceStopWatch result = new HighPerformanceStopWatch();
            result.Start();
            return result;
        }

        /// <summary>Initializes a new instance of the <see cref="HighPerformanceStopWatch"/> class.</summary>
        public HighPerformanceStopWatch()
        {
            Reset();
        }

        /// <summary>
        /// Checks for equality with another stopwatch
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            IStopWatch other = obj as IStopWatch;
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return
                other.StartDateTime == StartDateTime &&
                other.IsRunning == IsRunning;
        }

        /// <summary>
        /// Obtains the hashcode for this instance
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Starts the <see cref="IStopWatch"/> (to restart a <see cref="IStopWatch"/> use <see cref="Reset"/> first!)
        /// </summary>
        public void Start()
        {
            if (m_Stopwatch != null)
            {
                throw new InvalidOperationException(string.Format("Timer already running!"));
            }
#if !NETSTANDARD13
            System.Threading.Thread.BeginThreadAffinity();
#endif
            m_Stopwatch = new Stopwatch();
            StartDateTime = DateTime.UtcNow;
            m_Stopwatch.Start();
        }

        /// <summary>
        /// Resets the <see cref="IStopWatch"/> (can be used even if the <see cref="IStopWatch"/> is running)
        /// </summary>
        public void Reset()
        {
            if (m_Stopwatch != null)
            {
                m_Stopwatch.Reset();
                m_Stopwatch.Start();
            }
            m_Elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Stops the <see cref="IStopWatch"/>
        /// </summary>
        public void Stop()
        {
            if (m_Stopwatch == null)
            {
                throw new InvalidOperationException(string.Format("Timer not running!"));
            }

            m_Elapsed = m_Stopwatch.Elapsed;
            m_Stopwatch = null;
#if !NETSTANDARD13
            System.Threading.Thread.EndThreadAffinity();
#endif
        }

        /// <summary>Waits until the specified <see cref="Elapsed" /> time is reached.</summary>
        /// <param name="elapsed">The elapsed timespan.</param>
        /// <exception cref="System.Exception">StopWatch is not running!</exception>
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
        public TimeSpan Elapsed => m_Stopwatch == null ? m_Elapsed : m_Stopwatch.Elapsed;

        /// <summary>
        /// Gets the elapsed time in milli seconds.
        /// </summary>
        public long ElapsedMilliSeconds => Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>Gets the elapsed time in seconds</summary>
        public double ElapsedSeconds => Elapsed.Ticks / (double)TimeSpan.TicksPerSecond;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IStopWatch"/> is running or not
        /// </summary>
        public bool IsRunning => m_Stopwatch != null;

        /// <summary>
        /// Gets the resolution of the <see cref="IStopWatch"/> in seconds
        /// </summary>
        public TimeSpan Resolution => StopWatch.CheckResolution(StartNew());

        /// <summary>
        /// Gets the frequency of the <see cref="IStopWatch"/> in HZ
        /// </summary>
        public long Frequency => (long)(TimeSpan.TicksPerSecond / (double)Resolution.Ticks);

        /// <summary>
        /// Gets the <see cref="DateTime"/> (utc) value at start of the <see cref="IStopWatch"/>
        /// </summary>
        public DateTime StartDateTime { get; private set; }
    }
}