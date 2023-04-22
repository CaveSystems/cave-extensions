using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cave;

/// <summary>Provides thread safe hashing.</summary>
public static class Hash
{
    #region Type enum

    /// <summary>Available hash types.</summary>
    public enum Type
    {
        /// <summary>The none</summary>
        None,

        /// <summary>The crc32 hash algorithm</summary>
        CRC32,

        /// <summary>The crc64 hash algorithm</summary>
        CRC64,

        /// <summary>The md5 hash algorithm</summary>
        MD5,

        /// <summary>The sha1 hash algorithm</summary>
        SHA1,

        /// <summary>The sha256 hash algorithm</summary>
        SHA256,

        /// <summary>The sha384 hash algorithm</summary>
        SHA384,

        /// <summary>The sha512 hash algorithm</summary>
        SHA512
    }

    #endregion

    #region Static

    /// <summary>Creates a hash of the specified type.</summary>
    /// <param name="type">The type.</param>
    /// <returns>Returns a new HashAlgorithm instance.</returns>
    /// <exception cref="NotImplementedException">Throws an exeption if hash type is unknown.</exception>
    [SuppressMessage("Security", "CA5351:Do not use weak cryptos.")]
    [SuppressMessage("Security", "CA5350:Do not use weak cryptos.")]
    public static HashAlgorithm Create(Type type) => type switch
    {
        Type.CRC32 => new CRC32(),
        Type.CRC64 => new CRC64(),
        Type.MD5 => MD5.Create(),
        Type.SHA1 => SHA1.Create(),
        Type.SHA256 => SHA256.Create(),
        Type.SHA384 => SHA384.Create(),
        Type.SHA512 => SHA512.Create(),
        _ => throw new NotImplementedException()
    };

    /// <summary>Obtains the hash code for a specified data array.</summary>
    /// <param name="type">The type.</param>
    /// <param name="data">The bytes to hash.</param>
    /// <returns>A new byte[] containing the hash for the specified data.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    public static byte[] FromArray(Type type, byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var algorithm = Create(type);
        algorithm.Initialize();
        return algorithm.ComputeHash(data);
    }

    /// <summary>Obtains the hash code for a specified data array.</summary>
    /// <param name="type">The type.</param>
    /// <param name="data">The byte array to hash.</param>
    /// <param name="index">The start index.</param>
    /// <param name="count">The number of bytes to hash.</param>
    /// <returns>A new byte[] containing the hash for the specified data.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    public static byte[] FromArray(Type type, byte[] data, int index, int count)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        using var algorithm = Create(type);
        return algorithm.ComputeHash(data, index, count);
    }

    /// <summary>Obtains the hash code for a specified <see cref="Stream" /> string at the current position and reading to the end of the stream.</summary>
    /// <param name="type">The type.</param>
    /// <param name="stream">The stream to hash.</param>
    /// <returns>A new byte[] containing the hash for the specified data.</returns>
    /// <exception cref="ArgumentNullException">stream.</exception>
    public static byte[] FromStream(Type type, Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using var algorithm = Create(type);
        return algorithm.ComputeHash(stream);
    }

    /// <summary>Obtains the hash code for a specified data string (using UTF-8 encoding).</summary>
    /// <param name="type">The type.</param>
    /// <param name="data">The string to hash.</param>
    /// <param name="index">The start index.</param>
    /// <param name="count">The number of chars to hash.</param>
    /// <returns>A new byte[] containing the hash for the specified data.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    public static byte[] FromString(Type type, string data, int index, int count)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return FromArray(type, Encoding.UTF8.GetBytes(data.Substring(index, count)));
    }

    /// <summary>Obtains the hash code for a specified data string (using UTF-8 encoding).</summary>
    /// <param name="type">The type.</param>
    /// <param name="data">The string to hash.</param>
    /// <returns>A new byte[] containing the hash for the specified data.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    public static byte[] FromString(Type type, string data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return FromArray(type, Encoding.UTF8.GetBytes(data));
    }

    #endregion
}
