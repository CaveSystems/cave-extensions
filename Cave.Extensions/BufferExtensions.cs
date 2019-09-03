using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Cave
{
    /// <summary>
    /// Provides extensions to byte buffers.
    /// </summary>
    public static class BufferExtensions
    {
        /// <summary>
        /// Obfuscates a byte buffer with a random key. This is not an encryption.
        /// </summary>
        /// <param name="data">Byte buffer to obfuscate.</param>
        /// <param name="algorithm">Algorithm to use.</param>
        /// <returns>Returns the obfuscated byte buffer.</returns>
        public static byte[] Obfuscate(this byte[] data, SymmetricAlgorithm algorithm = null)
        {
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
                using (var enc = algorithm.CreateEncryptor())
                {
                    List<byte> encoded = new List<byte>();
                    encoded.Add((byte)algorithm.Key.Length);
                    encoded.AddRange(algorithm.Key);
                    encoded.Add((byte)algorithm.IV.Length);
                    encoded.AddRange(algorithm.IV);
                    encoded.AddRange(enc.TransformFinalBlock(data, 0, data.Length));
                    return encoded.ToArray();
                }
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        /// Deobfuscates a byte buffer.
        /// </summary>
        /// <param name="data">Byte buffer to deobfuscate.</param>
        /// <param name="algorithm">Algorithm to use.</param>
        /// <returns>Returns the deobfuscated byte buffer.</returns>
        public static byte[] Deobfuscate(this byte[] data, SymmetricAlgorithm algorithm = null)
        {
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
                int ofs = 1 + key.Length;
                var iv = new byte[data[ofs++]];
                Buffer.BlockCopy(data, ofs, iv, 0, iv.Length);
                ofs += iv.Length;
                using (var dec = algorithm.CreateDecryptor(key, iv))
                {
                    return dec.TransformFinalBlock(data, ofs, data.Length - ofs);
                }
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        /// Gets the 4 byte nibbles of each byte at the specified buffer.
        /// </summary>
        /// <param name="data">The buffer.</param>
        /// <returns>Returns an array of nibbles.</returns>
        public static byte[] GetNibbles(this byte[] data)
        {
            byte[] result = new byte[data.Length << 2];
            for (int i = 0, n = 0; i < data.Length; i++)
            {
                result[n++] = (byte)(data[i] >> 4);
                result[n++] = (byte)(data[i] & 0xF);
            }
            return result;
        }
    }
}
