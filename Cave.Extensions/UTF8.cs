using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most western languages / ascii based character sets.
/// </summary>
public sealed class UTF8 : IUnicode, IComparable<UTF8>, IEquatable<UTF8>
{
    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF8(byte[] data) => Data = data;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF8 Empty { get; } = new UTF8(ArrayExtension.Empty<byte>());

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

    /// <summary>Converts the specified <paramref name="text"/> to an <see cref="UTF8"/> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF8"/> instance.</returns>
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
            _ = sb.Append(char.ConvertFromUtf32(codepoint));
        }
        return sb.ToString();
    }

    /// <summary>Performs an implicit conversion from <see cref="UTF8"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF8 s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF8"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF8(string? s) => s == null ? UTF8.Empty : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF8 s1, UTF8 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF8 operator +(UTF8 left, UTF8 right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public static bool operator <(UTF8 left, UTF8 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(UTF8 left, UTF8 right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF8 s1, UTF8 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(UTF8 left, UTF8 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(UTF8 left, UTF8 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is null ? 1 : CompareTo(obj.ToString());

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : ToString().CompareTo(other);

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public int CompareTo(UTF8? other) => ToString().CompareTo(other?.ToString());

    /// <inheritdoc/>
    public IUnicode Concat(string text) => text is null ? this : this + text;

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && Codepoints.Equals(other.Codepoints);

    /// <inheritdoc/>
    public bool Equals(string? other) => ToString().Equals(other);

    /// <inheritdoc/>
    public bool Equals(UTF8? other) => other is not null && Data.SequenceEqual(other.Data);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UTF8 utf8 && Equals(utf8);

    /// <inheritdoc/>
    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)ToString()).GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public override string ToString() => ConvertToString(Data);

    #endregion Public Methods
}
