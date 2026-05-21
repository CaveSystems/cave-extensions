using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Cave;

/// <summary>Provides a fast managed implementation of the Cyclic Redundancy Checksum with 32 bits without reflection.</summary>
[SuppressMessage("Usage", "CA2231", Justification = "Not equatable")]
public struct FastCrc32 : IHashingFunction, IChecksum<uint>
{
    #region Private Fields

    static readonly uint[] bzip2Table = CRC32.BZIP2.Table;
    uint currentCRC = Initializer;

    #endregion Private Fields

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

    /// <summary>Initializes a new instance of the <see cref="FastCrc32"/> struct.</summary>
    /// <remarks>This produces a checksum equivalent to <see cref="CRC32.BZIP2"/>.</remarks>
    public FastCrc32() { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the lookup table.</summary>
    public static uint[] Table => (uint[])bzip2Table.Clone();

    /// <summary>Gets the name of the hash.</summary>
    public string Name => "CRC-32/BZIP2";

    /// <inheritdoc/>
    public uint Value
    {
        [MethodImpl((MethodImplOptions)0x0100)]
        get => ~currentCRC;
    }

    #endregion Public Properties

    #region Public Methods

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Feed(byte[] data) => HashCore(data, 0, data.Length);

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public unsafe void Feed(byte* data, int length)
    {
        var end = data + length;
        while (data + 4 <= end)
        {
            var c = currentCRC;
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[0]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[1]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[2]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[3]) & 0xFF];
            currentCRC = c;
            data += 4;
        }

        while (data < end)
        {
            var c = currentCRC;
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ *data) & 0xFF];
            currentCRC = c;
            data++;
        }
    }

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Feed(int value)
    {
        var c = currentCRC;
        c = (c << 8) ^ bzip2Table[((c >> 24) ^ (value & 0xFF)) & 0xFF];
        c = (c << 8) ^ bzip2Table[((c >> 24) ^ ((value >> 8) & 0xFF)) & 0xFF];
        c = (c << 8) ^ bzip2Table[((c >> 24) ^ ((value >> 16) & 0xFF)) & 0xFF];
        c = (c << 8) ^ bzip2Table[((c >> 24) ^ ((value >> 24) & 0xFF)) & 0xFF];
        currentCRC = c;
    }

    /// <summary>Returns the same value <see cref="ToHashCode"/> returns.</summary>
    /// <returns>The computed hash code.</returns>
    public override int GetHashCode() => ToHashCode();

    /// <summary>
    /// Processes a specified region of the input byte array and updates the hash state with the provided data segment.
    /// </summary>
    /// <param name="data">The input byte array containing the data to be hashed.</param>
    /// <param name="offset">The zero-based index in the array at which to begin reading data.</param>
    /// <param name="length">The number of bytes to process from the input array, starting at the specified offset.</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void HashCore(byte[] data, int offset, int length)
    {
        var end = offset + length;
        while (offset + 4 <= end)
        {
            var c = currentCRC;
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[offset]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[offset + 1]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[offset + 2]) & 0xFF];
            c = (c << 8) ^ bzip2Table[((c >> 24) ^ data[offset + 3]) & 0xFF];
            currentCRC = c;
            offset += 4;
        }
        while (offset < end)
        {
            var i = ((currentCRC >> 24) ^ data[offset]) & 0xFF;
            currentCRC = (currentCRC << 8) ^ bzip2Table[i];
            offset++;
        }
    }

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Reset() => currentCRC = Initializer;

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public int ToHashCode() => (int)~currentCRC;

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Update(int value)
    {
        var i = ((currentCRC >> 24) ^ value) & 0xFF;
        currentCRC = (currentCRC << 8) ^ bzip2Table[i];
    }

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Update(byte[] buffer) => HashCore(buffer, 0, buffer.Length);

    /// <inheritdoc/>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Update(byte[] buffer, int offset, int count) => HashCore(buffer, offset, count);

    #endregion Public Methods
}
