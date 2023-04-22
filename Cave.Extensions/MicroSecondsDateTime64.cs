using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Cave;

/// <summary>Micro seconds time stamp</summary>
[StructLayout(LayoutKind.Sequential, Size = 8)]
public struct MicroSecondsDateTime64 : IEquatable<MicroSecondsDateTime64>, IComparable<MicroSecondsDateTime64>, IFormattable, IConvertible
{
    /// <summary>Gets the number of .net ticks per microsecond.</summary>
    public const long TicksPerMicroSecond = TimeSpan.TicksPerMillisecond * 1000L;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds == value2.MicroSeconds;

    /// <summary>Implements the operator !=.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds != value2.MicroSeconds;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds > value2.MicroSeconds;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds >= value2.MicroSeconds;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds < value2.MicroSeconds;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => value1.MicroSeconds <= value2.MicroSeconds;

    /// <summary>Performs an implicit conversion from <see cref="ulong" /> to <see cref="MicroSecondsDateTime64" />.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator MicroSecondsDateTime64(long value) => new() { MicroSeconds = value };

    /// <summary>Adds a <see cref="TimeSpan" /> to the <see cref="MicroSecondsDateTime64" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static MicroSecondsDateTime64 operator +(MicroSecondsDateTime64 value1, TimeSpan value2) => new() { MicroSeconds = value1.MicroSeconds + (value2.Ticks / TicksPerMicroSecond) };

    /// <summary>Substracts a <see cref="TimeSpan" /> from the <see cref="MicroSecondsDateTime64" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static MicroSecondsDateTime64 operator -(MicroSecondsDateTime64 value1, TimeSpan value2) => new() { MicroSeconds = value1.MicroSeconds - (value2.Ticks / TicksPerMicroSecond) };

    /// <summary>Substracts two <see cref="MicroSecondsDateTime64" /> values.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static TimeSpan operator -(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => new((value1.MicroSeconds - value2.MicroSeconds) * (TimeSpan.TicksPerSecond / TicksPerMicroSecond));

    /// <summary>Parses a MicroSecondsDateTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static MicroSecondsDateTime64 Parse(string value) => new()
    {
        DateTime = DateTime.ParseExact(value, StringExtensions.InterOpDateTimeFormat, CultureInfo.InvariantCulture)
    };

    /// <summary>Parses a MicroSecondsDateTime64 previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static MicroSecondsDateTime64 Parse(string value, IFormatProvider provider) => new()
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
        return dateTime.Ticks / TimeSpan.TicksPerSecond;
    }

    /// <summary>Converts the specified unix time stamp.</summary>
    /// <param name="microseconds">The time stamp.</param>
    /// <param name="resultKind">Kind of the result.</param>
    /// <returns></returns>
    public static DateTime Convert(long microseconds, DateTimeKind resultKind)
    {
        if (microseconds == 0)
        {
            return new(0, resultKind);
        }
        return new(TicksPerMicroSecond * microseconds, resultKind);
    }

    /// <summary>Converts the specified time stamp.</summary>
    /// <param name="microSeconds">The time stamp.</param>
    /// <param name="timeZone">The time zone.</param>
    /// <returns></returns>
    public static DateTime ConvertToUTC(long microSeconds, TimeSpan timeZone)
    {
        if (microSeconds == 0)
        {
            return new(0, DateTimeKind.Unspecified);
        }
        return (Convert(microSeconds, DateTimeKind.Local) - timeZone).ToUniversalTime();
    }

    /// <summary>Performs an implicit conversion from <see cref="UnixTime64" /> to <see cref="DateTime" />.</summary>
    /// <param name="t">The unix time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(MicroSecondsDateTime64 t) => t.DateTime;

    /// <summary>Performs an implicit conversion from <see cref="DateTime" /> to <see cref="UnixTime64" />.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator MicroSecondsDateTime64(DateTime dateTime) =>
        new()
        {
            DateTime = dateTime
        };

    /// <summary>The time stamp in micro seconds</summary>
    public long MicroSeconds;

    /// <summary>Gets or sets the date time.</summary>
    /// <value>The date time.</value>
    public DateTime DateTime
    {
        get => Convert(MicroSeconds, DateTimeKind.Unspecified);
        set => MicroSeconds = Convert(value);
    }

    /// <inheritdoc />
    public override string ToString() => DateTime.ToString(StringExtensions.InterOpDateTimeFormat);

    /// <inheritdoc />
    public override int GetHashCode() => MicroSeconds.GetHashCode();

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is MicroSecondsDateTime64 time && Equals(time);

    /// <inheritdoc />
    public bool Equals(MicroSecondsDateTime64 other) => MicroSeconds.Equals(other.MicroSeconds);

    /// <inheritdoc />
    public int CompareTo(MicroSecondsDateTime64 other) => MicroSeconds.CompareTo(other.MicroSeconds);

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
