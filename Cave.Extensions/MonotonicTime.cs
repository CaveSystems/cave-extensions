using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cave;

/// <summary>Provides a fast and monotonic time implementation based on the systems high performance counter.</summary>
public static class MonotonicTime
{
    #region Private Classes

    sealed class State
    {
        #region Private Constructors

        State(long timer, long clock, long start)
        {
            Timer = timer;
            Clock = clock;
            Start = start;
        }

        #endregion Private Constructors

        #region Public Fields

        public readonly long Clock;

        public readonly long Start;

        public readonly long Timer;

        #endregion Public Fields

        #region Public Properties

        public DateTime ClockTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new(Clock, DateTimeKind.Utc);
        }

        public DateTime StartTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new(Start, DateTimeKind.Utc);
        }

        public TimeSpan TimerTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new(Timer);
        }

        #endregion Public Properties

        #region Public Methods

        public static State Init()
        {
            var now = GetSystemUtcDateTime();
            var ticks = (long)(Stopwatch.GetTimestamp() * stampToTicks);
            return new State(ticks, now.Ticks, now.Ticks - ticks);
        }

        [MethodImpl((MethodImplOptions)256)]
        public State UpdateStartTime(long start)
        {
            var timer = Math.Max(Timer, (long)(Stopwatch.GetTimestamp() * stampToTicks));
            var clock = Math.Max(Clock, start + timer);
            var newState = new State(timer, clock, start);
            if (timer < newState.Timer || clock < newState.Clock) throw new Exception();
            return newState;
        }

        [MethodImpl((MethodImplOptions)256)]
        public State UpdateTimeStamp()
        {
            var timer = Math.Max(Timer, (long)(Stopwatch.GetTimestamp() * stampToTicks));
            var clock = Math.Max(Clock, Start + timer);
            var newState = new State(timer, clock, Start);
            if (timer < newState.Timer || clock < newState.Clock) throw new InvalidDataException("Systemclock is moving backwards in time!");
            return newState;
        }

        #endregion Public Methods
    }

    #endregion Private Classes

    #region Private Fields

    static readonly TimeSpan offset;

    static readonly double stampToTicks;

    static readonly object syncRoot = new();

    static volatile State state;

    #endregion Private Fields

    #region Private Methods

    [MethodImpl((MethodImplOptions)256)]
    static State GetMonotonicState()
    {
        //one writer only
        if (Monitor.TryEnter(syncRoot))
        {
            try
            {
                state = state.UpdateTimeStamp();
                return state;
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }
        return state;
    }

    #endregion Private Methods

    #region Public Structs

    /// <summary>Provides a structure representing the average drift of values to a reference.</summary>
    public readonly struct Drift
    {
        #region Internal Constructors

        internal Drift(double[] values, int count)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
            var min = values[0];
            var max = values[0];
            var sum = 0.0;
            for (var i = 0; i < count; i++)
            {
                min = Math.Min(values[i], min);
                max = Math.Min(values[i], max);
                sum += values[i];
            }
            //average
            var avg = sum / count;

            var stdDevSum = 0.0;
            for (var i = 0; i < count; i++)
            {
                //add (deviation from mean)²
                stdDevSum += Math.Pow(values[i] - avg, 2);
            }
            StdDev = new TimeSpan((long)Math.Sqrt(stdDevSum / (count + 1)));
            Min = new TimeSpan((long)min);
            Max = new TimeSpan((long)max);
            Average = new TimeSpan((long)avg);
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>Gets the average drift within the range checked</summary>
        public TimeSpan Average { get; }

        /// <summary>Gets the maximum drift within the range checked</summary>
        public TimeSpan Max { get; }

        /// <summary>Gets the minimum drift within the range checked</summary>
        public TimeSpan Min { get; }

        /// <summary>Gets the standard deviation within the range checked</summary>
        public TimeSpan StdDev { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>Converts the drift to a timespan.</summary>
        /// <param name="drift"></param>
        public static implicit operator TimeSpan(Drift drift) => drift.Average;

        #endregion Public Methods
    }

    #endregion Public Structs

    #region Public Constructors

    static MonotonicTime()
    {
        IsHighResolution = Stopwatch.IsHighResolution;
        stampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        state = State.Init();
        offset = state.ClockTime.ToLocalTime() - state.ClockTime;
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Indicates whether the timer is based on a high-resolution performance counter.</summary>
    public static bool IsHighResolution { get; }

    /// <summary>Gets the current monotonic advancing local time.</summary>
    public static DateTime Now
    {
        [MethodImpl((MethodImplOptions)256)]
        get => new(GetMonotonicState().Clock + offset.Ticks, DateTimeKind.Local);
    }

    /// <summary>Gets the monotonic time systems start time.</summary>
    public static DateTime StartTime => GetMonotonicState().StartTime;

    /// <summary>Gets the monotonic time systems uptime since last resync.</summary>
    public static TimeSpan Uptime
    {
        [MethodImpl((MethodImplOptions)256)]
        get => GetMonotonicState().TimerTime;
    }

    /// <summary>Gets the current monotonic advancing utc time.</summary>
    public static DateTime UtcNow
    {
        [MethodImpl((MethodImplOptions)256)]
        get => GetMonotonicState().ClockTime;
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Function to be used when synchronizing and calibrating ( <see cref="Resync"/>, <see cref="Calibrate"/>).</summary>
    public static Func<DateTime> GetSystemUtcDateTime { get; set; } = () => DateTime.UtcNow;

    /// <summary>
    /// Calibrates the system start time and the current time. This is needed if the timer is used for a long time and the difference at <see cref="GetDrift"/>
    /// increases too much.
    /// </summary>
    /// <returns></returns>
    public static Drift Calibrate()
    {
        Drift result = default;
        for (var round = 1; round <= 3; round++)
        {
            // 10, 100, 1000 samples
            result = GetDrift((int)Math.Pow(10, round));
            Debug.WriteLine($"Calibrating... current drift {result.Average.FormatTime()}, round {round}.");

            lock (syncRoot)
            {
                state = state.UpdateStartTime(state.Start + result.Average.Ticks);
            }
            if (result.Average > TimeSpan.Zero)
            {
#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
                Task.Delay(result.Average);
#else
                Thread.Sleep(result.Average);
#endif
            }
        }
        Debug.WriteLine($"Calibration complete. StdDev {result.StdDev.FormatTime()}, average {result.Average.FormatTime()}, min {result.Min.FormatTime()}, max {result.Max.FormatTime()}");
        return result;
    }

    /// <summary>
    /// Gets the drift between this instance and the system clock. This might increase over time (drift of performance counter / platform high performance
    /// timer) and jump on time synchronizations or user interaction with the system time and cannot be corrected.
    /// </summary>
    public static Drift GetDrift(int samples = 0)
    {
        if (samples <= 1) samples = 1000;
        Trace.WriteLineIf(samples < 10, "Less than 100 samples may result in very bad average values!");
        var values = new double[samples];
        var count = 0;

        var last = GetSystemUtcDateTime();
        for (; count < values.Length; count++)
        {
            DateTime now;
            State state;
            for (; ; )
            {
                now = GetSystemUtcDateTime();
                state = GetMonotonicState();
                //wait for tick
                if (now > last) break;
            }
            values[count] = now.Ticks - state.Clock;
            if (count > samples) break;
            last = now;
        }
        return new Drift(values, count);
    }

    /// <summary>Restarts the monotonic time system resulting in a resync of the start time and the current time.</summary>
    /// <returns></returns>
    public static Drift Resync()
    {
        Drift result = default;
        lock (syncRoot)
        {
            state = State.Init();
            result = GetDrift(10);
        }
        Debug.WriteLine($"Resync complete. StdDev {result.StdDev.FormatTime()}, average {result.Average.FormatTime()}, min {result.Min.FormatTime()}, max {result.Max.FormatTime()}");
        return result;
    }

    #endregion Public Methods
}
