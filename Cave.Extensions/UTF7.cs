using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cave.Collections;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf7. This will reduce the memory usage by about 40-50% on most western languages / ascii based character sets.
/// </summary>
public sealed class UTF7 : IUnicode, IComparable<UTF7>, IEquatable<UTF7>
{
    #region Private Methods

    static string DecodeChunk(byte[] code)
    {
        var data = Base64.NoPadding.Decode(code);
        return Encoding.BigEndianUnicode.GetString(data);
    }

    static string EncodeChunk(string text)
    {
        var data = Encoding.BigEndianUnicode.GetBytes(text);
        return Base64.NoPadding.Encode(data);
    }

    #endregion Private Methods

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="UTF7"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF7(byte[] data) => Data = data;

    /// <summary>Creates a new instance of the <see cref="UTF7"/> class.</summary>
    /// <param name="text">Content</param>
    public UTF7(string text) => Data = Encode(text);

    #endregion Public Constructors

    #region Public Properties

    /// <inheritdoc/>
    public int[] Codepoints => ((UTF32LE)ToString()).Codepoints;

    /// <summary>Gets the data bytes.</summary>
    public byte[] Data { get; }

    /// <summary>Gets the length.</summary>
    /// <value>The length of the string.</value>
    public int Length => ToString().Length;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Provides extended UTF-7 decoding (rfc 3501)</summary>
    public static string Decode(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (!data.Contains((byte)'&'))
        {
            return ASCII.GetString(data);
        }
        var result = new StringBuilder();
        List<byte>? code = null;
        for (var i = 0; i < data.Length; i++)
        {
            if (code != null)
            {
                if (data[i] == '-')
                {
                    var chunk = DecodeChunk(code.ToArray());
                    result.Append(chunk);
                    code = null;
                }
                else
                {
                    code.Add(data[i]);
                }
            }
            else
            {
                if (data[i] == '&')
                {
                    if (data[++i] == '-')
                    {
                        result.Append('&');
                    }
                    else
                    {
                        code = new() { data[i] };
                    }
                }
                else
                {
                    result.Append(data[i]);
                }
            }
        }
        return result.ToString();
    }

    /// <summary>Provides extended UTF-7 encoding (rfc 3501)</summary>
    public static byte[] Encode(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }
        StringBuilder result = new();
        StringBuilder? code = null;
        for (var i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            var isCodeChar = ch is (< (char)0x20 or > (char)0x25) and (< (char)0x27 or > (char)0x7e);
            if (isCodeChar)
            {
                if (ch == '&')
                {
                    if (code != null)
                    {
                        var chunk = EncodeChunk(code.ToString());
                        result.Append($"&{chunk}-&-");
                        code = null;
                    }
                    else
                    {
                        result.Append("&-");
                    }
                }
                else
                {
                    if (code == null)
                    {
                        code = new();
                    }
                    code.Append(ch);
                }
            }
            else
            {
                if (code != null)
                {
                    var chunk = EncodeChunk(code.ToString());
                    result.Append($"&{chunk}-{ch}");
                    code = null;
                }
                else
                {
                    result.Append(ch);
                }
            }
        }
        if (code != null)
        {
            var chunk = EncodeChunk(code.ToString());
            result.Append("&" + chunk + "-");
        }
        return ASCII.GetBytes(result.ToString());
    }

    /// <summary>Performs an implicit conversion from <see cref="UTF7"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF7 s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF7"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF7(string s) => new(s);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(UTF7 s1, UTF7 s2) => s2 is null ? s1 is not null : !s1.Equals(s2);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF7 operator +(UTF7 left, UTF7 right) => new(Encode(left.ToString() + right.ToString()));

    /// <inheritdoc/>
    public static bool operator <(UTF7 left, UTF7 right) => left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(UTF7 left, UTF7 right) => left is null || (left.CompareTo(right) <= 0);

    /// <summary>Implements the operator ==.</summary>
    /// <param name="s1">The s1.</param>
    /// <param name="s2">The s2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(UTF7 s1, UTF7 s2) => s1 is null ? s2 is null : s1.Equals(s2);

    /// <inheritdoc/>
    public static bool operator >(UTF7 left, UTF7 right) => left is not null && (left.CompareTo(right) > 0);

    /// <inheritdoc/>
    public static bool operator >=(UTF7 left, UTF7 right) => left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj is null ? 1 : CompareTo(obj.ToString());

    /// <inheritdoc/>
    public int CompareTo(string? other) => other is null ? 1 : ToString().CompareTo(other);

    /// <inheritdoc/>
    public int CompareTo(UTF7? other) => ToString().CompareTo(other?.ToString());

    /// <inheritdoc/>
    public int CompareTo(IUnicode? other) => other is null ? 1 : DefaultComparer.Combine(Codepoints, other.Codepoints);

    /// <inheritdoc/>
    public IUnicode Concat(string text) => text is null ? this : this + text;

    /// <inheritdoc/>
    public bool Equals(IUnicode? other) => other is not null && Codepoints.Equals(other.Codepoints);

    /// <inheritdoc/>
    public bool Equals(string? other) => ToString().Equals(other, StringComparison.Ordinal);

    /// <inheritdoc/>
    public bool Equals(UTF7? other) => (Length == other?.Length) && Data.SequenceEqual(other.Data);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UTF7 utf7 && Equals(utf7);

    /// <inheritdoc/>
    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)ToString()).GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => DefaultHashingFunction.Calculate(Data);

    /// <inheritdoc/>
    public string ToString(IFormatProvider provider) => ToString();

    /// <inheritdoc/>
    public override string ToString() => Decode(Data);

    #endregion Public Methods
}
