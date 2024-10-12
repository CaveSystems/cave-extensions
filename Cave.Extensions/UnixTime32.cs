using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>unix time stamp in seconds since epoch</summary>
[StructLayout(LayoutKind.Sequential, Size = 4)]
public readonly struct UnixTime32 : IEquatable<UnixTime32>, IComparable<UnixTime32>, IFormattable
{
    /// <summary>Gets the current date time.</summary>
    public static UnixTime32 Now => DateTime.Now;

    /// <summary>Gets the current date time.</summary>
    public static UnixTime32 UtcNow => DateTime.UtcNow;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp == value2.TimeStamp;

    /// <summary>Implements the operator !=.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp != value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp > value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp >= value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp < value2.TimeStamp;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(UnixTime32 value1, UnixTime32 value2) => value1.TimeStamp <= value2.TimeStamp;

    /// <summary>Performs an implicit conversion from <see cref="uint"/> to <see cref="UnixTime32"/>.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime32(uint value) => new(value);

    /// <summary>Adds a <see cref="TimeSpan"/> to the <see cref="UnixTime64"/>.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime32 operator +(UnixTime32 value1, TimeSpan value2) => new((uint)(value1.TimeStamp + (value2.Ticks / TimeSpan.TicksPerSecond)));

    /// <summary>Substracts a <see cref="TimeSpan"/> from the <see cref="UnixTime32"/>.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime32 operator -(UnixTime32 value1, TimeSpan value2) => new((uint)(value1.TimeStamp - (value2.Ticks / TimeSpan.TicksPerSecond)));

    /// <summary>Substracts two <see cref="UnixTime64"/> values.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static TimeSpan operator -(UnixTime32 value1, UnixTime32 value2) => new((value1.TimeStamp - value2.TimeStamp) * TimeSpan.TicksPerSecond);

    /// <summary>Parses a UnixTime32 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static UnixTime32 Parse(string value) => Parse(value, null);

    /// <summary>Parses a UnixTime32 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static UnixTime32 Parse(string value, IFormatProvider? provider)
        => value is null ? throw new ArgumentNullException(nameof(value))
        : DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormat, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dateTime)
        ? dateTime
        : DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormatWithoutTimeZone, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateTime)
        ? dateTime
        : DateTime.Parse(value, provider ?? CultureInfo.CurrentCulture);

    /// <summary>Converts the specified date time.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static uint Convert(DateTime dateTime) =>
        (uint)((dateTime.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond);

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="resultKind">Kind of the result.</param>
    /// <returns></returns>
    public static DateTime Convert(uint timeStamp, DateTimeKind resultKind) =>
        new DateTime(1970, 1, 1, 0, 0, 0, resultKind) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="timeZone">The time zone.</param>
    /// <returns></returns>
    public static DateTime ConvertToUTC(uint timeStamp, TimeSpan timeZone) =>
        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - timeZone + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);

    /// <summary>Performs an implicit conversion from <see cref="UnixTime32"/> to <see cref="DateTime"/>.</summary>
    /// <param name="t">The unix time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(UnixTime32 t) => t.ToDateTime();

    /// <summary>Performs an implicit conversion from <see cref="DateTime"/> to <see cref="UnixTime32"/>.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime32(DateTime dateTime) => new(Convert(dateTime));

    /// <summary>Creates a new instance of the <see cref="UnixTime32"/> structure.</summary>
    /// <param name="timestamp"></param>
    public UnixTime32(uint timestamp) => TimeStamp = timestamp;

    /// <summary>The time stamp in seconds since 1.1.1970, this will overflow in 2038</summary>
    public readonly uint TimeStamp;

    /// <summary>Gets or sets the date time.</summary>
    /// <value>The date time.</value>
    public DateTime ToDateTime(DateTimeKind kind = 0) => Convert(TimeStamp, kind);

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public override int GetHashCode() => TimeStamp.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UnixTime32 time && Equals(time);

    /// <inheritdoc/>
    public bool Equals(UnixTime32 other) => TimeStamp.Equals(other.TimeStamp);

    /// <inheritdoc/>
    public int CompareTo(UnixTime32 other) => TimeStamp.CompareTo(other.TimeStamp);

    #region IFormattable

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
        => ToDateTime().ToString(format ?? StringExtensions.InteropDateTimeFormatWithoutTimeZone, formatProvider ?? CultureInfo.CurrentCulture);

    #endregion IFormattable
}
