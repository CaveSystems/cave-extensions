#if !NET5_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

#pragma warning disable CA2231

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    /// <summary>
    /// Represent a range has start and end indexes.
    /// </summary>
    /// <remarks>
    /// Range is used by the C# compiler to support the range syntax.
    /// <code>
    ///int[] someArray = new int[5] { 1, 2, 3, 4, 5 };
    ///int[] subArray1 = someArray[0..2]; // { 1, 2 }
    ///int[] subArray2 = someArray[1..^0]; // { 2, 3, 4, 5 }
    /// </code>
    /// </remarks>
    public readonly struct Range : IEquatable<Range>
    {
        #region Private Constructors

        /// <summary>
        /// Construct a Range object using the start and end indexes.
        /// </summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Represent the inclusive start index of the Range.
        /// </summary>
        public Index Start { get; }

        /// <summary>
        /// Represent the exclusive end index of the Range.
        /// </summary>
        public Index End { get; }

        #endregion Public Properties

        #region Public Methods

        /// <inheritdoc/>
        public override bool Equals(object obj) => (obj is Range range) && Equals(range);

        /// <inheritdoc/>
        public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

        /// <inheritdoc/>
        public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Start + ".." + End;

        #endregion Public Methods

        /// <summary>
        /// Create a Range object starting from start index to the end of the collection.
        /// </summary>
        public static Range StartAt(Index start) => new(start, Index.End);

        /// <summary>
        /// Create a Range object starting from first element in the collection to the end Index.
        /// </summary>
        public static Range EndAt(Index end) => new(Index.Start, end);

        /// <summary>
        /// Create a Range object starting from first element to the end.
        /// </summary>
        public static Range All => new(Index.Start, Index.End);
    }
}

#endif
