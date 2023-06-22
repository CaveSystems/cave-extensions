#nullable enable

using System;

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf16.</summary>
public sealed class UTF16LE : Unicode<UTF16LE>
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

    /// <summary>Creates a new empty instance of the <see cref="UTF8"/> class.</summary>
    public UTF16LE() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF8"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF16LE(byte[] data) : base(data) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF16LE Empty { get; } = new UTF16LE(ArrayExtension.Empty<byte>());

    /// <inheritdoc/>
    public override int[] Codepoints
    {
        get
        {
            var data = Data;
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
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Performs an implicit conversion from <see cref="UTF16LE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF16LE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF32BE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF16LE(string s) => s == null ? Empty : Empty.FromString(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF16LE operator +(UTF16LE left, UTF16LE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public override UTF16LE FromArray(byte[] data, int start = 0, int length = -1) => new(data.GetRange(start, length));

    /// <inheritdoc/>
    public override UTF16LE FromCodepoints(int[] codepoints, int start = 0, int length = -1) => FromString(ToString(codepoints.GetRange(start, length)));

    /// <inheritdoc/>
    public override UTF16LE FromString(string text)
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

    #endregion Public Methods
}
