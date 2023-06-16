using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Cave;

/// <summary>Provides a managed implementation of the Cyclic Redundancy Checksum with 32 bits.</summary>
public class CRC32 : HashAlgorithm, IChecksum<uint>, IHashingFunction
{
    #region Static

    /// <summary>
    /// Provides the default polynomial (*the* standard CRC-32 polynomial, first popularized by Ethernet)
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0 (little endian value).
    /// </summary>
    public static readonly uint DefaultPolynomial = 0x04c11db7;

    /// <summary>Reflects 32 bits.</summary>
    /// <param name="x">The bits.</param>
    /// <returns>Returns a center reflection.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint Reflect32(uint x)
    {
        // move bits
        x = ((x & 0x55555555) << 1) | ((x >> 1) & 0x55555555);
        x = ((x & 0x33333333) << 2) | ((x >> 2) & 0x33333333);
        x = ((x & 0x0F0F0F0F) << 4) | ((x >> 4) & 0x0F0F0F0F);

        // move bytes
        x = (x << 24) | ((x & 0xFF00) << 8) | ((x >> 8) & 0xFF00) | (x >> 24);
        return x;
    }

    /// <summary>
    /// Gets width=32 poly=0xf4acfb13 init=0xffffffff refin=true refout=true xorout=0xffffffff check=0x1697d06a residue=0x904cddbf
    /// name="CRC-32/AUTOSAR".
    /// </summary>
    public static CRC32 AUTOSAR => new(0xf4acfb13, 0xFFFFFFFF, finalXor: 0xffffffff, reflectInput: true, reflectOutput: true, name: "CRC-32/AUTOSAR");

    /// <summary>
    /// Gets width=32 poly=0x04c11db7 init=0xffffffff refin=false refout=false xorout=0xffffffff check=0xfc891918 residue=0xc704dd7b
    /// name="CRC-32/BZIP2".
    /// </summary>
    public static CRC32 BZIP2 => new(DefaultPolynomial, 0xFFFFFFFF, finalXor: 0, reflectInput: false, reflectOutput: false, name: "CRC-32/BZIP2");

    /// <summary>
    /// Gets width=32 poly=0x1edc6f41 init=0xffffffff refin=true refout=true xorout=0xffffffff check=0xe3069283 residue=0xb798b438
    /// name="CRC-32C".
    /// </summary>
    public static CRC32 C => new(0x1edc6f41, 0xFFFFFFFF, finalXor: 0xFFFFFFFF, reflectInput: true, reflectOutput: true, name: "CRC-32C");

    /// <summary>Gets alias for <see cref="POSIX" />.</summary>
    public static CRC32 CKSUM => POSIX;

    /// <summary>
    /// Gets width=32 poly=0xa833982b init=0xffffffff refin=true refout=true xorout=0xffffffff check=0x87315576 residue=0x45270551
    /// name="CRC-32D".
    /// </summary>
    public static CRC32 D => new(0xa833982b, 0xFFFFFFFF, finalXor: 0xFFFFFFFF, reflectInput: true, reflectOutput: true, name: "CRC-32D");

    /// <summary>
    /// Gets width=32 poly=0x04c11db7 init=0xffffffff refin=true refout=true xorout=0xffffffff check=0xcbf43926 residue=0xdebb20e3
    /// name="CRC-32".
    /// </summary>
    public static CRC32 Default => new(DefaultPolynomial, 0xFFFFFFFF, finalXor: 0xFFFFFFFF, reflectInput: true, reflectOutput: true, name: "CRC-32");

    /// <summary>
    /// Gets width=32 poly=0x04c11db7 init=0xffffffff refin=false refout=false xorout=0x00000000 check=0x0376e6e7 residue=0x00000000
    /// name="CRC-32/MPEG-2".
    /// </summary>
    public static CRC32 MPEG2 => new(DefaultPolynomial, 0xFFFFFFFF, finalXor: 0xFFFFFFFF, reflectInput: false, reflectOutput: false, name: "CRC-32/MPEG-2");

    /// <summary>
    /// Gets width=32 poly=0x04c11db7 init=0x00000000 refin=false refout=false xorout=0xffffffff check=0x765e7680 residue=0xc704dd7b
    /// name="CRC-32/POSIX".
    /// </summary>
    public static CRC32 POSIX => new(DefaultPolynomial, 0x00000000, finalXor: 0xFFFFFFFF, reflectInput: false, reflectOutput: false, name: "CRC-32/POSIX");

    /// <summary>
    /// Gets width=32 poly=0x814141ab init=0x00000000 refin=false refout=false xorout=0x00000000 check=0x3010bf7f residue=0x00000000
    /// name="CRC-32Q".
    /// </summary>
    public static CRC32 Q => new(0x814141ab, 0x00000000, finalXor: 0x00000000, reflectInput: false, reflectOutput: false, name: "CRC-32Q");

    #endregion

    #region Fields

    /// <summary>The polynomial used to generate the table.</summary>
    public readonly uint Polynomial;

    /// <summary>The initializer value.</summary>
    public readonly uint Initializer;

    /// <summary>The final xor value.</summary>
    public readonly uint FinalXor;

    /// <summary>The reflect input flag.</summary>
    public readonly bool ReflectInput;

    /// <summary>The reflect output flag.</summary>
    public readonly bool ReflectOutput;

    /// <summary>The name of the hash.</summary>
    public readonly string Name;

    uint currentCRC;
    uint[] table;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CRC32" /> class.</summary>
    /// <param name="blueprint">The blueprint to copy all properties from.</param>
    /// <exception cref="NotImplementedException">Throws an error if reflection is uneven.</exception>
    public CRC32(CRC32 blueprint)
    {
        Polynomial = blueprint.Polynomial;
        Initializer = blueprint.Initializer;
        FinalXor = blueprint.FinalXor;
        ReflectInput = blueprint.ReflectInput;
        ReflectOutput = blueprint.ReflectOutput;
        if (ReflectInput != ReflectOutput)
        {
            throw new NotImplementedException("ReflectInput has to match ReflectOutput. Uneven reflection is not implemented!");
        }

        Name = blueprint.Name;
        table = blueprint.table;
        currentCRC = Initializer;
    }

    /// <summary>Initializes a new instance of the <see cref="CRC32" /> class.</summary>
    public CRC32()
        : this(DefaultPolynomial, 0xFFFFFFFF, true, true, 0xffffffff, "CRC-32") { }

    /// <summary>Initializes a new instance of the <see cref="CRC32" /> class.</summary>
    /// <param name="poly">The polynom.</param>
    /// <param name="init">The initialize value.</param>
    /// <param name="reflectInput">if set to <c>true</c> [reflect input value] first.</param>
    /// <param name="reflectOutput">if set to <c>true</c> [reflect output value] first.</param>
    /// <param name="finalXor">The final xor value.</param>
    /// <param name="name">The name of the checksum.</param>
    public CRC32(uint poly, uint init, bool reflectInput, bool reflectOutput, uint finalXor, string name)
    {
        Polynomial = poly;
        Initializer = init;
        FinalXor = finalXor;
        ReflectInput = reflectInput;
        ReflectOutput = reflectOutput;
        if (ReflectInput != ReflectOutput)
        {
            throw new NotImplementedException("ReflectInput has to match ReflectOutput. Uneven reflection is not implemented!");
        }

        Name = name;
        if (ReflectInput)
        {
            CalculateReflectedTable();
        }
        else
        {
            CalculateTable();
        }

        currentCRC = Initializer;
    }

    #endregion

    #region Properties

    /// <summary>Gets the lookup table.</summary>
    /// <value>The table.</value>
    public uint[] Table => (uint[])table.Clone();

    #endregion

    #region IChecksum<uint> Members

    /// <summary>Resets the checksum to initialization state.</summary>
    public void Reset() => Initialize();

    /// <summary>Adds one byte to the checksum.</summary>
    /// <param name="value">the byte to add. Only the lowest 8 bits will be used.</param>
    public void Update(int value) => HashCore((byte)(value & 0xFF));

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">The buffer containing the data.</param>
    public void Update(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        HashCore(buffer, 0, buffer.Length);
    }

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">The buffer containing the data.</param>
    /// <param name="offset">The offset in the buffer where the data starts.</param>
    /// <param name="count">the number of data bytes to add.</param>
    public void Update(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if ((offset < 0) || (offset > buffer.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if ((count < 0) || ((offset + count) > buffer.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        HashCore(buffer, offset, count);
    }

    /// <summary>Gets or sets the checksum computed so far.</summary>
    public uint Value
    {
        [MethodImpl((MethodImplOptions)0x0100)]
        get
        {
            if (ReflectOutput)
            {
                return currentCRC ^ FinalXor;
            }

            return ~currentCRC ^ FinalXor;
        }
        [MethodImpl((MethodImplOptions)0x0100)]
        set => currentCRC = value;
    }

    #endregion

    #region IHashingFunction Members

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Add<T>(T item)
    {
        var itemHash = (uint)(item?.GetHashCode() ?? 0);
        if (ReflectInput)
        {
            currentCRC = (currentCRC >> 8) ^ table[(currentCRC ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC >> 8) ^ table[(currentCRC ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC >> 8) ^ table[(currentCRC ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC >> 8) ^ table[(currentCRC ^ itemHash) & 0xFF];
        }
        else
        {
            currentCRC = (currentCRC << 8) ^ table[((currentCRC >> 24) ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC << 8) ^ table[((currentCRC >> 24) ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC << 8) ^ table[((currentCRC >> 24) ^ itemHash) & 0xFF];
            itemHash >>= 8;
            currentCRC = (currentCRC << 8) ^ table[((currentCRC >> 24) ^ itemHash) & 0xFF];
        }
    }

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
    public int ToHashCode() => (int)Value;

    #endregion

    #region Overrides

#if !NET20 && !NETCOREAPP1_0_OR_GREATER && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Gets the value of the computed hash code.</summary>
    public override byte[] Hash => BitConverter.GetBytes(Value);
#endif

    /// <summary>Computes the hash for the specified data.</summary>
    /// <param name="array">Array of bytes to hash.</param>
    /// <param name="ibStart">Start index of data.</param>
    /// <param name="cbSize">Size of data in bytes.</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        for (var i = 0; i < cbSize; i++)
        {
            HashCore(array[ibStart++]);
        }
    }

    /// <summary>Finalizes the hash computation, gets the resulting hash code in the systems byte order.</summary>
    /// <returns>Byte array of the hash.</returns>
    protected override byte[] HashFinal() => BitConverter.GetBytes(Value);

    /// <summary>Gets the size, in bits, of the computed hash code.</summary>
    public override int HashSize => 32;

    /// <summary>(Re-)initializes the <see cref="CRC32" />.</summary>
    public override void Initialize() => currentCRC = Initializer;

    #endregion

    #region Overrides

    /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() => Name + " width=32 poly=" + Polynomial + " init=" + Initializer + " refin=" + ReflectInput + " refout=" + ReflectOutput + " xorout=" + FinalXor;

    #endregion

    #region Members

    /// <inheritdoc />
    public object Clone() => new CRC32(this);

    /// <summary>directly hashes one byte.</summary>
    /// <param name="b">The byte.</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void HashCore(byte b)
    {
        if (ReflectInput)
        {
            var i = (currentCRC ^ b) & 0xFF;
            currentCRC = (currentCRC >> 8) ^ table[i];
        }
        else
        {
            var i = ((currentCRC >> 24) ^ b) & 0xFF;
            currentCRC = (currentCRC << 8) ^ table[i];
        }
    }

    /// <summary>Calculates the table.</summary>
    protected void CalculateTable()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var value = i << 24;
            uint crc = 0;
            for (uint n = 0; n < 8; n++)
            {
                unchecked
                {
                    if ((int)(crc ^ value) < 0)
                    {
                        crc = (crc << 1) ^ Polynomial;
                    }
                    else
                    {
                        crc <<= 1;
                    }

                    value <<= 1;
                }
            }

            table[i] = crc;
        }

        this.table = table;
    }

    void CalculateReflectedTable()
    {
        var poly = Reflect32(Polynomial);
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            var crc = i;
            unchecked
            {
                for (uint n = 0; n < 8; n++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = (crc >> 1) ^ poly;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            table[i] = crc;
        }

        this.table = table;
    }

    #endregion
}
