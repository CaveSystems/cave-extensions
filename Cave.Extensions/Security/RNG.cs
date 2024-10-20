using System;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Cave.Security;

/// <summary>Provides cryptographically strong random number functions.</summary>
public static class RNG
{
#if NET20_OR_GREATER || NETSTANDARD2_0_OR_GREATER
    static RNGCryptoServiceProvider generator = new();

    /// <summary>Fills an array of bytes with a cryptographically strong sequence of random values.</summary>
    /// <param name="block">The array to fill with a cryptographically strong sequence of random values.</param>
    public static void Fill(byte[] block) => generator.GetBytes(block);

    /// <summary>Gets or sets the currently used generator.</summary>
    public static RNGCryptoServiceProvider Generator { get => generator; set => generator = value ?? throw new ArgumentNullException(nameof(value)); }

#else
    static RandomNumberGenerator generator = RandomNumberGenerator.Create();

    /// <summary>Fills an array of bytes with a cryptographically strong sequence of random values.</summary>
    /// <param name="block">The array to fill with a cryptographically strong sequence of random values.</param>
    public static void Fill(byte[] block) => generator.GetBytes(block);

    /// <summary>Gets or sets the currently used generator.</summary>
    public static RandomNumberGenerator Generator { get => generator; set => generator = value ?? throw new ArgumentNullException(nameof(value)); }
#endif

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    /// <summary>
    /// Generates a random integer between a specified inclusive lower bound and a specified exclusive upper bound using a cryptographically strong random
    /// number generator.
    /// </summary>
    /// <param name="fromInclusive">The inclusive lower bound of the random range.</param>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between fromInclusive (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int fromInclusive, int toExclusive) => RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);

    /// <summary>Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a cryptographically strong random number generator.</summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between 0 (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int toExclusive) => RandomNumberGenerator.GetInt32(toExclusive);

#else

    /// <summary>
    /// Generates a random integer between a specified inclusive lower bound and a specified exclusive upper bound using a cryptographically strong random
    /// number generator.
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

    /// <summary>Generates a random integer between 0 (inclusive) and a specified exclusive upper bound using a cryptographically strong random number generator.</summary>
    /// <param name="toExclusive">The exclusive upper bound of the random range.</param>
    /// <returns>A random integer between 0 (inclusive) and toExclusive (exclusive).</returns>
    public static int GetInt32(int toExclusive) => GetInt32(0, toExclusive);

#endif

#if NET6_0_OR_GREATER

    /// <summary>Creates an array of bytes with a cryptographically strong random sequence of values.</summary>
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

    /// <summary>Gets a random 16 bit signed integer.</summary>
    public static short Int16 => BitConverter.ToInt16(GetBytes(2), 0);

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static sbyte Int8 => (sbyte)GetBytes(1)[0];

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static ushort UInt16 => BitConverter.ToUInt16(GetBytes(2), 0);

    /// <summary>Gets a cryptographically strong random value within full valid range of the target value.</summary>
    public static byte UInt8 => GetBytes(1)[0];

    /// <summary>Gets a random 7 bit us ascii string in range 1..127</summary>
    /// <param name="length">Length of the string</param>
    /// <returns>Returns a new string</returns>
    public static string GetAscii(int length)
    {
        var block = GetBytes(length);
        var result = new char[length];
        for (var i = 0; i < length; i++)
        {
            var value = block[i] % 128;
            while (value == 0) value = UInt8 % 128;
            result[i] = (char)value;
        }
        return new(result);
    }

    /// <summary>Gets a random string using the specified characters</summary>
    /// <param name="length">Length of the string</param>
    /// <param name="validChars">Characters to use</param>
    /// <returns>Returns a new string</returns>
    public static string GetString(int length, string validChars)
    {
        var block = GetBytes(length);
        var result = new char[length];
        for (var i = 0; i < length; i++)
        {
            result[i] = validChars[block[i] % validChars.Length];
        }
        return new(result);
    }

    /// <summary>Gets a random unicode string</summary>
    /// <param name="length">Length of the string</param>
    /// <returns>Returns a new string</returns>
    public static string GetUnicode(int length)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            for (; ; )
            {
                var codepoint = (int)(UInt32 % 0x110000);
                if (codepoint >= 0xD800 && codepoint <= 0xDFFF) continue;
                try
                {
                    sb.Append(char.ConvertFromUtf32(codepoint));
                    break;
                }
                catch { /* depending on framework version there are bugs in the unicode converter, ignore them */ }
            }
        }
        return sb.ToString();
    }

    /// <summary>Gets a random value</summary>
    public static bool Bool => (UInt8 & 1) == 0;

    static ulong MaxTicks { get; } = (ulong)(DateTime.MaxValue - DateTime.MinValue).Ticks;

    /// <summary>Returns the range in ticks for new random timespans (+/- ~5000 years)</summary>
    public const long TimeSpanDefaultTickRange = TimeSpan.TicksPerDay * (long)(365.25 * 5000);

    /// <summary>Gets a random timespan within the given range.</summary>
    /// <param name="ticksMin">Minimum ticks</param>
    /// <param name="ticksMax">Maximum ticks</param>
    /// <param name="tickStep">Step in ticks</param>
    /// <returns>Returns a new timespan value</returns>
    /// <exception cref="Exception"></exception>
    public static TimeSpan GetTimeSpan(long ticksMin, long ticksMax, long tickStep)
    {
        for (var i = 0; ; i++)
        {
            var value = unchecked(ticksMin + (UInt32 * tickStep));
            value -= value % tickStep;
            if (value > ticksMin && value < ticksMax) return new TimeSpan(value);
            if (i > 100) throw new Exception("Could not find a valid result!");
        }
    }

    /// <summary>Gets a random timespan within <see cref="TimeSpan.MinValue"/> and <see cref="TimeSpan.MaxValue"/>.</summary>
    public static TimeSpan TimeSpanAny => new TimeSpan(Int64);

    /// <summary>Gets a random timespan within +/- <see cref="TimeSpanDefaultTickRange"/> with an accuracy of one millisecond.</summary>
    public static TimeSpan TimeSpanMilliSeconds => GetTimeSpan(-TimeSpanDefaultTickRange, +TimeSpanDefaultTickRange, TimeSpan.TicksPerMillisecond);

    /// <summary>Gets a random timespan within +/- <see cref="TimeSpanDefaultTickRange"/> with an accuracy of one second.</summary>
    public static TimeSpan TimeSpanSeconds => GetTimeSpan(-TimeSpanDefaultTickRange, +TimeSpanDefaultTickRange, TimeSpan.TicksPerSecond);

    /// <summary>Gets a random datetime within <see cref="DateTime.MinValue"/> and <see cref="DateTime.MaxValue"/>.</summary>
    public static DateTime DateTime => new DateTime((long)(UInt64 % MaxTicks));

    /// <summary>Creates a random password using ascii printable characters.</summary>
    /// <param name="count">Length of the desired password.</param>
    /// <param name="characters">The characters.</param>
    /// <returns>The password string.</returns>
    public static string GetPassword(int count, string? characters = null)
    {
        var result = new char[count];
        var value = UInt32;
        char[] chars;
        if (characters != null)
        {
            chars = characters.ToCharArray();
        }
        else
        {
            chars = new char[126 - 32 - 2];
            for (int x = 33, n = 0; n < chars.Length; x++)
            {
                var c = (char)x;
                if (c == '"')
                {
                    continue;
                }

                if (c == '\'')
                {
                    continue;
                }

                chars[n++] = c;
            }
        }

        var charsCount = (uint)chars.Length;
        var i = 0;
        while (i < count)
        {
            if (value < chars.Length)
            {
                value ^= UInt32;
            }

            var index = value % charsCount;
            result[i++] = chars[index];
            value /= charsCount;
        }

        return new string(result);
    }
}
