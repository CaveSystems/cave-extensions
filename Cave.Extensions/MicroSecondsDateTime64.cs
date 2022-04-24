using System;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Cave
{
    /// <summary>
    /// Micro seconds time stamp
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct MicroSecondsDateTime64 : IEquatable<MicroSecondsDateTime64>, IComparable<MicroSecondsDateTime64>
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

        /// <summary>Performs an implicit conversion from <see cref="ulong"/> to <see cref="MicroSecondsDateTime64"/>.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator MicroSecondsDateTime64(long value) => new() { MicroSeconds = value };

        /// <summary>Adds a <see cref="TimeSpan"/> to the <see cref="MicroSecondsDateTime64"/>.</summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>The result of the calculation.</returns>
        public static MicroSecondsDateTime64 operator +(MicroSecondsDateTime64 value1, TimeSpan value2) => new() { MicroSeconds = value1.MicroSeconds + (value2.Ticks / TicksPerMicroSecond) };

        /// <summary>Substracts a <see cref="TimeSpan"/> from the <see cref="MicroSecondsDateTime64"/>.</summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>The result of the calculation.</returns>
        public static MicroSecondsDateTime64 operator -(MicroSecondsDateTime64 value1, TimeSpan value2) => new() { MicroSeconds = value1.MicroSeconds - (value2.Ticks / TicksPerMicroSecond) };

        /// <summary>Substracts two <see cref="MicroSecondsDateTime64"/> values.</summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>The result of the calculation.</returns>
        public static TimeSpan operator -(MicroSecondsDateTime64 value1, MicroSecondsDateTime64 value2) => new((value1.MicroSeconds - value2.MicroSeconds) * (TimeSpan.TicksPerSecond / TicksPerMicroSecond));

        /// <summary>
        /// Parses a MicroSecondsDateTime64 previously converted to a string with ToString()
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MicroSecondsDateTime64 Parse(string value) => new()
        {
            DateTime = DateTime.ParseExact(value, StringExtensions.InterOpDateTimeFormat, CultureInfo.InvariantCulture),
        };

        /// <summary>Converts the specified date time.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static long Convert(DateTime dateTime)
        {
            if (dateTime.Ticks == 0) return 0;
            return dateTime.Ticks / TimeSpan.TicksPerSecond;
        }

        /// <summary>Converts the specified unix time stamp.</summary>
        /// <param name="microseconds">The time stamp.</param>
        /// <param name="resultKind">Kind of the result.</param>
        /// <returns></returns>
        public static DateTime Convert(long microseconds, DateTimeKind resultKind)
        {
            if (microseconds == 0) return new DateTime(0, resultKind);
            return new DateTime(TicksPerMicroSecond * microseconds, resultKind);
        }

        /// <summary>Converts the specified time stamp.</summary>
        /// <param name="microSeconds">The time stamp.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public static DateTime ConvertToUTC(long microSeconds, TimeSpan timeZone)
        {
            if (microSeconds == 0) return new DateTime(0, DateTimeKind.Unspecified);
            return Convert(microSeconds, DateTimeKind.Local).ToUniversalTime();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnixTime64"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="t">The unix time stamp.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime(MicroSecondsDateTime64 t) => t.DateTime;

        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="UnixTime64"/>.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator MicroSecondsDateTime64(DateTime dateTime)
        {
            return new MicroSecondsDateTime64()
            {
                DateTime = dateTime,
            };
        }

        /// <summary>The time stamp in micro seconds</summary>
        public long MicroSeconds;

        /// <summary>Gets or sets the date time.</summary>
        /// <value>The date time.</value>
        public DateTime DateTime
        {
            get => Convert(MicroSeconds, DateTimeKind.Unspecified);
            set => MicroSeconds = Convert(value);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => DateTime.ToString(StringExtensions.InterOpDateTimeFormat);

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode() => MicroSeconds.GetHashCode();

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is MicroSecondsDateTime64 time && Equals(time);

        /// <summary>Determines whether the specified <see cref="UnixTime64" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="MicroSecondsDateTime64" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="MicroSecondsDateTime64" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(MicroSecondsDateTime64 other) => MicroSeconds.Equals(other.MicroSeconds);

        /// <summary>Vergleicht das aktuelle Objekt mit einem anderen Objekt desselben Typs.</summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns>
        /// Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt.Der Rückgabewert hat folgende Bedeutung:Wert Bedeutung Kleiner als 0 (null) Dieses Objekt ist kleiner als der <paramref name="other" />-Parameter.Zero Dieses Objekt ist gleich <paramref name="other" />. Größer als 0 (null) Dieses Objekt ist größer als <paramref name="other" />.
        /// </returns>
        public int CompareTo(MicroSecondsDateTime64 other) => MicroSeconds.CompareTo(other.MicroSeconds);
    }
}
