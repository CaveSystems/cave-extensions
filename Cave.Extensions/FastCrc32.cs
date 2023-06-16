using System.Runtime.CompilerServices;

#pragma warning disable CS0162

namespace Cave.CodeGen;

/// <summary>Provides a fast managed implementation of the Cyclic Redundancy Checksum with 32 bits without reflection.</summary>
public struct FastCrc32 : IHashingFunction, IChecksum<uint>
{
    #region Static

    /// <summary>The initializer value.</summary>
    public const uint Initializer = 0xffffffff;

    /// <summary>The name of the hash.</summary>
    public string Name => "CRC-32/BZIP2";

    /// <summary>
    /// Provides the default polynomial (*the* standard CRC-32 polynomial, first popularized by Ethernet)
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0 (little endian value).
    /// </summary>
    public const uint Polynomial = 0x04c11db7;

    static readonly uint[] table = CRC32.BZIP2.Table;

    #endregion

    /// <summary>
    /// Creates a new instance of the <see cref="FastCrc32" /> structure. This produces a checksum equivalent to
    /// <see cref="CRC32.BZIP2" />.
    /// </summary>
    public FastCrc32() { }

    #region Properties

    /// <summary>Gets the lookup table.</summary>
    public static uint[] Table => (uint[])table.Clone();

    /// <inheritdoc />
    public uint Value
    {
        [MethodImpl((MethodImplOptions)0x0100)]
        get => ~currentCRC;
    }

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public int ToHashCode() => (int)~currentCRC;

    #endregion

    #region Members

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Feed(byte[] data) => HashCore(data, 0, data.Length);

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public unsafe void Feed(byte* data, int length)
    {
        for (var i = 0; i < length; i++)
        {
            HashCore(data[i]);
        }
    }

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Add<T>(T item)
    {
        var val = (uint)(item?.GetHashCode() ?? 0);
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
        val >>= 8;
        HashCore(val & 0xFF);
    }

    [MethodImpl((MethodImplOptions)0x0100)]
    void HashCore(uint @byte)
    {
        var i = ((currentCRC >> 24) ^ @byte) & 0xFF;
        currentCRC = (currentCRC << 8) ^ table[i];
    }

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public void HashCore(byte[] data, int offset, int length)
    {
        for (var i = 0; i < length; i++)
        {
            HashCore(data[offset++]);
        }
    }

    /// <inheritdoc />
    public void Reset() => currentCRC = Initializer;

    /// <inheritdoc />
    public void Update(int value) => HashCore((uint)value);

    /// <inheritdoc />
    public void Update(byte[] buffer) => HashCore(buffer, 0, buffer.Length);

    /// <inheritdoc />
    public void Update(byte[] buffer, int offset, int count) => HashCore(buffer, offset, count);

    #endregion

    #region private funtionality

    uint currentCRC = Initializer;

    #endregion private funtionality
}
