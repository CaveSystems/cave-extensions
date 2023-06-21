using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Cave;

/// <summary>Gets extensions to byte buffers.</summary>
public static class BufferExtensions
{
    #region Static

    /// <summary>Concatenates buffers.</summary>
    /// <param name="block1">First block</param>
    /// <param name="array">Blocks to add.</param>
    /// <returns>Returns a new buffer.</returns>
    public static byte[] Concat(this byte[] block1, params byte[][] array)
    {
        var len = block1.Length + array.Sum(a => a.Length);
        var result = new byte[len];

        Buffer.BlockCopy(block1, 0, result, 0, block1.Length);
        len = block1.Length;

        foreach (var block in array)
        {
            Buffer.BlockCopy(block, 0, result, len, block.Length);
            len += block.Length;
        }
        return result;
    }

    /// <summary>Concatenates buffers.</summary>
    /// <param name="block1">First block</param>
    /// <param name="block2">Second block.</param>
    /// <returns>Returns a new buffer.</returns>
    public static byte[] Concat(this byte[] block1, byte[] block2)
    {
        var result = new byte[block1.Length + block2.Length];
        Buffer.BlockCopy(block1, 0, result, 0, block1.Length);
        Buffer.BlockCopy(block2, 0, result, block1.Length, block2.Length);
        return result;
    }

    /// <summary>Deobfuscates a byte buffer.</summary>
    /// <param name="data">Byte buffer to deobfuscate.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <returns>Returns the deobfuscated byte buffer.</returns>
    public static byte[] Deobfuscate(this byte[] data, SymmetricAlgorithm algorithm = null)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        IDisposable disposable = null;
        if (algorithm == null)
        {
#if NET20
            disposable = algorithm = Rijndael.Create();
#else
            disposable = algorithm = Aes.Create();
#endif
        }

        try
        {
            var key = new byte[data[0]];
            Buffer.BlockCopy(data, 1, key, 0, key.Length);
            var ofs = 1 + key.Length;
            var iv = new byte[data[ofs++]];
            Buffer.BlockCopy(data, ofs, iv, 0, iv.Length);
            ofs += iv.Length;
            using var dec = algorithm.CreateDecryptor(key, iv);
            return dec.TransformFinalBlock(data, ofs, data.Length - ofs);
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    /// <summary>Gets the 4 byte nibbles of each byte at the specified buffer.</summary>
    /// <param name="data">The buffer.</param>
    /// <returns>Returns an array of nibbles.</returns>
    public static byte[] GetNibbles(this byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var result = new byte[data.Length << 1];
        for (int i = 0, n = 0; i < data.Length; i++)
        {
            result[n++] = (byte)(data[i] >> 4);
            result[n++] = (byte)(data[i] & 0xF);
        }

        return result;
    }

    /// <summary>Obfuscates a byte buffer with a random key. This is not an encryption.</summary>
    /// <param name="data">Byte buffer to obfuscate.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <returns>Returns the obfuscated byte buffer.</returns>
    [SuppressMessage("Style", "IDE0028")]
    public static byte[] Obfuscate(this byte[] data, SymmetricAlgorithm algorithm = null)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        IDisposable disposable = null;
        if (algorithm == null)
        {
#if NET20
            disposable = algorithm = Rijndael.Create();
#else
            disposable = algorithm = Aes.Create();
#endif
        }

        try
        {
            using var enc = algorithm.CreateEncryptor();
            var maxLength = algorithm.Key.Length + algorithm.IV.Length + (algorithm.BlockSize * 2) + data.Length;
            var encoded = new List<byte>(maxLength);
            // add key
            encoded.Add((byte)algorithm.Key.Length);
            encoded.AddRange(algorithm.Key);
            // add iv
            encoded.Add((byte)algorithm.IV.Length);
            encoded.AddRange(algorithm.IV);
            // add data
            encoded.AddRange(enc.TransformFinalBlock(data, 0, data.Length));
            return encoded.ToArray();
        }
        finally
        {
            disposable?.Dispose();
        }
    }

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong SwapEndian(this ulong value) => (value << 56) | ((value & 0xFF00) << 40) | ((value & 0xFF0000) << 24) | ((value & 0xFF000000) << 8) | ((value >> 8) & 0xFF000000) | ((value >> 24) & 0xFF0000) | ((value >> 40) & 0xFF00) | (value >> 56);

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long SwapEndian(this long value) => (value << 56) | ((value & 0xFF00) << 40) | ((value & 0xFF0000) << 24) | ((value & 0xFF000000) << 8) | ((value >> 8) & 0xFF000000) | ((value >> 24) & 0xFF0000) | ((value >> 40) & 0xFF00) | (value >> 56);

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint SwapEndian(this uint value) => (value << 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | (value >> 24);

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int SwapEndian(this int value) => (value << 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | (value >> 24);

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ushort SwapEndian(this ushort value) => (ushort)((value << 8) | (value >> 8));

    /// <summary>Returns a value with swapped endianess of the specified <paramref name="value"/>.</summary>
    /// <param name="value">Value to swap endianess at.</param>
    /// <returns>Returns the value with swapped endianess.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static short SwapEndian(this short value) => (short)((value << 8) | (value >> 8));

    /// <summary>Swaps low and high byte for a byte buffer containing 16 bit values inplace.</summary>
    /// <param name="data">Buffer to change endianess at.</param>
    public static unsafe void SwapEndian16(this byte[] data)
    {
        unchecked
        {
            fixed (byte* ptr = data)
            {
                var pointer = (ushort*)ptr;
                var len = data.Length / 2;
                for (var i = 0; i < len; i++)
                {
                    pointer[i] = SwapEndian(pointer[i]);
                }
            }
        }
    }

    /// <summary>Swaps low and high byte for a byte buffer containing 32 bit values inplace.</summary>
    /// <param name="data">Buffer to change endianess at.</param>
    public static unsafe void SwapEndian32(this byte[] data)
    {
        unchecked
        {
            fixed (byte* ptr = data)
            {
                var pointer = (uint*)ptr;
                var len = data.Length / 2;
                for (var i = 0; i < len; i++)
                {
                    pointer[i] = SwapEndian(pointer[i]);
                }
            }
        }
    }

    /// <summary>Swaps low and high byte for a byte buffer containing 64 bit values inplace.</summary>
    /// <param name="data">Buffer to change endianess at.</param>
    public static unsafe void SwapEndian64(this byte[] data)
    {
        unchecked
        {
            fixed (byte* ptr = data)
            {
                var pointer = (uint*)ptr;
                var len = data.Length / 2;
                for (var i = 0; i < len; i++)
                {
                    pointer[i] = SwapEndian(pointer[i]);
                }
            }
        }
    }

    #endregion Static
}
