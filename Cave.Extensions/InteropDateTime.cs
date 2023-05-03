using System;
using System.Globalization;

namespace Cave;

/// <summary>Provides access to a better precision date time that always advances in time.</summary>
public readonly struct InteropDateTime : IComparable, IComparable<InteropDateTime>, IEquatable<InteropDateTime>, IFormattable
{
    #region Static

    /// <summary>Parses a MonotonicDateTime previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static InteropDateTime Parse(string value) => Parse(value, null);

    /// <summary>Parses a MonotonicDateTime previously converted to a string with ToString()</summary>
    /// <param name="value"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static InteropDateTime Parse(string value, IFormatProvider provider) =>
        DateTimeOffset.TryParseExact(value, StringExtensions.InteropDateTimeFormat, provider ?? CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result)
            ? result
            : DateTimeOffset.ParseExact(value, StringExtensions.InteropDateTimeFormatWithoutTimeZone, provider ?? CultureInfo.InvariantCulture);

    /// <summary>Adds a <see cref="TimeSpan" /> to the <see cref="InteropDateTime" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static InteropDateTime operator +(InteropDateTime value1, TimeSpan value2) => new(value1.DateTimeOffset + value2);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset == value2.DateTimeOffset;

    /// <summary>Performs an implicit conversion from <see cref="InteropDateTime" /> to <see cref="DateTimeOffset" />.</summary>
    /// <param name="dateTime">The time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTimeOffset(InteropDateTime dateTime) => dateTime.DateTimeOffset;

    /// <summary>Performs an implicit conversion from <see cref="InteropDateTime" /> to <see cref="DateTime" />.</summary>
    /// <param name="dateTime">The time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator DateTime(InteropDateTime dateTime) => dateTime.DateTimeOffset.DateTime;

    /// <summary>Performs an implicit conversion from <see cref="InteropDateTime" /> to <see cref="UnixTime64" />.</summary>
    /// <param name="dateTime">The time stamp.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator UnixTime64(InteropDateTime dateTime) => UnixTime64.Convert((DateTime)dateTime);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset > value2.DateTimeOffset;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >=(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset >= value2.DateTimeOffset;

    /// <summary>Performs an implicit conversion from <see cref="DateTimeOffset" /> to <see cref="InteropDateTime" />.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator InteropDateTime(DateTimeOffset dateTime) => new(dateTime);

    /// <summary>Performs an implicit conversion from <see cref="DateTime" /> to <see cref="InteropDateTime" />.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator InteropDateTime(DateTime dateTime) => new(dateTime);

    /// <summary>Performs an implicit conversion from <see cref="UnixTime64" /> to <see cref="InteropDateTime" />.</summary>
    /// <param name="unixDateTime">The date time.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator InteropDateTime(UnixTime64 unixDateTime) => new(unixDateTime.DateTime);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset != value2.DateTimeOffset;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset < value2.DateTimeOffset;

    /// <summary>Implements the operator ==.</summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <=(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset <= value2.DateTimeOffset;

    /// <summary>Subtracts a <see cref="TimeSpan" /> from the <see cref="InteropDateTime" />.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static InteropDateTime operator -(InteropDateTime value1, TimeSpan value2) => new(value1.DateTimeOffset - value2);

    /// <summary>Subtracts two <see cref="InteropDateTime" /> values.</summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>The result of the calculation.</returns>
    public static TimeSpan operator -(InteropDateTime value1, InteropDateTime value2) => value1.DateTimeOffset - value2.DateTimeOffset;

    #endregion

    #region Fields

    /// <summary>Gets the <see cref="DateTimeOffset" />.</summary>
    public readonly DateTimeOffset DateTimeOffset;

    #endregion

    #region Constructors

    /// <summary>Initializes a new <see cref="InteropDateTime" /> instance.</summary>
    /// <param name="dateTimeOffset"></param>
    public InteropDateTime(DateTimeOffset dateTimeOffset) => DateTimeOffset = dateTimeOffset;

    #endregion

    #region Properties

    /// <summary>Gets the date component.</summary>
    public DateTime Date => DateTimeOffset.Date;

    /// <summary>Gets the the day component.</summary>
    public int Day => DateTimeOffset.Day;

    /// <summary>Gets the the day of week represented.</summary>
    public DayOfWeek DayOfWeek => DateTimeOffset.DayOfWeek;

    /// <summary>Gets the the hour component.</summary>
    public int Hour => DateTimeOffset.Hour;

    /// <summary>Gets the the millisecond component.</summary>
    public int Millisecond => DateTimeOffset.Millisecond;

    /// <summary>Gets the the minute component.</summary>
    public int Minute => DateTimeOffset.Minute;

    /// <summary>Gets the month component.</summary>
    public int Month => DateTimeOffset.Month;

    /// <summary>Gets the the second component.</summary>
    public int Second => DateTimeOffset.Second;

    /// <summary>Gets the time of day component.</summary>
    public TimeSpan TimeOfDay => DateTimeOffset.TimeOfDay;

    /// <summary>Gets the year component.</summary>
    public int Year => DateTimeOffset.Year;

    #endregion

    #region IComparable Members

    /// <inheritdoc />
    public int CompareTo(object obj) => obj is InteropDateTime mdt ? CompareTo(mdt) : 1;

    #endregion

    #region IComparable<InteropDateTime> Members

    /// <inheritdoc />
    public int CompareTo(InteropDateTime other) => DateTimeOffset.CompareTo(other.DateTimeOffset);

    #endregion

    #region IEquatable<InteropDateTime> Members

    /// <inheritdoc />
    public bool Equals(InteropDateTime other) => DateTimeOffset.Equals(other.DateTimeOffset);

    #endregion

    #region IFormattable Members

    /// <inheritdoc />
    public string ToString(string format, IFormatProvider formatProvider) => DateTimeOffset.ToString(format ?? StringExtensions.InteropDateTimeFormat, formatProvider ?? CultureInfo.CurrentCulture);

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is InteropDateTime time && Equals(time);

    /// <inheritdoc />
    public override int GetHashCode() => DateTimeOffset.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => ToString(null, null);

    #endregion
}
