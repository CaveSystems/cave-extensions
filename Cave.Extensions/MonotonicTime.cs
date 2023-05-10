﻿using System;
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
    public readonly struct Drift
    {
        public static implicit operator TimeSpan(Drift drift) => drift.Average;

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

        public TimeSpan Min { get; }

        public TimeSpan Max { get; }

        public TimeSpan Average { get; }

        public TimeSpan StdDev { get; }
    }

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
            if (result.Average > TimeSpan.Zero) Thread.Sleep(result.Average);
        }
        Debug.WriteLine($"Calibration complete. StdDev {result.StdDev.FormatTime()}, average {result.Average.FormatTime()}, min {result.Min.FormatTime()}, max {result.Max.FormatTime()}");
        return result;
    }

    /// <summary>
    /// Gets the drift between this instance and the system clock. This might increase over time (drift of performance counter / platform
    /// high performance timer) and jump on time synchronizations or user interaction with the system time and cannot be corrected.
    /// </summary>
    public static Drift GetDrift(int samples = 0)
    {
        if (samples <= 1) samples = 1000;
        Trace.WriteLineIf(samples < 10, "Less than 100 samples may result in very bad average values!");
        var values = new double[samples];
        var count = 0;

        var last = DateTime.UtcNow;
        for (; count < values.Length; count++)
        {
            DateTime now;
            State state;
            for (; ; )
            {
                now = DateTime.UtcNow;
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
        lock (syncRoot) return state;
    }

    #endregion
}
