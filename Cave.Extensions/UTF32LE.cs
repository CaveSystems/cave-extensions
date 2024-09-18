#nullable enable

using System;

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf32.</summary>
public sealed class UTF32LE : Unicode
{
    #region Public Constructors

    /// <summary>Creates a new empty instance of the <see cref="UTF32LE"/> class.</summary>
    public UTF32LE() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF32LE"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF32LE(byte[] data) : base(data) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF32LE Empty { get; } = new UTF32LE([]);

    /// <inheritdoc/>
    public override byte[] ByteOrderMark => new byte[] { 0xFF, 0xFE, 0x00, 0x00 };

    /// <inheritdoc/>
    public override int[] Codepoints
    {
        get
        {
            var data = Data;
            if (!BitConverter.IsLittleEndian)
            {
                data = (byte[])data.Clone();
                data.SwapEndian32();
            }
            var result = new int[data.Length / 4];
            Buffer.BlockCopy(data, 0, result, 0, result.Length * 4);
            return result;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Converts from string to a new <see cref="UTF32LE"/> instance.</summary>
    /// <param name="text">String to convert.</param>
    /// <returns>Returns a new <see cref="UTF32LE"/> instance.</returns>
    /// <exception cref="ArgumentNullException"></exception>
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

    /// <summary>Performs an implicit conversion from <see cref="UTF32LE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF32LE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF32LE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF32LE(string? s) => s == null ? UTF32LE.Empty : ConvertFromString(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF32LE operator +(UTF32LE left, UTF32LE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public override IUnicode FromArray(byte[] data, int start = 0, int length = -1) => new UTF32LE(data.GetRange(start, length));

    /// <inheritdoc/>
    public override IUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1)
    {
        codepoints = codepoints.GetRange(start, length);
        var data = new byte[codepoints.Length * 4];
        Buffer.BlockCopy(codepoints, 0, data, 0, data.Length);
        if (!BitConverter.IsLittleEndian)
        {
            data.SwapEndian32();
        }
        return new UTF32LE(data);
    }

    /// <inheritdoc/>
    public override IUnicode FromString(string text) => ConvertFromString(text);

    #endregion Public Methods
}
