using System;
using System.Diagnostics;

namespace Cave
{
    /// <summary>
    /// Implements a basic timer with a low resolution based on <see cref="Environment.TickCount"/>
    /// (This class has an accuracy of about 15 msec on most platforms.)
    /// </summary>
    [DebuggerDisplay("{Elapsed}")]
    public sealed class DateTimeStopWatch : IStopWatch
    {
        /// <summary>
        /// Starts a new stop watch
        /// </summary>
        /// <returns></returns>
        public static DateTimeStopWatch StartNew()
        {
            DateTimeStopWatch result = new DateTimeStopWatch();
            result.Start();
            return result;
        }

        /// <summary>
        /// The <see cref="DateTime"/> value at start of the <see cref="IStopWatch"/>
        /// </summary>
        DateTime m_StartDateTime;

        /// <summary>
        /// Obtains the elapsed time if the timer is no longer running
        /// </summary>
        TimeSpan m_Elapsed;

        /// <summary>
        /// Flag to check if the timer is running
        /// </summary>
        bool m_Running;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeStopWatch"/> class.
        /// </summary>
        public DateTimeStopWatch()
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
            if (m_Elapsed != TimeSpan.Zero)
            {
                throw new InvalidOperationException(string.Format("Timer has to be reset first!"));
            }

            if (m_Running)
            {
                throw new InvalidOperationException(string.Format("Timer already running!"));
            }

            m_StartDateTime = DateTime.UtcNow;
            m_Running = true;
        }

        /// <summary>
        /// Resets the <see cref="IStopWatch"/> (can be used even if the <see cref="IStopWatch"/> is running)
        /// </summary>
        public void Reset()
        {
            m_StartDateTime = DateTime.UtcNow;
            m_Elapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Stops the <see cref="IStopWatch"/>
        /// </summary>
        public void Stop()
        {
            if (!m_Running)
            {
                throw new InvalidOperationException(string.Format("Timer not running!"));
            }

            m_Elapsed = DateTime.UtcNow - m_StartDateTime;
            m_Running = false;
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
        public TimeSpan Elapsed
        {
            get
            {
                if (m_Running)
                { return DateTime.UtcNow - m_StartDateTime; }
                else
                { return m_Elapsed; }
            }

        }

        /// <summary>
        /// Gets the elapsed time in milli seconds.
        /// </summary>
        public long ElapsedMilliSeconds => Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

        /// <summary>
        /// Gets the elapsed time in seconds.
        /// </summary>
        public double ElapsedSeconds => Elapsed.Ticks / (double)TimeSpan.TicksPerSecond;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IStopWatch"/> is running or not
        /// </summary>
        public bool IsRunning => m_Running;

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
        public DateTime StartDateTime => m_StartDateTime;

        /// <summary>
        /// Creates a copy of the <see cref="IStopWatch"/>
        /// </summary>
        /// <returns>Returns a copy of the <see cref="IStopWatch"/></returns>
        public object Clone()
        {
            DateTimeStopWatch result = new DateTimeStopWatch
            {
                m_StartDateTime = m_StartDateTime,
                m_Elapsed = m_Elapsed,
                m_Running = m_Running
            };
            return result;
        }
    }
}
