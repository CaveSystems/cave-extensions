#nullable enable

using System;

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf16.</summary>
public sealed class UTF16BE : Unicode
{
    #region Public Constructors

    /// <summary>Creates a new empty instance of the <see cref="UTF16BE"/> class.</summary>
    public UTF16BE() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF16BE"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF16BE(byte[] data) : base(data) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF16BE Empty { get; } = new UTF16BE(ArrayExtension.Empty<byte>());

    /// <inheritdoc/>
    public override byte[] ByteOrderMark => new byte[] { 0xFE, 0xFF };

    /// <summary>Gets the unicode codepoints.</summary>
    public override unsafe int[] Codepoints
    {
        get
        {
            int len = Data.Length / 2, count = 0;
            var codepoints = new int[len];
            var data = Data;
            fixed (byte* p = Data)
            {
                var ptr = (ushort*)p;
                for (var i = 0; i < len;)
                {
                    char GetChar() => (char)(BitConverter.IsLittleEndian ? ptr[i].SwapEndian() : ptr[i]);
                    var c1 = GetChar();
                    i++;
                    if (char.IsHighSurrogate(c1))
                    {
                        var c2 = GetChar();
                        i++;
                        codepoints[count++] = char.ConvertToUtf32(c1, c2);
                    }
                    else
                    {
                        codepoints[count++] = c1;
                    }
                }
            }
            Array.Resize(ref codepoints, count);
            return codepoints;
        }
    }

    #endregion Public Properties

    #region Public Methods

    public static UTF16BE ConvertFromString(string text)
    {
        var chars = text.ToCharArray();
        var data = new byte[chars.Length * 2];
        Buffer.BlockCopy(chars, 0, data, 0, data.Length);
        if (BitConverter.IsLittleEndian)
        {
            data.SwapEndian16();
        }
        return new UTF16BE(data);
    }

    /// <summary>Performs an implicit conversion from <see cref="UTF16BE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF16BE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF16LE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF16BE(string s) => s == null ? Empty : ConvertFromString(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF16BE operator +(UTF16BE left, UTF16BE right) => new(left.Data.Concat(right.Data));

    /// <inheritdoc/>
    public override IUnicode FromArray(byte[] data, int start = 0, int length = -1) => new UTF16BE(data.GetRange(start, length));

    /// <inheritdoc/>
    public override IUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1) => ConvertFromString(ToString(codepoints.GetRange(start, length)));

    /// <inheritdoc/>
    public override IUnicode FromString(string text) => ConvertFromString(text);

    #endregion Public Methods
}
