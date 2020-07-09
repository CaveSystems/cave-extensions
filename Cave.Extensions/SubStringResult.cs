using System;

namespace Cave
{
    /// <summary>
    /// Provides a string search result containing start index an length.
    /// </summary>
    public struct SubStringResult : IEquatable<SubStringResult>
    {
        /// <summary>
        /// Checks two SubStringResult instances for equality.
        /// </summary>
        /// <param name="subStringResult1">The first substringResult.</param>
        /// <param name="subStringResult2">The second substringResult.</param>
        /// <returns>True if the substringResults are equal.</returns>
        public static bool operator ==(SubStringResult subStringResult1, SubStringResult subStringResult2) => subStringResult1.Equals(subStringResult2);

        /// <summary>
        /// Checks two SubStringResult instances for inequality.
        /// </summary>
        /// <param name="subStringResult1">The first substringResult.</param>
        /// <param name="subStringResult2">The second substringResult.</param>
        /// <returns>True if the substringResults are not equal.</returns>
        public static bool operator !=(SubStringResult subStringResult1, SubStringResult subStringResult2) => !subStringResult1.Equals(subStringResult2);

        /// <summary>
        /// Searches for a sub string at the specified string.
        /// </summary>
        /// <param name="text">Full string.</param>
        /// <param name="value">Substring to be searched.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The substringResult.</returns>
        public static SubStringResult Find(string text, string value, StringComparison stringComparison = default)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var index = text.IndexOf(value, stringComparison);
            if (index < 0)
            {
                return default;
            }

            return new SubStringResult(text, index, value.Length);
        }

        /// <summary>
        /// Searches for a sub string at the specified string.
        /// </summary>
        /// <param name="text">Full string.</param>
        /// <param name="value">Substring to be searched.</param>
        /// <param name="startIndex">Startindex to begin searching at.</param>
        /// <param name="stringComparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>The substringResult.</returns>
        public static SubStringResult Find(string text, string value, int startIndex, StringComparison stringComparison = default)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var index = text.IndexOf(value, startIndex, stringComparison);
            return new SubStringResult(text, index, index < 0 ? 0 : value.Length);
        }

        /// <summary>
        /// Creates a new instance from the specified start and end indices.
        /// </summary>
        /// <param name="text">Full string.</param>
        /// <param name="startIndex">start index.</param>
        /// <param name="endIndex">end index.</param>
        /// <returns>The substringResult.</returns>
        public static SubStringResult FromIndices(string text, int startIndex, int endIndex)
        {
            return new SubStringResult(text, startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubStringResult"/> struct.
        /// </summary>
        /// <param name="text">The String contining the Substring.</param>
        /// <param name="index">Start index of the substring.</param>
        /// <param name="count">Length of the substring.</param>
        public SubStringResult(string text, int index, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            Index = index;
            Length = count;
            EndIndex = Index + Length;
            Text = text.Substring(Index, Length);
        }

        /// <summary>
        /// Checks whether an index is part of this substring or not.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>True if the index is contained.</returns>
        public bool Contains(int index) => (index >= Index) && (index < EndIndex);

        /// <summary>
        /// Gets a value indicating whether the substring is valid or not.
        /// </summary>
        public bool Valid => Length > 0;

        /// <summary>
        /// Substring Text.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Index of the sub string.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Length of the sub string.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Endindex (Index+Length).
        /// </summary>
        public readonly int EndIndex;

        /// <summary>
        /// Gets a string "[Length] Start..End 'Text'.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString() => Index < 0 ? "Invalid" : ("[" + Length + "] " + Index + ".." + EndIndex + " = '" + Text + "'");

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Index ^ Length ^ Text.GetHashCode();
        }

        /// <summary>
        /// Checks this instance with another one.
        /// </summary>
        /// <param name="obj">The other SubStringResult.</param>
        /// <returns>True if both instances are equal.</returns>
        public override bool Equals(object obj) => obj is SubStringResult other && Equals(other);

        /// <summary>
        /// Checks this instance with another one.
        /// </summary>
        /// <param name="other">The other SubStringResult.</param>
        /// <returns>True if both instances are equal.</returns>
        public bool Equals(SubStringResult other) => (other.Index == Index) && (other.Length == Length) && (other.Text == Text);
    }
}
