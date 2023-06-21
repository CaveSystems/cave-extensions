using System;
using System.Security.Cryptography;

namespace Cave.Security;

/// <summary>Provides cryptographically strong random number functions.</summary>
public static class RNG
{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#elif NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER
    static readonly RNGCryptoServiceProvider rng = new();

    /// <summary>Fills an array of bytes with a cryptographically strong sequence of random values.</summary>
    /// <param name="block">The array to fill with a cryptographically strong sequence of random values.</param>
    public static void Fill(byte[] block) => rng.GetBytes(block);
#else
    static Random rng = new();

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong sequence of random values.
    /// </summary>
    /// <param name="block">The array to fill with a cryptographically strong sequence of random values.</param>
    public static void Fill(byte[] block) => rng.NextBytes(block);
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Generates a random integer between a specified inclusive lower bound and a specified
    /// exclusive upper bound using a cryptographically strong random number generator.
    /// </summary>
    /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between fromInclusive (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int fromInclusive, int toExclusive) => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);

    /// <summary>
    /// Generates a random integer between 0 (inclusive) and a specified exclusive upper
    /// bound using a cryptographically strong random number generator.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between 0 (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int toExclusive) => RandomNumberGenerator.GetInt32(toExclusive);

    /// <summary>
    /// Fills an array of bytes with a cryptographically strong sequence of random values.
    /// </summary>
    /// <param name="block">The array to fill with a cryptographically strong sequence of random values.</param>
    public static void Fill(byte[] block) => RandomNumberGenerator.Fill(block);
#else
    /// <summary>
    /// Generates a random integer between a specified inclusive lower bound and a specified exclusive upper bound using a
    /// cryptographically strong random number generator.
    /// </summary>
    /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between fromInclusive (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int fromInclusive, int toExclusive)
    {
        if (toExclusive <= fromInclusive)
        {
            throw new ArgumentOutOfRangeException(nameof(toExclusive));
        }
        var count = toExclusive - fromInclusive;
        return fromInclusive + (Int32 % count);
    }

    /// <summary>
    /// Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a cryptographically strong random
    /// number generator.
    /// </summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between 0 (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int toExclusive) => GetInt32(0, toExclusive);
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Creates an array of bytes with a cryptographically strong random sequence of values.
    /// </summary>
    /// <param name="count">The number of bytes of random values to create.</param>
    /// <returns>An array populated with cryptographically strong random values.</returns>
    public static byte[] GetBytes(int count) => RandomNumberGenerator.GetBytes(count);
#else
    /// <summary>Creates an array of bytes with a cryptographically strong random sequence of values.</summary>
    /// <param name="count">The number of bytes of random values to create.</param>
    /// <returns>An array populated with cryptographically strong random values.</returns>
    public static byte[] GetBytes(int count)
    {
        var block = new byte[count];
        Fill(block);
        return block;
    }
#endif

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static int Int32 => BitConverter.ToInt32(GetBytes(4), 0);

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static long Int64 => BitConverter.ToInt64(GetBytes(8), 0);

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static uint UInt32 => BitConverter.ToUInt32(GetBytes(4), 0);

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static ulong UInt64 => BitConverter.ToUInt64(GetBytes(8), 0);
}
