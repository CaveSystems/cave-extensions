using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>unix time stamp in seconds since epoch</summary>
[StructLayout(LayoutKind.Sequential, Size = 4)]
public readonly struct UnixTime32 : IEquatable<UnixTime32>, IComparable<UnixTime32>, IConvertible, IFormattable
{
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

    /// <summary>Performs an implicit conversion from <see cref="uint" /> to <see cref="UnixTime32" />.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime32(uint value) => new(value);

    /// <summary>Adds a <see cref="TimeSpan" /> to the <see cref="UnixTime64" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime32 operator +(UnixTime32 value1, TimeSpan value2) => new((uint)(value1.TimeStamp + (value2.Ticks / TimeSpan.TicksPerSecond)));

    /// <summary>Substracts a <see cref="TimeSpan" /> from the <see cref="UnixTime32" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime32 operator -(UnixTime32 value1, TimeSpan value2) => new((uint)(value1.TimeStamp - (value2.Ticks / TimeSpan.TicksPerSecond)));

    /// <summary>Substracts two <see cref="UnixTime64" /> values.</summary>
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
    public static UnixTime32 Parse(string value, IFormatProvider provider)
        => DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormat, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dateTime)
        ? dateTime
        : DateTime.TryParseExact(value, StringExtensions.InteropDateTimeFormatWithoutTimeZone, provider ?? CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateTime)
        ? dateTime
        : DateTime.Parse(value, provider ?? CultureInfo.CurrentCulture);

    /// <summary>Converts the specified date time.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static uint Convert(DateTime dateTime)
    {
        if (dateTime.Ticks == 0)
        {
            return 0;
        }
        return (uint)((dateTime.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond);
    }

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="resultKind">Kind of the result.</param>
    /// <returns></returns>
    public static DateTime Convert(uint timeStamp, DateTimeKind resultKind)
    {
        if (timeStamp == 0)
        {
            return new(0, resultKind);
        }
        return new DateTime(1970, 1, 1, 0, 0, 0, resultKind) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);
    }

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="timeZone">The time zone.</param>
    /// <returns></returns>
    public static DateTime ConvertToUTC(uint timeStamp, TimeSpan timeZone)
    {
        if (timeStamp == 0)
        {
            return new(0, DateTimeKind.Unspecified);
        }
        return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - timeZone) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);
    }

    /// <summary>Performs an implicit conversion from <see cref="UnixTime32" /> to <see cref="DateTime" />.</summary>
    /// <param name="t">The unix time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(UnixTime32 t) => t.DateTime;

    /// <summary>Performs an implicit conversion from <see cref="DateTime" /> to <see cref="UnixTime32" />.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime32(DateTime dateTime) => new UnixTime32(Convert(dateTime));

    /// <summary>
    /// Creates a new instance of the <see cref="UnixTime32"/> structure.
    /// </summary>
    /// <param name="timestamp"></param>
    public UnixTime32(uint timestamp) => TimeStamp = timestamp;

    /// <summary>The time stamp in seconds since 1.1.1970, this will overflow in 2038</summary>
    public readonly uint TimeStamp;

    /// <summary>Gets or sets the date time.</summary>
    /// <value>The date time.</value>
    public DateTime DateTime => Convert(TimeStamp, DateTimeKind.Unspecified);

    /// <inheritdoc />
    public override string ToString() => ToString(null, null);

    /// <inheritdoc />
    public override int GetHashCode() => TimeStamp.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UnixTime32 time && Equals(time);

    /// <inheritdoc />
    public bool Equals(UnixTime32 other) => TimeStamp.Equals(other.TimeStamp);

    /// <inheritdoc />
    public int CompareTo(UnixTime32 other) => TimeStamp.CompareTo(other.TimeStamp);

    #region IConvertible

    /// <inheritdoc />
    public TypeCode GetTypeCode() => TypeCode.DateTime;

    /// <inheritdoc />
    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)DateTime).ToBoolean(provider);

    /// <inheritdoc />
    public byte ToByte(IFormatProvider provider) => ((IConvertible)DateTime).ToByte(provider);

    /// <inheritdoc />
    public char ToChar(IFormatProvider provider) => ((IConvertible)DateTime).ToChar(provider);

    /// <inheritdoc />
    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)DateTime).ToDateTime(provider);

    /// <inheritdoc />
    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)DateTime).ToDecimal(provider);

    /// <inheritdoc />
    public double ToDouble(IFormatProvider provider) => ((IConvertible)DateTime).ToDouble(provider);

    /// <inheritdoc />
    public short ToInt16(IFormatProvider provider) => ((IConvertible)DateTime).ToInt16(provider);

    /// <inheritdoc />
    public int ToInt32(IFormatProvider provider) => ((IConvertible)DateTime).ToInt32(provider);

    /// <inheritdoc />
    public long ToInt64(IFormatProvider provider) => ((IConvertible)DateTime).ToInt64(provider);

    /// <inheritdoc />
    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)DateTime).ToSByte(provider);

    /// <inheritdoc />
    public float ToSingle(IFormatProvider provider) => ((IConvertible)DateTime).ToSingle(provider);

    /// <inheritdoc />
    public string ToString(IFormatProvider provider) => DateTime.ToString(provider);

    /// <inheritdoc />
    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)DateTime).ToType(conversionType, provider);

    /// <inheritdoc />
    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)DateTime).ToUInt16(provider);

    /// <inheritdoc />
    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)DateTime).ToUInt32(provider);

    /// <inheritdoc />
    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)DateTime).ToUInt64(provider);

    #endregion

    #region IFormattable

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider formatProvider)
        => DateTime.ToString(format ?? StringExtensions.InteropDateTimeFormat, formatProvider ?? CultureInfo.CurrentCulture);

    #endregion
}
