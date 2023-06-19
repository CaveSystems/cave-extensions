using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf16.</summary>
public sealed class UTF16BE : IUnicode, IComparable<UTF16BE>, IEquatable<UTF16BE>
{
    #region Static

    static char[] ConvertToChars(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (BitConverter.IsLittleEndian)
        {
            data = (byte[])data.Clone();
            data.SwapEndian16();
        }
        var chars = new char[data.Length / 2];
        Buffer.BlockCopy(data, 0, chars, 0, data.Length);
        return chars;
    }

    /// <summary>Converts the specified utf16 data to a csharp string.</summary>
    /// <param name="data">Data to convert.</param>
    /// <returns>Returns a csharp string.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ConvertToString(byte[] data) => new(ConvertToChars(data));

    /// <summary>Converts the specified utf16 data to unicode codepoints.</summary>
    /// <param name="data">Data to convert.</param>
    /// <returns>Returns the unicode codepoints.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static int[] ConvertToCodepoints(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        var chars = ConvertToChars(data);
        var result = new int[chars.Length];
        var len = 0;
        for (var i = 0; i < chars.Length;)
        {
            int codepoint = chars[i++];
            if (codepoint is >= 0xD800 and < 0xDFFF)
            {
                int lowSurrogate = chars[i++];
                codepoint = 0x10000 + (((codepoint & 0x3FF) << 10) | (lowSurrogate & 0x3FF));
            }
            result[len++] = codepoint;
        }
        if (len != result.Length)
        {
            Array.Resize(ref result, len);
        }
        return result;
    }

    /// <summary>Converts the specified <paramref name="text" /> to an <see cref="UTF16BE" /> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF16BE" /> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static UTF16BE ConvertFromString(string text)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var chars = text.ToCharArray();
        var data = new byte[chars.Length * 2];
        Buffer.BlockCopy(chars, 0, data, 0, data.Length);
        if (BitConverter.IsLittleEndian)
        {
            data.SwapEndian16();
        }
        return new(data);
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF16BE s1, UTF16BE s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator >(UTF16BE left, UTF16BE right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc />
    public static bool operator >=(UTF16BE left, UTF16BE right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>Performs an implicit conversion from <see cref="UTF16BE" /> to <see cref="string" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(UTF16BE s) => s?.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="UTF16LE" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF16BE(string s) => s == null ? null : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF16BE s1, UTF16BE s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator <(UTF16BE left, UTF16BE right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator <=(UTF16BE left, UTF16BE right) => left is null || (left.CompareTo(right) <= 0);

    #endregion

    #region Constructors

    /// <summary>Creates a new instance of the <see cref="UTF16BE" /> class.</summary>
    /// <param name="data">Content</param>
    public UTF16BE(byte[] data) => Data = data;

    #endregion

    #region Properties

    /// <summary>Gets the data bytes.</summary>
    public byte[] Data { get; }

    /// <summary>Gets the unicode codepoints.</summary>
    public int[] Codepoints => ConvertToCodepoints(Data).ToArray();

    /// <summary>Gets the string length or csharp character count.</summary>
    /// <remarks>
    /// This is not the number of unicode characters since csharp uses utf16 with high and low surrogate pairs to represent non BMP (Basic
    /// Multilingual Plane) characters.
    /// </remarks>
    public int Length => ToString().Length;

    #endregion

    #region IComparable Members

    /// <inheritdoc />
    public int CompareTo(object obj)
    {
        if (null == obj)
        {
            return 1;
        }
        return ToString().CompareTo(obj.ToString());
    }

    #endregion

    #region IComparable<string> Members

    /// <inheritdoc />
    public int CompareTo(string other) => ToString().CompareTo(other);

    #endregion

    #region IComparable<UTF8> Members

    /// <inheritdoc />
    public int CompareTo(UTF16BE other) => ToString().CompareTo(other?.ToString());

    #endregion

    #region IConvertible Members

    /// <inheritdoc />
    public TypeCode GetTypeCode() => TypeCode.String;

    /// <inheritdoc />
    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)ToString()).ToBoolean(provider);

    /// <inheritdoc />
    public byte ToByte(IFormatProvider provider) => ((IConvertible)ToString()).ToByte(provider);

    /// <inheritdoc />
    public char ToChar(IFormatProvider provider) => ((IConvertible)ToString()).ToChar(provider);

    /// <inheritdoc />
    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)ToString()).ToDateTime(provider);

    /// <inheritdoc />
    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)ToString()).ToDecimal(provider);

    /// <inheritdoc />
    public double ToDouble(IFormatProvider provider) => ((IConvertible)ToString()).ToDouble(provider);

    /// <inheritdoc />
    public short ToInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToInt16(provider);

    /// <inheritdoc />
    public int ToInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToInt32(provider);

    /// <inheritdoc />
    public long ToInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToInt64(provider);

    /// <inheritdoc />
    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)ToString()).ToSByte(provider);

    /// <inheritdoc />
    public float ToSingle(IFormatProvider provider) => ((IConvertible)ToString()).ToSingle(provider);

    /// <inheritdoc />
    public string ToString(IFormatProvider provider) => ToString();

    /// <inheritdoc />
    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)ToString()).ToType(conversionType, provider);

    /// <inheritdoc />
    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt16(provider);

    /// <inheritdoc />
    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt32(provider);

    /// <inheritdoc />
    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt64(provider);

    #endregion

    #region IEnumerable<char> Members

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if NETCOREAPP1_0 || NETCOREAPP1_1 || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)
    /// <inheritdoc />
    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)ToString()).GetEnumerator();
#else
    /// <inheritdoc />
    public IEnumerator<char> GetEnumerator() => ToString().GetEnumerator();
#endif

    #endregion

    #region IEquatable<string> Members

    /// <inheritdoc />
    public bool Equals(string other) => ToString().Equals(other);

    #endregion

    #region IEquatable<UTF8> Members

    /// <inheritdoc />
    public bool Equals(UTF16BE other) => other is not null && Data.SequenceEqual(other.Data);

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UTF16BE utf16 && Equals(utf16);

    /// <inheritdoc />
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc />
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Codepoints).GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => ConvertToString(Data);

    #endregion
}
