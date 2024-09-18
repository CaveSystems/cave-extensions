using System;
using System.Numerics;
using System.Text;

namespace Cave;

/// <summary>Provides extensions to <see cref="IBaseX"/> interfaces.</summary>
public static class IBaseXExtensions
{
    #region Public Methods

    /// <summary>Decodes the specified BaseX string.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded data</returns>
    public static byte[] Decode(this IBaseX baseX, string baseXstring) => baseX.Decode(ASCII.GetBytes(baseXstring));

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static short DecodeInt16(this IBaseX baseX, string baseXstring) => (short)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static int DecodeInt32(this IBaseX baseX, string baseXstring) => (int)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static long DecodeInt64(this IBaseX baseX, string baseXstring) => baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static sbyte DecodeInt8(this IBaseX baseX, string baseXstring) => (sbyte)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static ushort DecodeUInt16(this IBaseX baseX, string baseXstring) => (ushort)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static uint DecodeUInt32(this IBaseX baseX, string baseXstring) => (uint)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static ulong DecodeUInt64(this IBaseX baseX, string baseXstring) => (ulong)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX string to a value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded value</returns>
    public static byte DecodeUInt8(this IBaseX baseX, string baseXstring) => (byte)baseX.DecodeValue(baseXstring);

    /// <summary>Decodes the specified BaseX data using utf8 encoding.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXdata">BaseX data array</param>
    /// <returns>Returns the decoded string</returns>
    public static string DecodeUtf8(this IBaseX baseX, byte[] baseXdata) => UTF8.GetString(baseX.Decode(baseXdata));

    /// <summary>Decodes the specified BaseX string using utf8 encoding.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded string</returns>
    public static string DecodeUtf8(this IBaseX baseX, string baseXstring) => UTF8.GetString(baseX.Decode(baseXstring));

#if !NET20 && !NET35
    /// <summary>Decodes the specified BaseX string using binary encoding.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded string</returns>
    public static BigInteger DecodeBigInteger(this IBaseX baseX, string baseXstring) => baseX.DecodeBigInteger(ASCII.GetBytes(baseXstring));
#endif

    /// <summary>Decodes the specified BaseX string using binary encoding.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="baseXstring">BaseX string value</param>
    /// <returns>Returns the decoded string</returns>
    public static long DecodeValue(this IBaseX baseX, string baseXstring) => baseX.DecodeValue(ASCII.GetBytes(baseXstring));

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, byte value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, int value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, long value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, sbyte value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, short value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, uint value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, ulong value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified value.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="value">The value to encode.</param>
    /// <returns>Returns the encoded string.</returns>
    public static string Encode(this IBaseX baseX, ushort value) => baseX.EncodeValue(value);

    /// <summary>Encodes the specified string using utf8 characters into a resulting BaseX string.</summary>
    /// <param name="baseX">Instance to use</param>
    /// <param name="text">Text to encode to BaseX</param>
    /// <returns>Returns a BaseX string.</returns>
    public static string Encode(this IBaseX baseX, string text) => baseX.Encode(UTF8.GetBytes(text));

    #endregion Public Methods
}
