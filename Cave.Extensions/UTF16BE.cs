using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf16.</summary>
public sealed class UTF16BE : IUnicode, IComparable<UTF16BE>, IEquatable<UTF16BE>
{
    #region Private Methods

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

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="UTF16BE"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF16BE(byte[] data) => Data = data;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF16BE Empty { get; } = new UTF16BE(ArrayExtension.Empty<byte>());

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

    /// <summary>Converts the specified <paramref name="text"/> to an <see cref="UTF16BE"/> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF16BE"/> instance.</returns>
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

    /// <summary>Performs an implicit conversion from <see cref="UTF16BE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF16BE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF16LE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF16BE(string s) => s == null ? Empty : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF16BE s1, UTF16BE s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF16BE operator +(UTF16BE left, UTF16BE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public static bool operator <(UTF16BE left, UTF16BE right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(UTF16BE left, UTF16BE right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF16BE s1, UTF16BE s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(UTF16BE left, UTF16BE right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(UTF16BE left, UTF16BE right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is null ? 1 : CompareTo(obj.ToString());

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : ToString().CompareTo(other);

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public int CompareTo(UTF16BE? other) => ToString().CompareTo(other?.ToString());

    /// <inheritdoc/>
    public IUnicode Concat(string text) => text is null ? this : this + text;

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && Codepoints.Equals(other.Codepoints);

    /// <inheritdoc/>
    public bool Equals(string? other) => ToString().Equals(other);

    /// <inheritdoc/>
    public bool Equals(UTF16BE? other) => other is not null && Data.SequenceEqual(other.Data);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UTF16BE utf16 && Equals(utf16);

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public override string ToString() => ConvertToString(Data);

    #endregion Public Methods
}
