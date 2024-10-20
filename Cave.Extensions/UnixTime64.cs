using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>unix time stamp in seconds since epoch</summary>
[StructLayout(LayoutKind.Sequential, Size = 8)]
public readonly struct UnixTime64 : IEquatable<UnixTime64>, IComparable<UnixTime64>, IFormattable
{
    /// <summary>Gets the current date time.</summary>
    public static UnixTime64 Now => DateTime.Now;

    /// <summary>Gets the current date time.</summary>
    public static UnixTime64 UtcNow => DateTime.UtcNow;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp == value2.TimeStamp;

    /// <summary>Implements the operator !=.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp != value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp > value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp >= value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp < value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(UnixTime64 value1, UnixTime64 value2) => value1.TimeStamp <= value2.TimeStamp;

    /// <summary>Performs an implicit conversion from <see cref="uint"/> to <see cref="UnixTime64"/>.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime64(long value) => new(value);

    /// <summary>Adds a <see cref="TimeSpan"/> to the <see cref="UnixTime64"/>.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime64 operator +(UnixTime64 value1, TimeSpan value2) => new(value1.TimeStamp + (value2.Ticks / TimeSpan.TicksPerSecond));

    /// <summary>Subtracts a <see cref="TimeSpan"/> from the <see cref="UnixTime64"/>.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime64 operator -(UnixTime64 value1, TimeSpan value2) => new(value1.TimeStamp - (value2.Ticks / TimeSpan.TicksPerSecond));

    /// <summary>Subtracts two <see cref="UnixTime64"/> values.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static TimeSpan operator -(UnixTime64 value1, UnixTime64 value2) => new((value1.TimeStamp - value2.TimeStamp) * TimeSpan.TicksPerSecond);

    /// <summary>Parses a UnixTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static UnixTime64 Parse(string value) => Parse(value, null);

    /// <summary>Parses a UnixTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static UnixTime64 Parse(string value, IFormatProvider? provider)
        => value is null ? throw new ArgumentNullException(nameof(value))
        : DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormat, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dateTime)
        ? dateTime
        : DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormatWithoutTimeZone, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateTime)
        ? dateTime
        : DateTime.Parse(value, provider ?? CultureInfo.CurrentCulture);

    /// <summary>Converts the specified date time.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static long Convert(DateTime dateTime) =>
        (dateTime.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond;

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="resultKind">Kind of the result.</param>
    /// <returns></returns>
    public static DateTime Convert(long timeStamp, DateTimeKind resultKind) =>
        new DateTime(1970, 1, 1, 0, 0, 0, resultKind) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="timeZone">The time zone.</param>
    /// <returns></returns>
    public static DateTime ConvertToUTC(long timeStamp, TimeSpan timeZone) =>
        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - timeZone + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);

    /// <summary>Performs an implicit conversion from <see cref="UnixTime64"/> to <see cref="DateTime"/>.</summary>
    /// <param name="t">The unix time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(UnixTime64 t) => t.ToDateTime();

    /// <summary>Performs an implicit conversion from <see cref="DateTime"/> to <see cref="UnixTime64"/>.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime64(DateTime dateTime) => new(Convert(dateTime));

    /// <summary>Creates a new instance of the <see cref="UnixTime64"/> structure.</summary>
    /// <param name="timestamp"></param>
    public UnixTime64(long timestamp) => TimeStamp = timestamp;

    /// <summary>The time stamp in seconds since 1.1.1970</summary>
    public readonly long TimeStamp;

    /// <summary>Gets or sets the date time.</summary>
    /// <value>The date time.</value>
    public DateTime ToDateTime(DateTimeKind kind = 0) => Convert(TimeStamp, kind);

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public override int GetHashCode() => TimeStamp.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UnixTime64 time && Equals(time);

    /// <inheritdoc/>
    public bool Equals(UnixTime64 other) => TimeStamp.Equals(other.TimeStamp);

    /// <inheritdoc/>
    public int CompareTo(UnixTime64 other) => TimeStamp.CompareTo(other.TimeStamp);

    #region IFormattable

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
        => ToDateTime().ToString(format ?? StringExtensions.InteropDateTimeFormatWithoutTimeZone, formatProvider ?? CultureInfo.CurrentCulture);

    #endregion IFormattable
}
