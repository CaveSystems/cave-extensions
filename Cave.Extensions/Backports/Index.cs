#pragma warning disable IDE0130
#pragma warning disable CS1591
#pragma warning disable CA2231
#if (NET20_OR_GREATER && !NET5_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP3_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER)

using System.Runtime.CompilerServices;

namespace System;

public readonly struct Index : IEquatable<Index>
{
    #region Private Fields

    readonly int value;

    #endregion Private Fields

    #region Public Constructors

    public Index(int value, bool fromEnd)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
        }

        this.value = fromEnd ? ~value : value;
    }

    Index(int value) => this.value = value;

    public static Index Start => new(0);

    public static Index End => new(~0);

    [MethodImpl((MethodImplOptions)256)]
    public static Index FromStart(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
        }

        return new(value);
    }

    [MethodImpl((MethodImplOptions)256)]
    public static Index FromEnd(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Index must not be negative.");
        }

        return new(~value);
    }

    #endregion Public Constructors

    #region Public Properties

    public bool IsFromEnd => value < 0;

    public int Value => value < 0 ? ~value : value;

    #endregion Public Properties

    #region Public Methods

    public static implicit operator Index(int value) => FromStart(value);

    [MethodImpl((MethodImplOptions)256)]
    public int GetOffset(int length)
    {
        var offset = value;
        if (IsFromEnd)
        {
            offset += length + 1;
        }
        return offset;
    }

    public override bool Equals(object obj) => obj is Index index && Equals(index);

    public bool Equals(Index other) => value == other.value;

    public override int GetHashCode() => value;

    public override string ToString() => IsFromEnd ? '^' + Value.ToString() : ((uint)Value).ToString();

    #endregion Public Methods
}

#endif
