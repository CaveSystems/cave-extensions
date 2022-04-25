#if !NET5_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

#pragma warning disable CA2231

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
    /// <remarks>
    /// Index is used by the C# compiler to support the new index syntax
    /// <code>
    /// int[] someArray = new int[5] { 1, 2, 3, 4, 5 } ;
    /// int lastElement = someArray[^1]; // lastElement = 5
    /// </code>
    /// </remarks>
    public readonly struct Index : IEquatable<Index>
    {
        #region Private Fields

        readonly int value;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Construct an Index using a value and indicating if the index is from the start or from the end.
        /// </summary>
        /// <param name="value">The index value. it has to be zero or positive number.</param>
        /// <param name="fromEnd">Indicating if the index is from the start or from the end.</param>
        /// <remarks>
        /// If the Index constructed from the end, index value 1 means pointing at the last element and index value 0 means pointing at beyond last element.
        /// </remarks>
        public Index(int value, bool fromEnd)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
            }

            this.value = fromEnd ? ~value : value;
        }

        // The following private constructors mainly created for perf reason to avoid the checks
        private Index(int value) => this.value = value;

        /// <summary>
        /// Create an Index pointing at first element.
        /// </summary>
        public static Index Start => new(0);

        /// <summary>
        /// Create an Index pointing at beyond last element.
        /// </summary>
        public static Index End => new(~0);

        /// <summary>
        /// Create an Index from the start at the position indicated by the value.
        /// </summary>
        /// <param name="value">The index value from the start.</param>
        [MethodImpl(256)]
        public static Index FromStart(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
            }

            return new Index(value);
        }

        /// <summary>
        /// Create an Index from the end at the position indicated by the value.
        /// </summary>
        /// <param name="value">The index value from the end.</param>
        [MethodImpl(256)]
        public static Index FromEnd(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
            }

            return new Index(~value);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Indicates whether the index is from the start or the end.
        /// </summary>
        public bool IsFromEnd => value < 0;

        /// <summary>
        /// Returns the index value.
        /// </summary>
        public int Value => value < 0 ? ~value : value;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Converts integer number to an Index.
        /// </summary>
        public static implicit operator Index(int value) => FromStart(value);

        /// <summary>
        /// Calculate the offset from the start using the giving collection length.
        /// </summary>
        /// <param name="length">The length of the collection that the Index will be used with. length has to be a positive value</param>
        /// <remarks>
        /// For performance reason, we don't validate the input length parameter and the returned offset value against negative values. we don't validate either
        /// the returned offset is greater than the input length. It is expected Index will be used with collections which always have non negative
        /// length/count. If the returned offset is negative and then used to index a collection will get out of range exception which will be same affect as
        /// the validation.
        /// </remarks>
        [MethodImpl(256)]
        public int GetOffset(int length)
        {
            var offset = Value;
            if (IsFromEnd)
            {
                offset += length + 1;
            }
            return offset;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Index index && Equals(index);

        /// <inheritdoc/>
        public bool Equals(Index other) => value == other.value;

        /// <inheritdoc/>
        public override int GetHashCode() => value;

        /// <summary>
        /// Converts the value of the current Index object to its equivalent string representation.
        /// </summary>
        public override string ToString() => IsFromEnd ? '^' + Value.ToString() : ((uint)Value).ToString();

        #endregion Public Methods
    }
}

#endif
