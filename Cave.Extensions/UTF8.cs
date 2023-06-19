using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most western languages /
/// ascii based character sets.
/// </summary>
public sealed class UTF8 : IUnicode, IComparable<UTF8>, IEquatable<UTF8>
{
    #region Static

    /// <summary>Converts the specified utf8 data to a csharp string.</summary>
    /// <param name="data">Data to convert.</param>
    /// <returns>Returns a csharp string.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ConvertToString(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        var sb = new StringBuilder();
        foreach (var codepoint in ConvertToCodepoints(data))
        {
            sb.Append(char.ConvertFromUtf32(codepoint));
        }
        return sb.ToString();
    }

    /// <summary>Converts the specified utf8 data to unicode codepoints.</summary>
    /// <param name="data">Data to convert.</param>
    /// <returns>Returns the unicode codepoints.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static IEnumerable<int> ConvertToCodepoints(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        var i = 0;
        while (i < data.Length)
        {
            var b = data[i++];
            if (b < 0x80)
            {
                yield return b;
                continue;
            }

            if (i == data.Length)
            {
                throw new InvalidDataException("Incomplete character at end of buffer!");
            }
            var b2 = data[i++];
            if ((b2 & 0x11000000) == 0x10000000)
            {
                throw new InvalidDataException("Invalid multibyte value!");
            }
            if (b < 0xE0)
            {
                yield return ((b & 0x1F) << 6) | (b2 & 0x3F);
                continue;
            }

            if (i == data.Length)
            {
                throw new InvalidDataException("Incomplete character at end of buffer!");
            }
            var b3 = data[i++];
            if ((b3 & 0x11000000) == 0x10000000)
            {
                throw new InvalidDataException("Invalid multibyte value!");
            }
            if (b < 0xF0)
            {
                yield return ((((b & 0xF) << 6) | (b2 & 0x3F)) << 6) | (b3 & 0x3F);
                continue;
            }

            if (i == data.Length)
            {
                throw new InvalidDataException("Incomplete character at end of buffer!");
            }
            var b4 = data[i++];
            if ((b4 & 0x11000000) == 0x10000000)
            {
                throw new InvalidDataException("Invalid multibyte value!");
            }
            if (b < 0xF8)
            {
                yield return ((((((b & 0x7) << 6) | (b2 & 0x3F)) << 6) | (b3 & 0x3F)) << 6) | (b4 & 0x3F);
                continue;
            }
            //invalid codepoint
            throw new InvalidDataException("Invalid codepoint at utf-8 encoding!");
        }
    }

    /// <summary>Converts the specified <paramref name="text" /> to an <see cref="UTF8" /> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF8" /> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static UTF8 ConvertFromString(string text)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        var i = 0;
        var data = new byte[text.Length * 4];
        var len = 0;
        while (i < text.Length)
        {
            var codepoint = char.ConvertToUtf32(text, i);
            i += char.IsHighSurrogate(text[i]) ? 2 : 1;
            if (codepoint < 0x80)
            {
                data[len++] = (byte)codepoint;
                continue;
            }
            if (codepoint < 0x800)
            {
                data[len++] = (byte)(0xC0 | (codepoint >> 6));
                data[len++] = (byte)(0x80 | (codepoint & 0x3F));
                continue;
            }
            if (codepoint < 0x10000)
            {
                data[len++] = (byte)(0xE0 | (codepoint >> 12));
                data[len++] = (byte)(0x80 | ((codepoint >> 6) & 0x3F));
                data[len++] = (byte)(0x80 | (codepoint & 0x3F));
                continue;
            }
            if (codepoint < 0x110000)
            {
                data[len++] = (byte)(0xF0 | (codepoint >> 18));
                data[len++] = (byte)(0x80 | ((codepoint >> 12) & 0x3F));
                data[len++] = (byte)(0x80 | ((codepoint >> 6) & 0x3F));
                data[len++] = (byte)(0x80 | (codepoint & 0x3F));
                continue;
            }
            throw new NotImplementedException("Codepoints > 0x10ffff are not implemented!");
        }
        Array.Resize(ref data, len);
        return new(data);
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF8 s1, UTF8 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator >(UTF8 left, UTF8 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc />
    public static bool operator >=(UTF8 left, UTF8 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>Performs an implicit conversion from <see cref="UTF8" /> to <see cref="string" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator string(UTF8 s) => s?.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="UTF8" />.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF8(string s) => s == null ? null : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF8 s1, UTF8 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <inheritdoc />
    public static bool operator <(UTF8 left, UTF8 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator <=(UTF8 left, UTF8 right) => left is null || (left.CompareTo(right) <= 0);

    #endregion

    #region Fields

    #endregion

    #region Constructors

    /// <summary>Creates a new instance of the <see cref="UTF8" /> class.</summary>
    /// <param name="data">Content</param>
    public UTF8(byte[] data) => Data = data;

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
    public int CompareTo(UTF8 other) => ToString().CompareTo(other?.ToString());

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
    public bool Equals(UTF8 other) => other is not null && Data.SequenceEqual(other.Data);

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is UTF8 utf8 && Equals(utf8);

    /// <inheritdoc />
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc />
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => ((IEnumerable<int>)Codepoints).GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => ConvertToString(Data);

    #endregion
}
