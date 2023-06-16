using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Cave;

/// <summary>Provides a managed implementation of the Cyclic Redundancy Checksum with 64 bits.</summary>
public class CRC64 : HashAlgorithm, IChecksum<ulong>, IHashingFunction
{
    #region Static

    /// <summary>Provides the default polynomial.</summary>
    public static readonly ulong DefaultPolynomial = 0x42f0e1eba9ea3693;

    /// <summary>Reflects 64 bits.</summary>
    /// <param name="x">The bits.</param>
    /// <returns>Returns a center reflection.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong Reflect64(ulong x)
    {
        // move bits
        x = ((x & 0x5555555555555555) << 1) | ((x >> 1) & 0x5555555555555555);
        x = ((x & 0x3333333333333333) << 2) | ((x >> 2) & 0x3333333333333333);
        x = ((x & 0x0F0F0F0F0F0F0F0F) << 4) | ((x >> 4) & 0x0F0F0F0F0F0F0F0F);

        // move bytes
        x = (x << 56) | ((x & 0xFF00) << 40) | ((x & 0xFF0000) << 24) | ((x & 0xFF000000) << 8) | ((x >> 8) & 0xFF000000) | ((x >> 24) & 0xFF0000) | ((x >> 40) & 0xFF00) | (x >> 56);
        return x;
    }

    /// <summary>
    /// Gets width=64 poly=0x42f0e1eba9ea3693 init=0x0000000000000000 refin=false refout=false xorout=0x0000000000000000
    /// check=0x6c40df5f0b497347 residue=0x0000000000000000 name="CRC-64".
    /// </summary>
    public static CRC64 ECMA182 => new(DefaultPolynomial, 0x0000000000000000, finalXor: 0x0000000000000000, reflectInput: false, reflectOutput: false, name: "CRC-64");

    /// <summary>
    /// Gets width=64 poly=0x42f0e1eba9ea3693 init=0xffffffffffffffff refin=false refout=false xorout=0xffffffffffffffff
    /// check=0x62ec59e3f1a4f00a residue=0xfcacbebd5931a992 name="CRC-64/WE".
    /// </summary>
    public static CRC64 WE => new(DefaultPolynomial, 0xffffffffffffffff, finalXor: 0xffffffffffffffff, reflectInput: false, reflectOutput: false, name: "CRC-64/WE");

    /// <summary>
    /// Gets width=64 poly=0x42f0e1eba9ea3693 init=0xffffffffffffffff refin=true refout=true xorout=0xffffffffffffffff
    /// check=0x995dc9bbdf1939fa residue=0x49958c9abd7d353f name="CRC-64/XZ".
    /// </summary>
    public static CRC64 XZ => new(DefaultPolynomial, 0xffffffffffffffff, finalXor: 0xffffffffffffffff, reflectInput: true, reflectOutput: true, name: "CRC-64/XZ");

    #endregion

    #region Fields

    /// <summary>The polynomial used to generate the table.</summary>
    public readonly ulong Polynomial;

    /// <summary>The initializer value.</summary>
    public readonly ulong Initializer;

    /// <summary>The final xor value.</summary>
    public readonly ulong FinalXor;

    /// <summary>The reflect input flag.</summary>
    public readonly bool ReflectInput;

    /// <summary>The reflect output flag.</summary>
    public readonly bool ReflectOutput;

    ulong currentCRC;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CRC64" /> class.</summary>
    /// <param name="blueprint">The blueprint to copy all properties from.</param>
    /// <exception cref="NotImplementedException">Throws an error if reflection is uneven.</exception>
    public CRC64(CRC64 blueprint)
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
        Table = blueprint.Table;
        currentCRC = Initializer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CRC64" /> class. Creates a new CRC64.XZ: width=64 poly=0x42f0e1eba9ea3693
    /// init=0xffffffffffffffff refin=true refout=true xorout=0xffffffffffffffff check=0x995dc9bbdf1939fa residue=0x49958c9abd7d353f
    /// name="CRC-64/XZ".
    /// </summary>
    public CRC64()
        : this(DefaultPolynomial, 0xffffffffffffffff, finalXor: 0xffffffffffffffff, reflectInput: true, reflectOutput: true, name: "CRC-64/XZ") { }

    /// <summary>Initializes a new instance of the <see cref="CRC64" /> class.</summary>
    /// <param name="poly">The polynom.</param>
    /// <param name="init">The initialize value.</param>
    /// <param name="reflectInput">if set to <c>true</c> [reflect input value] first.</param>
    /// <param name="reflectOutput">if set to <c>true</c> [reflect output value] first.</param>
    /// <param name="finalXor">The final xor value.</param>
    /// <param name="name">The name of the checksum.</param>
    public CRC64(ulong poly, ulong init, bool reflectInput, bool reflectOutput, ulong finalXor, string name)
    {
        Polynomial = poly;
        Initializer = init;
        FinalXor = finalXor;
        ReflectInput = reflectInput;
        ReflectOutput = reflectOutput;
        Name = name;
        if (ReflectInput != ReflectOutput)
        {
            throw new NotImplementedException("ReflectInput has to match ReflectOutput. Uneven reflection is not implemented!");
        }

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

    /// <summary>Gets the name of the hash.</summary>
    public string Name { get; }

    /// <summary>Gets the lookup table.</summary>
    /// <value>The table.</value>
    public ulong[] Table { get; set; }

    #endregion

    #region IChecksum<ulong> Members

    /// <summary>Resets the checksum to initialization state.</summary>
    public void Reset() => Initialize();

    /// <summary>Adds one byte to the checksum.</summary>
    /// <param name="value">the byte to add. Only the lowest 8 bits will be used.</param>
    public void Update(int value)
    {
        unchecked
        {
            HashCore((byte)value);
        }
    }

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">The buffer containing the data.</param>
    public void Update(byte[] buffer) => HashCore(buffer, 0, buffer.Length);

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">The buffer containing the data.</param>
    /// <param name="offset">The offset in the buffer where the data starts.</param>
    /// <param name="count">the number of data bytes to add.</param>
    public void Update(byte[] buffer, int offset, int count) => HashCore(buffer, offset, count);

    /// <summary>Gets or sets the checksum computed so far.</summary>
    public ulong Value
    {
        [MethodImpl((MethodImplOptions)0x0100)]
        get => currentCRC ^ FinalXor;
        [MethodImpl((MethodImplOptions)0x0100)]
        set => currentCRC = value;
    }

    #endregion

    #region IHashingFunction Members

    /// <inheritdoc />
    [MethodImpl((MethodImplOptions)0x0100)]
    public void Add<T>(T item)
    {
        var itemHash = item?.GetHashCode() ?? 0;
        Update(itemHash);
        itemHash >>= 8;
        Update(itemHash);
        itemHash >>= 8;
        Update(itemHash);
        itemHash >>= 8;
        Update(itemHash);
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
    public int ToHashCode() => (int)((Value >> 32) ^ (Value & 0xffffffff));

    #endregion

    #region Overrides

#if !NET20 && !NETCOREAPP1_0_OR_GREATER && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Gets the value of the computed hash code.</summary>
    public override byte[] Hash => BitConverter.GetBytes(Value);
#endif

    /// <summary>
    /// Computes the hash for the specified data. The caller needs to <see cref="Initialize" /> the <see cref="CRC64" /> first and call
    /// <see
    ///     cref="HashFinal" />
    /// afterwards to obtain the full hash code.
    /// </summary>
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

    /// <summary>Finalizes the hash computation obtains the resulting hash code in the systems byte order.</summary>
    /// <returns>Byte array of the hash.</returns>
    protected override byte[] HashFinal() => BitConverter.GetBytes(Value);

    /// <summary>Gets the size, in bits, of the computed hash code.</summary>
    public override int HashSize => 32;

    /// <summary>(Re-)initializes the <see cref="CRC64" />.</summary>
    public override void Initialize() => currentCRC = Initializer;

    #endregion

    #region Overrides

    /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() => Name + " width=32 poly=" + Polynomial + " init=" + Initializer + " refin=" + ReflectInput + " refout=" + ReflectOutput + " xorout=" + FinalXor;

    #endregion

    #region Members

    /// <summary>Erstellt ein neues Objekt, das eine Kopie der aktuellen Instanz darstellt.</summary>
    /// <returns>Ein neues Objekt, das eine Kopie dieser Instanz darstellt.</returns>
    public object Clone() => new CRC64(this);

    /// <summary>directly hashes one byte.</summary>
    /// <param name="b">The byte.</param>
    [MethodImpl((MethodImplOptions)0x0100)]
    public void HashCore(byte b)
    {
        if (ReflectInput)
        {
            var i = (currentCRC ^ b) & 0xFF;
            currentCRC = (currentCRC >> 8) ^ Table[i];
        }
        else
        {
            var i = ((currentCRC >> 56) ^ b) & 0xFF;
            currentCRC = (currentCRC << 8) ^ Table[i];
        }
    }

    /// <summary>Calculates the table.</summary>
    protected void CalculateTable()
    {
        var table = new ulong[256];
        for (ulong i = 0; i < 256; i++)
        {
            var value = i << 56;
            ulong crc = 0;
            for (ulong n = 0; n < 8; n++)
            {
                unchecked
                {
                    if ((long)(crc ^ value) < 0)
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

        Table = table;
    }

    void CalculateReflectedTable()
    {
        var poly = Reflect64(Polynomial);
        var table = new ulong[256];
        for (uint i = 0; i < 256; i++)
        {
            ulong crc = i;
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

        Table = table;
    }

    #endregion
}
