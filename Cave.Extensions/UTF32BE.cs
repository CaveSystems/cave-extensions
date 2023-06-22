using System;

#nullable enable

#pragma warning disable CA1710

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf32.</summary>
public sealed class UTF32BE : Unicode<UTF32BE>
{
    #region Public Constructors

    /// <summary>Creates a new empty instance of the <see cref="UTF32BE"/> class.</summary>
    public UTF32BE() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF32BE"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF32BE(byte[] data) : base(data) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the empty instance.</summary>
    public static UTF32BE Empty { get; } = new UTF32BE(ArrayExtension.Empty<byte>());

    #endregion Public Properties

    /// <inheritdoc/>
    public override unsafe UTF32BE FromString(string text)
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
        if (BitConverter.IsLittleEndian)
        {
            data.SwapEndian32();
        }
        return new(data);
    }

    /// <inheritdoc/>
    public override int[] Codepoints
    {
        get
        {
            var data = Data;
            if (BitConverter.IsLittleEndian)
            {
                data = (byte[])data.Clone();
                data.SwapEndian32();
            }
            var result = new int[data.Length / 4];
            Buffer.BlockCopy(data, 0, result, 0, data.Length);
            return result;
        }
    }

    /// <inheritdoc/>
    public override UTF32BE FromArray(byte[] data, int start = 0, int length = -1) => new(data.GetRange(start, length));

    /// <inheritdoc/>
    public override UTF32BE FromCodepoints(int[] codepoints, int start = 0, int length = -1)
    {
        codepoints = codepoints.GetRange(start, length);
        var data = new byte[codepoints.Length * 4];
        Buffer.BlockCopy(codepoints, 0, data, 0, data.Length);
        if (BitConverter.IsLittleEndian)
        {
            data.SwapEndian32();
        }
        return new UTF32BE(data);
    }

    /// <summary>Performs an implicit conversion from <see cref="UTF32BE"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF32BE s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF32BE"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF32BE(string s) => s == null ? UTF32BE.Empty : Empty.FromString(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF32BE operator +(UTF32BE left, UTF32BE right) => new(left.Data.Concat(right.Data));
}
