using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>unix time stamp in seconds since epoch</summary>
[StructLayout(LayoutKind.Sequential, Size = 8)]
public struct UnixTime64 : IEquatable<UnixTime64>, IComparable<UnixTime64>, IFormattable, IConvertible
{
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

    /// <summary>Performs an implicit conversion from <see cref="uint" /> to <see cref="UnixTime64" />.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime64(long value) => new() { TimeStamp = value };

    /// <summary>Adds a <see cref="TimeSpan" /> to the <see cref="UnixTime64" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime64 operator +(UnixTime64 value1, TimeSpan value2) => new() { TimeStamp = value1.TimeStamp + (value2.Ticks / TimeSpan.TicksPerSecond) };

    /// <summary>Substracts a <see cref="TimeSpan" /> from the <see cref="MicroSecondsDateTime64" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static UnixTime64 operator -(UnixTime64 value1, TimeSpan value2) => new() { TimeStamp = value1.TimeStamp - (value2.Ticks / TimeSpan.TicksPerSecond) };

    /// <summary>Substracts two <see cref="UnixTime64" /> values.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static TimeSpan operator -(UnixTime64 value1, UnixTime64 value2) => new((value1.TimeStamp - value2.TimeStamp) * TimeSpan.TicksPerSecond);

    /// <summary>Parses a UnixTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static UnixTime64 Parse(string value) => new()
    {
        DateTime = DateTime.ParseExact(value, StringExtensions.InterOpDateTimeFormat, CultureInfo.InvariantCulture)
    };

    /// <summary>Parses a UnixTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static UnixTime64 Parse(string value, IFormatProvider provider) => new()
    {
        DateTime = DateTime.ParseExact(value, StringExtensions.InterOpDateTimeFormat, provider)
    };

    /// <summary>Converts the specified date time.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static long Convert(DateTime dateTime)
    {
        if (dateTime.Ticks == 0)
        {
            return 0;
        }
        return (dateTime.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond;
    }

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="timeStamp">The unix time stamp.</param>
    /// <param name="resultKind">Kind of the result.</param>
    /// <returns></returns>
    public static DateTime Convert(long timeStamp, DateTimeKind resultKind)
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
    public static DateTime ConvertToUTC(long timeStamp, TimeSpan timeZone)
    {
        if (timeStamp == 0)
        {
            return new(0, DateTimeKind.Unspecified);
        }
        return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - timeZone) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);
    }

    /// <summary>Performs an implicit conversion from <see cref="UnixTime64" /> to <see cref="DateTime" />.</summary>
    /// <param name="t">The unix time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(UnixTime64 t) => t.DateTime;

    /// <summary>Performs an implicit conversion from <see cref="DateTime" /> to <see cref="UnixTime64" />.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UnixTime64(DateTime dateTime) =>
        new()
        {
            DateTime = dateTime
        };

    /// <summary>The time stamp in seconds since 1.1.1970</summary>
    public long TimeStamp;

    /// <summary>Gets or sets the date time.</summary>
    /// <value>The date time.</value>
    public DateTime DateTime
    {
        get => Convert(TimeStamp, DateTimeKind.Unspecified);
        set => TimeStamp = Convert(value);
    }

    /// <inheritdoc />
    public override string ToString() => DateTime.ToString(StringExtensions.InterOpDateTimeFormat);

    /// <inheritdoc />
    public override int GetHashCode() => TimeStamp.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UnixTime64 time && Equals(time);

    /// <inheritdoc />
    public bool Equals(UnixTime64 other) => TimeStamp.Equals(other.TimeStamp);

    /// <inheritdoc />
    public int CompareTo(UnixTime64 other) => TimeStamp.CompareTo(other.TimeStamp);

    #region IConvertible

    /// <inheritdoc />
    public TypeCode GetTypeCode() => DateTime.GetTypeCode();

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
    public string ToString(string format, IFormatProvider formatProvider) => DateTime.ToString(format, formatProvider);

    #endregion
}
