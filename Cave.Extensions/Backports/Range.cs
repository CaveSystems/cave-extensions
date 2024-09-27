#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CA2231

#if (NET20_OR_GREATER && !NET5_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP3_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER)

// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly struct Range : IEquatable<Range>
{
    #region Private Constructors

    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    #endregion Private Constructors

    #region Public Properties

    public Index Start { get; }

    public Index End { get; }

    #endregion Public Properties

    #region Public Methods

    public override bool Equals(object? obj) => obj is Range range && Equals(range);

    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    public override int GetHashCode() => Start.GetHashCode() ^ End.GetHashCode();

    public override string ToString() => Start + ".." + End;

    #endregion Public Methods

    public static Range StartAt(Index start) => new(start, Index.End);

    public static Range EndAt(Index end) => new(Index.Start, end);

    public static Range All => new(Index.Start, Index.End);
}

#endif
