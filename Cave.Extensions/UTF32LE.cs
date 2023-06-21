using System;
using System.IO;
using System.Linq;
using System.Text;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf32.</summary>
public sealed class UTF32LE : IUnicode, IComparable<UTF32LE>, IEquatable<UTF32LE>
{
    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="UTF32LE"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF32LE(byte[] data) => Data = data;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF32LE Empty { get; } = new UTF32LE(new byte[0]);

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

    /// <summary>Converts the specified <paramref name="text"/> to an <see cref="UTF32LE"/> instance.</summary>
    /// <param name="text">Text to convert.</param>
    /// <returns>Returns a new <see cref="UTF32LE"/> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static unsafe UTF32LE ConvertFromString(string text)
    {
        if (text is null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        var data = new byte[text.Length * 4];
        fixed (byte* ptr = data)
        {
            var pointer = (int*)ptr;
            var i = 0;
            var len = 0;
            while (i < text.Length)
            {
                var codepoint = char.ConvertToUtf32(text, i);
                i += char.IsHighSurrogate(text[i]) ? 2 : 1;
                pointer[len++] = codepoint;
            }
            Array.Resize(ref data, len * 4);
        }
        if (!BitConverter.IsLittleEndian)
        {
            data.SwapEndian32();
        }
        return new(data);
    }

    /// <summary>Converts the specified utf32 data to unicode codepoints.</summary>
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
        if (!BitConverter.IsLittleEndian)
        {
            data = (byte[])data.Clone();
            data.SwapEndian32();
        }
        var result = new int[data.Length / 4];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);
        return result;
    }

    /// <summary>Converts the specified utf32 data to a csharp string.</summary>
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

    /// <summary>Performs an implicit conversion from <see cref="UTF32LE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF32LE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF32LE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF32LE(string s) => s == null ? UTF32LE.Empty : ConvertFromString(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF32LE s1, UTF32LE s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF32LE operator +(UTF32LE left, UTF32LE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public static bool operator <(UTF32LE left, UTF32LE right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(UTF32LE left, UTF32LE right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF32LE s1, UTF32LE s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(UTF32LE left, UTF32LE right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(UTF32LE left, UTF32LE right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is null ? 1 : CompareTo(obj.ToString());

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : ToString().CompareTo(other);

    /// <inheritdoc/>
    public int CompareTo(UTF32LE? other) => ToString().CompareTo(other?.ToString());

    /// <inheritdoc/>
    public IUnicode Concat(string text) => text is null ? this : this + text;

    /// <inheritdoc/>
    public bool Equals(string? other) => ToString().Equals(other);

    /// <inheritdoc/>
    public bool Equals(UTF32LE? other) => other is not null && Data.SequenceEqual(other.Data);

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && Codepoints.Equals(other.Codepoints);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UTF32LE utf32 && Equals(utf32);

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public override string ToString() => ConvertToString(Data);

    #endregion Public Methods
}
