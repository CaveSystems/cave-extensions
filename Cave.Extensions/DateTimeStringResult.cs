using System;

namespace Cave
{
    /// <summary>Gets a date time string parser result containing the <see cref="SubStringResult" /> for date and time.</summary>
    public struct DateTimeStringResult : IEquatable<DateTimeStringResult>
    {
        /// <summary>Implements the operator ==.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DateTimeStringResult value1, DateTimeStringResult value2) => Equals(value1.Time, value2.Time) && Equals(value1.Date, value2.Date);

        /// <summary>Implements the operator !=.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DateTimeStringResult value1, DateTimeStringResult value2) => !Equals(value1.Time, value2.Time) || !Equals(value1.Date, value2.Date);

        /// <summary>Gets or sets Time SubStringResult.</summary>
        public SubStringResult Time;

        /// <summary>Gets or sets Date SubStringResult.</summary>
        public SubStringResult Date;

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is DateTimeStringResult result && base.Equals(result);

        /// <summary>Determines whether the specified <see cref="DateTimeStringResult" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="DateTimeStringResult" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTimeStringResult" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(DateTimeStringResult other) => (other.Time == Time) && (other.Date == Date);
    }
}
