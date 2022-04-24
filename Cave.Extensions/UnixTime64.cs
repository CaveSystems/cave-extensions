using System;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Cave
{
    /// <summary>
    /// unix time stamp
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct UnixTime64 : IEquatable<UnixTime64>, IComparable<UnixTime64>
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

        /// <summary>Performs an implicit conversion from <see cref="uint"/> to <see cref="UnixTime64"/>.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UnixTime64(long value) => new() { TimeStamp = value };

        /// <summary>
        /// Parses a UnixTime64 previously converted to a string with ToString()
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UnixTime64 Parse(string value) => new()
        {
            DateTime = DateTime.ParseExact(value, StringExtensions.InterOpDateTimeFormat, CultureInfo.InvariantCulture),
        };

        /// <summary>Converts the specified date time.</summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static uint Convert(DateTime dateTime)
        {
            if (dateTime.Ticks == 0) return 0;
            return (uint)((dateTime.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond);
        }

        /// <summary>Converts the specified unix time stamp.</summary>
        /// <param name="timeStamp">The unix time stamp.</param>
        /// <param name="resultKind">Kind of the result.</param>
        /// <returns></returns>
        public static DateTime Convert(long timeStamp, DateTimeKind resultKind)
        {
            if (timeStamp == 0) return new DateTime(0, resultKind);
            return new DateTime(1970, 1, 1, 0, 0, 0, resultKind) + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);
        }

        /// <summary>Converts the specified unix time stamp.</summary>
        /// <param name="timeStamp">The unix time stamp.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public static DateTime ConvertToUTC(uint timeStamp, TimeSpan timeZone)
        {
            if (timeStamp == 0) return new DateTime(0, DateTimeKind.Unspecified);
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - timeZone + TimeSpan.FromTicks(TimeSpan.TicksPerSecond * timeStamp);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnixTime64"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="t">The unix time stamp.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DateTime(UnixTime64 t) => t.DateTime;

        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="UnixTime64"/>.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UnixTime64(DateTime dateTime)
        {
            return new UnixTime64()
            {
                DateTime = dateTime,
            };
        }

        /// <summary>The time stamp in seconds since 1.1.1970</summary>
        public long TimeStamp;

        /// <summary>Gets or sets the date time.</summary>
        /// <value>The date time.</value>
        public DateTime DateTime
        {
            get => Convert(TimeStamp, DateTimeKind.Unspecified);
            set => TimeStamp = Convert(value);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => DateTime.ToString(StringExtensions.InterOpDateTimeFormat);

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode() => TimeStamp.GetHashCode();

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is UnixTime64 time && Equals(time);

        /// <summary>Determines whether the specified <see cref="UnixTime64" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="UnixTime64" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="UnixTime64" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(UnixTime64 other) => TimeStamp.Equals(other.TimeStamp);

        /// <summary>Vergleicht das aktuelle Objekt mit einem anderen Objekt desselben Typs.</summary>
        /// <param name="other">Ein Objekt, das mit diesem Objekt verglichen werden soll.</param>
        /// <returns>
        /// Ein Wert, der die relative Reihenfolge der verglichenen Objekte angibt.Der Rückgabewert hat folgende Bedeutung:Wert Bedeutung Kleiner als 0 (null) Dieses Objekt ist kleiner als der <paramref name="other" />-Parameter.Zero Dieses Objekt ist gleich <paramref name="other" />. Größer als 0 (null) Dieses Objekt ist größer als <paramref name="other" />.
        /// </returns>
        public int CompareTo(UnixTime64 other) => TimeStamp.CompareTo(other.TimeStamp);
    }
}
