using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cave.Collections;

namespace Cave;

/// <summary>Provides a fast and monotonic time implementation based on the systems high performance counter.</summary>
public static class MonotonicTime
{
    #region Static

    static readonly TimeSpan offset;
    static readonly double stampToTicks;

    static long lastUtcTicks;
    static long lastTicks;
    static long startTicks;

    /// <summary>Calibrates the system start time and the current time. This is needed if the <see cref="Drift" /> timer is used for a long time.</summary>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public static bool Calibrate(TimeSpan accuracy = default)
    {
        //minimum accuracy is 10 stamps
        var accuracyTicks = Math.Max(10 * stampToTicks, accuracy.Ticks);
        var maxRounds = 1000000;
        while (maxRounds-- > 0)
        {
            var drift = (long)new Counter(0, 100).Select(_ => Drift.Ticks).Average();
            if (Math.Abs(drift) < accuracyTicks)
            {
                return true;
            }

            Interlocked.Add(ref startTicks, drift);
            Thread.Sleep(new TimeSpan(Math.Abs(drift)));
        }
        return false;
    }

    /// <summary>
    /// Gets the drift between this instance and the system clock. This might increase over time (drift of performance counter / platform
    /// high performance timer) and jump on time synchronizations or user interaction with the system time and cannot be corrected.
    /// </summary>
    public static TimeSpan Drift
    {
        [MethodImpl((MethodImplOptions)256)]
        get
        {
            //test only at utc now tick
            var utcTicks = DateTime.UtcNow.Ticks;
            while (true)
            {
                Thread.Sleep(0);
                var test = DateTime.UtcNow.Ticks;
                if (test != utcTicks)
                {
                    utcTicks = test;
                    break;
                }
            }

            return new(utcTicks - GetMonotonicTicks(true));
        }
    }

    /// <summary>Gets the current monotonic advancing local time.</summary>
    public static DateTime Now
    {
        [MethodImpl((MethodImplOptions)256)]
        get => new(GetMonotonicTicks(true) + offset.Ticks, DateTimeKind.Local);
    }

    /// <summary>Gets the systems start time.</summary>
    public static DateTime StartTime => new(Interlocked.Read(ref startTicks), DateTimeKind.Utc);

    /// <summary>Gets the current uptime.</summary>
    public static TimeSpan Uptime
    {
        [MethodImpl((MethodImplOptions)256)]
        get => new(GetMonotonicTicks(false));
    }

    /// <summary>Gets the current monotonic advancing utc time.</summary>
    public static DateTime UtcNow
    {
        [MethodImpl((MethodImplOptions)256)]
        get => new(GetMonotonicTicks(true), DateTimeKind.Utc);
    }

    static MonotonicTime()
    {
        Thread.BeginThreadAffinity();
        Thread.BeginCriticalRegion();
        var now = DateTime.UtcNow;
        var stamp = Stopwatch.GetTimestamp();
        Thread.EndCriticalRegion();
        Thread.EndThreadAffinity();

        stampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        lastTicks = (long)(stamp * stampToTicks);
        startTicks = now.Ticks - lastTicks;
        lastUtcTicks = startTicks + lastTicks;
        offset = now.ToLocalTime() - now;
    }

    static long Update(ref long source, long value)
    {
        while (true)
        {
            var temp = Interlocked.Read(ref source);
            if (value <= temp)
            {
                return temp;
            }
            Interlocked.CompareExchange(ref source, value, temp);
        }
    }

    [MethodImpl((MethodImplOptions)256)]
    static long GetMonotonicTicks(bool withStartTime)
    {
        var ticks = Update(ref lastTicks, (long)(Stopwatch.GetTimestamp() * stampToTicks));
        if (!withStartTime) return ticks;
        return Update(ref lastUtcTicks, ticks + Interlocked.Read(ref startTicks));
    }

    #endregion
}
