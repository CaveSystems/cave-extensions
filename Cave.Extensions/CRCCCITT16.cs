using System;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Cave;

/// <summary>Implements a fast implementation of the CRC-CCITT-16 algorithm for the polynomial 0x1021.</summary>
public class CRCCCITT16 : HashAlgorithm, IChecksum<ushort>
{
    #region Private Fields

    uint crc = ushort.MaxValue;

    #endregion Private Fields

    #region Protected Methods

    /// <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
    /// <remarks>
    /// This method is not called by application code. <br/> This abstract method performs the hash computation.Every write to the cryptographic stream object
    /// passes the data through this method.For each block of data, this method updates the state of the hash object so a correct hash value is returned at the
    /// end of the data stream.
    /// </remarks>
    /// <param name="array">The input to compute the hash code for.</param>
    /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
    /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
    protected override void HashCore(byte[] array, int ibStart, int cbSize) => Update(array, ibStart, cbSize);

    /// <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
    /// <remarks>This method finalizes any partial computation and returns the correct hash value for the data stream.</remarks>
    /// <returns>The computed hash code.</returns>
    protected override byte[] HashFinal() => BitConverter.GetBytes(Value);

    #endregion Protected Methods

    #region Public Properties

    /// <summary>Gets the size, in bits, of the computed hash code.</summary>
    public override int HashSize => 16;

    /// <summary>Gets or sets the checksum computed so far.</summary>
    public ushort Value
    {
        get => (ushort)(crc & 0xFFFF);
        set => crc = value;
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <summary>Initializes an implementation of the HashAlgorithm class.</summary>
    public override void Initialize() => crc = ushort.MaxValue;

    /// <summary>Resets the checksum to initialization state.</summary>
    public void Reset() => Initialize();

    /// <summary>Adds one byte to the checksum.</summary>
    /// <param name="value">the byte to add. Only the lowest 8 bits will be used.</param>
    public void Update(int value)
    {
        var v = (byte)(value &= 0xFF);
        var x = crc >> 8;
        x ^= v;
        x ^= x >> 4;
        crc = (crc << 8) ^ (x << 12) ^ (x << 5) ^ x;
    }

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">buffer an array of bytes.</param>
    public void Update(byte[] buffer)
    {
        for (var i = 0; i < buffer.Length; i++)
        {
            var x = crc >> 8;
            x ^= buffer[i];
            x ^= x >> 4;
            crc = (crc << 8) ^ (x << 12) ^ (x << 5) ^ x;
        }
    }

    /// <summary>Updates the checksum with the specified byte array.</summary>
    /// <param name="buffer">The buffer containing the data.</param>
    /// <param name="offset">The offset in the buffer where the data starts.</param>
    /// <param name="count">the number of data bytes to add.</param>
    public void Update(byte[] buffer, int offset, int count)
    {
        var end = offset + count;
        for (var i = offset; i < end; i++)
        {
            var x = crc >> 8;
            x ^= buffer[i];
            x ^= x >> 4;
            crc = (crc << 8) ^ (x << 12) ^ (x << 5) ^ x;
        }
    }

    #endregion Public Methods

#if !NET20 && !NETCOREAPP1_0_OR_GREATER && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>Gets the value of the computed hash code.</summary>
    public override byte[] Hash => BitConverter.GetBytes(Value);
#endif
}
