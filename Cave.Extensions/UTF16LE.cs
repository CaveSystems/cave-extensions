using System;
using System.IO;
using System.Linq;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf16.</summary>
public sealed class UTF16LE : IUnicode, IComparable<UTF16LE>, IEquatable<UTF16LE>
{
    #region Private Methods

    static char[] ConvertToChars(byte[] data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (!BitConverter.IsLittleEndian)
        {
            data = (byte[])data.Clone();
            data.SwapEndian16();
        }
        var chars = new char[data.Length / 2];
        Buffer.BlockCopy(data, 0, chars, 0, data.Length);
        return chars;
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF16LE(byte[] data) => Data = data;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF16LE Empty { get; } = new UTF16LE(new byte[0]);

    /// <summary>Gets the unicode codepoints.</summary>
    public int[] Codepoints => ConvertToCodepoints(Data).ToArray();

    /// <summary>Gets the data bytes.</summary>
    public byte[] Data { get; }

    /// <summary>Gets the string length or csharp character count.</summary>
    /// <remarks>
    /// This is not the number of unicode characters since csharp uses utf16 with high and low surrogate pairs to represent non BMP (Basic Multilingual Plane) characters.
    /// </remarks>
    public int Length => ToString().Length;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Converts the specified <paramref name="text"/> to an <see cref="UTF16LE"/> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF16LE"/> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static UTF16LE ConvertFromString(string text)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var chars = text.ToCharArray();
        var data = new byte[chars.Length * 2];
        Buffer.BlockCopy(chars, 0, data, 0, data.Length);
        if (!BitConverter.IsLittleEndian)
        {
            data.SwapEndian16();
        }
        return new(data);
    }

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

    /// <summary>Converts the specified utf16 data to a csharp string.</summary>
    /// <param name="data">Data to convert.</param>
    /// <returns>Returns a csharp string.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ConvertToString(byte[] data) => new(ConvertToChars(data));

    /// <summary>Performs an implicit conversion from <see cref="UTF16LE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF16LE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF32BE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF16LE(string s) => s == null ? Empty : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF16LE s1, UTF16LE s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF16LE operator +(UTF16LE left, UTF16LE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public static bool operator <(UTF16LE left, UTF16LE right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(UTF16LE left, UTF16LE right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF16LE s1, UTF16LE s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(UTF16LE left, UTF16LE right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(UTF16LE left, UTF16LE right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is null ? 1 : CompareTo(obj.ToString());

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : ToString().CompareTo(other);

    /// <inheritdoc/>
    public int CompareTo(UTF16LE? other) => other is null ? 1 : ToString().CompareTo(other.ToString());

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public IUnicode Concat(string text) => text is null ? this : this + text;

    /// <inheritdoc/>
    public bool Equals(string? other) => other is not null && ToString().Equals(other);

    /// <inheritdoc/>
    public bool Equals(UTF16LE? other) => other is not null && Data.SequenceEqual(other.Data);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UTF16LE utf16 && Equals(utf16);

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && Codepoints.Equals(other.Codepoints);

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public TypeCode GetTypeCode() => TypeCode.String;

    /// <inheritdoc/>
    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)ToString()).ToBoolean(provider);

    /// <inheritdoc/>
    public byte ToByte(IFormatProvider provider) => ((IConvertible)ToString()).ToByte(provider);

    /// <inheritdoc/>
    public char ToChar(IFormatProvider provider) => ((IConvertible)ToString()).ToChar(provider);

    /// <inheritdoc/>
    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)ToString()).ToDateTime(provider);

    /// <inheritdoc/>
    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)ToString()).ToDecimal(provider);

    /// <inheritdoc/>
    public double ToDouble(IFormatProvider provider) => ((IConvertible)ToString()).ToDouble(provider);

    /// <inheritdoc/>
    public short ToInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToInt16(provider);

    /// <inheritdoc/>
    public int ToInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToInt32(provider);

    /// <inheritdoc/>
    public long ToInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToInt64(provider);

    /// <inheritdoc/>
    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)ToString()).ToSByte(provider);

    /// <inheritdoc/>
    public float ToSingle(IFormatProvider provider) => ((IConvertible)ToString()).ToSingle(provider);

    /// <inheritdoc/>
    public string ToString(IFormatProvider provider) => ToString();

    /// <inheritdoc/>
    public override string ToString() => ConvertToString(Data);

    /// <inheritdoc/>
    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)ToString()).ToType(conversionType, provider);

    /// <inheritdoc/>
    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt16(provider);

    /// <inheritdoc/>
    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt32(provider);

    /// <inheritdoc/>
    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)ToString()).ToUInt64(provider);

    #endregion Public Methods
}
