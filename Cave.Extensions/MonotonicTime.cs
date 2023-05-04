using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cave.Collections.Generic;

namespace Cave;

/// <summary>Provides a fast and monotonic time implementation based on the systems high performance counter.</summary>
public static class MonotonicTime
{
    class State
    {
        public static State Init()
        {
            var now = DateTime.UtcNow;
            var ticks = (long)(Stopwatch.GetTimestamp() * stampToTicks);
            return new State(ticks, now.Ticks, now.Ticks - ticks);
        }

        State(long timer, long clock, long start)
        {
            Timer = timer;
            Clock = clock;
            Start = start;
        }
        public readonly long Clock;
        public readonly long Timer;
        public readonly long Start;

        public TimeSpan TimerTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new TimeSpan(Timer);
        }

        public DateTime ClockTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new DateTime(Clock, DateTimeKind.Utc);
        }

        public DateTime StartTime
        {
            [MethodImpl((MethodImplOptions)256)]
            get => new DateTime(Start, DateTimeKind.Utc);
        }

        [MethodImpl((MethodImplOptions)256)]
        public State UpdateTimeStamp()
        {
            var timer = Math.Max(Timer, (long)(Stopwatch.GetTimestamp() * stampToTicks));
            var clock = Math.Max(Clock, Start + timer);
            var newState = new State(timer, clock, Start);
            if (timer < newState.Timer || clock < newState.Clock) throw new Exception();
            return newState;
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
    }
    #region Static

    static readonly object syncRoot = new();
    static readonly TimeSpan offset;
    static readonly double stampToTicks;
    static volatile State state;


    /// <summary>
    /// Indicates whether the timer is based on a high-resolution performance counter.
    /// </summary>
    public static bool IsHighResolution { get; }

    /// <summary>Calibrates the system start time and the current time. This is needed if the timer is used for a long time and the difference at <see cref="GetDrift" /> increases too much.</summary>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public static TimeSpan Calibrate(TimeSpan accuracy = default)
    {
        //minimum accuracy is 100 stamps
        var accuracyTicks = (long)Math.Max(100 * stampToTicks, accuracy.Ticks);
        var maxRounds = IsHighResolution ? 20 : 10;
        TimeSpan result;
        for (var round = 1; ; round++)
        {
            result = GetDrift();
            Debug.WriteLine($"Calibrating with accuracy requirement {accuracyTicks.FormatTicks()}, current drift {result.FormatTime()}, round {round}.");
            if (round > maxRounds) break;
            if (Math.Abs(result.Ticks) < accuracyTicks)
            {
                Debug.WriteLine($"Calibration complete.");
                return result;
            }

            lock (syncRoot)
            {
                state = state.UpdateStartTime(state.Start + result.Ticks);
            }
            if (result > TimeSpan.Zero) Thread.Sleep(result);
        }
        return result;
    }

    /// <summary>
    /// Gets the drift between this instance and the system clock. This might increase over time (drift of performance counter / platform
    /// high performance timer) and jump on time synchronizations or user interaction with the system time and cannot be corrected.
    /// </summary>
    public static TimeSpan GetDrift()
    {
        var drift = new List<long>(512);
        while (true)
        {
            //add one block
            for (var i = 0; i < 256; i++)
            {
                var value = DateTime.UtcNow.Ticks - GetMonotonicState().Clock;
                drift.Add(value);
            }
            //calculate standard deviation
            //average
            var avg = drift.Average();
            //deviation from mean
            var dev = drift.Select(d => (d - avg)).Select(d => d * d);
            //standard dev
            var stdDev = Math.Sqrt(dev.Sum() / (drift.Count + 1));
            //require a std dev < 0.01%
            var maxStdDev = Math.Abs(avg / 10000);
            if (stdDev < maxStdDev || drift.Count >= 10240)
            {
                return new TimeSpan((long)avg);
            }
        }
    }

    /// <summary>Gets the current monotonic advancing local time.</summary>
    public static DateTime Now
    {
        [MethodImpl((MethodImplOptions)256)]
        get => new(GetMonotonicState().Clock + offset.Ticks, DateTimeKind.Local);
    }

    /// <summary>Gets the systems start time.</summary>
    public static DateTime StartTime => GetMonotonicState().StartTime;

    /// <summary>Gets the current uptime.</summary>
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

    static MonotonicTime()
    {
        IsHighResolution = Stopwatch.IsHighResolution;
        stampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        state = State.Init();
        offset = state.ClockTime.ToLocalTime() - state.ClockTime;
    }

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
        lock(syncRoot)
        return state;
    }

    #endregion
}
