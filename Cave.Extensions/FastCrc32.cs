using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Cave;

/// <summary>Provides a fast managed implementation of the Cyclic Redundancy Checksum with 32 bits without reflection.</summary>
[SuppressMessage("Usage", "CA2231", Justification = "Not equatable")]
public struct FastCrc32 : IHashingFunction, IChecksum<uint>
{
    #region Private Fields

    static readonly uint[] Bzip2Table = CRC32.BZIP2.Table;
    uint currentCRC = Initializer;

    #endregion Private Fields

    #region Private Methods

    [MethodImpl((MethodImplOptions)0x0100)]
    void HashCore(uint @byte)
    {
        var i = ((currentCRC >> 24) ^ @byte) & 0xFF;
        currentCRC = (currentCRC << 8) ^ Bzip2Table[i];
    }

    #endregion Private Methods

    #region Public Fields

    /// <summary>The initializer value.</summary>
    public const uint Initializer = 0xffffffff;

    /// <summary>
    /// Provides the default polynomial (*the* standard CRC-32 polynomial, first popularized by Ethernet)
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0 (little endian value).
    /// </summary>
    public const uint Polynomial = 0x04c11db7;

    #endregion Public Fields

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="FastCrc32"/> structure. This produces a checksum equivalent to <see cref="CRC32.BZIP2"/>.</summary>
    public FastCrc32() { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the lookup table.</summary>
    public static uint[] Table => (uint[])Bzip2Table.Clone();

    /// <summary>The name of the hash.</summary>
    public string Name => "CRC-32/BZIP2";

    /// <inheritdoc/>
    public uint Value
    {
        [MethodImpl((MethodImplOptions)0x0100)]
        get => ~currentCRC;
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Feed(byte[] data) => HashCore(data, 0, data.Length);

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public unsafe void Feed(byte* data, int length)
    {
        for (var i = 0; i < length; i++)
        {
            HashCore(data[i]);
        }
    }

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Feed(int hash)
    {
        var val = (uint)hash;
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
    }

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void HashCore(byte[] data, int offset, int length)
    {
        for (var i = 0; i < length; i++)
        {
            HashCore(data[offset++]);
        }
    }

    /// <inheritdoc/>
    public void Reset() => currentCRC = Initializer;

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public int ToHashCode() => (int)~currentCRC;

    /// <inheritdoc/>
    public void Update(int value) => HashCore((uint)value);

    /// <inheritdoc/>
    public void Update(byte[] buffer) => HashCore(buffer, 0, buffer.Length);

    /// <inheritdoc/>
    public void Update(byte[] buffer, int offset, int count) => HashCore(buffer, offset, count);

    #endregion Public Methods
}
