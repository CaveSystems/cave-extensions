#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

using System;
using System.Diagnostics;
using System.Threading;

namespace Cave;

/// <summary>Implements the <see cref="IStopWatch"/> interface on top of the <see cref="Stopwatch"/> class.</summary>
[DebuggerDisplay("{Elapsed}")]
public sealed class HighPerformanceStopWatch : IStopWatch
{
    #region Static

    /// <summary>Starts a new stopwatch.</summary>
    /// <returns>The new started stopwatch.</returns>
    public static HighPerformanceStopWatch StartNew()
    {
        var result = new HighPerformanceStopWatch();
        result.Start();
        return result;
    }

    #endregion Static

    #region Fields

    TimeSpan elapsed;
    Stopwatch? stopwatch;

    #endregion Fields

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="HighPerformanceStopWatch"/> class.</summary>
    public HighPerformanceStopWatch() => Reset();

    #endregion Constructors

    #region IStopWatch Members

    /// <summary>Gets the elapsed time.</summary>
    public TimeSpan Elapsed => stopwatch == null ? elapsed : stopwatch.Elapsed;

    /// <summary>Gets the elapsed time in milli seconds.</summary>
    public long ElapsedMilliSeconds => Elapsed.Ticks / TimeSpan.TicksPerMillisecond;

    /// <summary>Gets the elapsed time in seconds.</summary>
    public double ElapsedSeconds => Elapsed.Ticks / (double)TimeSpan.TicksPerSecond;

    /// <summary>Gets the frequency of the <see cref="IStopWatch"/> in HZ.</summary>
    public long Frequency => (long)(TimeSpan.TicksPerSecond / (double)Resolution.Ticks);

    /// <summary>Gets a value indicating whether the <see cref="IStopWatch"/> is running or not.</summary>
    public bool IsRunning => stopwatch != null;

    /// <summary>Resets the <see cref="IStopWatch"/> (can be used even if the <see cref="IStopWatch"/> is running).</summary>
    public void Reset()
    {
        if (stopwatch != null)
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        elapsed = TimeSpan.Zero;
    }

    /// <summary>Gets the resolution of the <see cref="IStopWatch"/> in seconds.</summary>
    public TimeSpan Resolution => StopWatch.CheckResolution(StartNew());

    /// <summary>Starts the <see cref="IStopWatch"/> (to restart a <see cref="IStopWatch"/> use <see cref="Reset"/> first!).</summary>
    public void Start()
    {
        if (stopwatch != null)
        {
            throw new InvalidOperationException("Timer already running!");
        }
        Thread.BeginThreadAffinity();
        stopwatch = new();
        StartDateTime = DateTime.UtcNow;
        stopwatch.Start();
    }

    /// <summary>Gets the <see cref="DateTime"/> (utc) value at start of the <see cref="IStopWatch"/>.</summary>
    public DateTime StartDateTime { get; private set; }

    /// <summary>Stops the <see cref="IStopWatch"/>.</summary>
    public void Stop()
    {
        if (stopwatch == null)
        {
            throw new InvalidOperationException("Timer not running!");
        }

        elapsed = stopwatch.Elapsed;
        stopwatch = null;
        Thread.EndThreadAffinity();
    }

    /// <summary>Waits until the specified <see cref="Elapsed"/> time is reached.</summary>
    /// <param name="elapsed">The elapsed timespan.</param>
    /// <exception cref="System.Exception">StopWatch is not running!.</exception>
    public void Wait(TimeSpan elapsed)
    {
        while (true)
        {
            if (!IsRunning)
            {
                throw new("StopWatch is not running!");
            }

            var waitTime = elapsed - Elapsed;
            if (waitTime > TimeSpan.Zero)
            {
                Thread.Sleep(waitTime);
            }
            else
            {
                break;
            }
        }
    }

    #endregion IStopWatch Members
}

#endif
