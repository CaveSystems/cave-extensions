#nullable enable

using System;
using System.Diagnostics;

namespace Cave;

/// <summary>
/// Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most western languages / ascii based character sets.
/// </summary>
public sealed class UTF8 : Unicode
{
    #region Public Constructors

    /// <summary>Creates a new empty instance of the <see cref="UTF8"/> class.</summary>
    public UTF8() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF8(byte[] data) : base(data) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets an empty instance.</summary>
    public static UTF8 Empty { get; } = new();

    /// <inheritdoc/>
    public override byte[] ByteOrderMark => new byte[] { 0xEF, 0xBB, 0xBF };

    /// <inheritdoc/>
    public override int[] Codepoints
    {
        get
        {
            var data = Data;
            var result = new int[data.Length];
            int i = 0, n = 0;

            while (i < data.Length)
            {
                var b = data[i++];
                if (b < 0x80)
                {
                    result[n++] = b;
                    continue;
                }

                if (i == data.Length)
                {
                    Trace.WriteLine("Incomplete character at end of buffer!");
                    break;
                }
                var b2 = data[i++];
                if ((b2 & 0x11000000) == 0x10000000)
                {
                    Trace.WriteLine("Invalid multibyte value!");
                    continue;
                }
                if (b < 0xE0)
                {
                    result[n++] = ((b & 0x1F) << 6) | (b2 & 0x3F);
                    continue;
                }

                if (i == data.Length)
                {
                    Trace.WriteLine("Incomplete character at end of buffer!");
                    break;
                }
                var b3 = data[i++];
                if ((b3 & 0x11000000) == 0x10000000)
                {
                    Trace.WriteLine("Invalid multibyte value!");
                    continue;
                }
                if (b < 0xF0)
                {
                    result[n++] = ((((b & 0xF) << 6) | (b2 & 0x3F)) << 6) | (b3 & 0x3F);
                    continue;
                }

                if (i == data.Length)
                {
                    Trace.WriteLine("Incomplete character at end of buffer!");
                    break;
                }
                var b4 = data[i++];
                if ((b4 & 0x11000000) == 0x10000000)
                {
                    Trace.WriteLine("Invalid multibyte value!");
                    continue;
                }
                if (b < 0xF8)
                {
                    result[n++] = ((((((b & 0x7) << 6) | (b2 & 0x3F)) << 6) | (b3 & 0x3F)) << 6) | (b4 & 0x3F);
                    continue;
                }
                //invalid codepoint
                Trace.WriteLine("Invalid codepoint at utf-8 encoding!");
            }
            Array.Resize(ref result, n);
            return result;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Converts from string to a new <see cref="UTF8"/> instance.</summary>
    /// <param name="text">String to convert.</param>
    /// <returns>Returns a new <see cref="UTF8"/> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
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

    /// <summary>Performs an implicit conversion from <see cref="UTF8"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF8 s) => s.ToString();

    public static byte[] GetBytes(string str) => ConvertFromString(str).Data;

    public static string GetString(byte[] buffer, int offset = 0, int length = -1) => new UTF8(buffer.GetRange(offset, length)).ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF8"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF8(string? s) => s == null ? Empty : ConvertFromString(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF8 operator +(UTF8 left, UTF8 right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public override IUnicode FromArray(byte[] data, int start = 0, int length = -1) => new UTF8(data.GetRange(start, length));

    /// <inheritdoc/>
    public override IUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1)
    {
        if (codepoints is null)
        {
            throw new ArgumentNullException(nameof(codepoints));
        }
        codepoints = codepoints.GetRange(start, length);
        var data = new byte[codepoints.Length * 4];
        var len = 0;
        for (var i = 0; i < codepoints.Length; i++)
        {
            var codepoint = codepoints[i];
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
        return new UTF8(data);
    }

    /// <inheritdoc/>
    public override IUnicode FromString(string text) => ConvertFromString(text);

    #endregion Public Methods
}
