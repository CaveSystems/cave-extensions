using System;
using System.Runtime.CompilerServices;

namespace Cave;

/// <summary>
/// Provides extensions to the <see cref="TimeSpan"/> struct.
/// </summary>
public static class TimeSpanExtension
{
    /// <summary>
    /// Gets the absolute (&gt;= 0) time.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    [MethodImpl((MethodImplOptions)256)]
    public static TimeSpan Absolute(this TimeSpan time) => time.Ticks < 0 ? new TimeSpan(-time.Ticks) : time;
}
