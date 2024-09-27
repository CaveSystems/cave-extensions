

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cave;

/// <summary>Provides a string encoded on the heap using utf7.</summary>
public sealed class UTF7 : Unicode
{
    #region Public Constructors

    /// <summary>Creates a new empty instance of the <see cref="UTF7"/> class.</summary>
    public UTF7() : base() { }

    /// <summary>Creates a new instance of the <see cref="UTF7"/> class.</summary>
    /// <param name="data">Content</param>
    public UTF7(byte[] data) : base(data) { }

    /// <summary>Creates a new instance of the <see cref="UTF7"/> class.</summary>
    /// <param name="text">Content</param>
    public UTF7(string text) : base(Encode(text)) { }

    #endregion Public Constructors

    #region Public Properties

    /// <inheritdoc/>
    public override byte[] ByteOrderMark => new byte[] { 0x2B, 0x2F, 0x76 };

    /// <inheritdoc/>
    public override int[] Codepoints
    {
        get
        {
            var data = Data;
            var result = new int[data.Length];
            var len = 0;
            List<byte>? code = null;
            for (var i = 0; i < data.Length; i++)
            {
                if (code != null)
                {
                    if (data[i] == '-')
                    {
                        var chunk = DecodeChunk([.. code]);
                        result[len++] = char.ConvertToUtf32(chunk, 0);
                        code = null;
                    }
                    else
                    {
                        code.Add(data[i]);
                    }
                }
                else
                {
                    if (data[i] == '+')
                    {
                        if (data[++i] == '-')
                        {
                            result[len++] = '+';
                        }
                        else
                        {
                            code = [data[i]];
                        }
                    }
                    else
                    {
                        result[len++] = data[i];
                    }
                }
            }
            Array.Resize(ref result, len);
            return result;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Provides extended UTF-7 decoding (rfc 3501)</summary>
    public static string Decode(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (!data.Contains((byte)'+'))
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
                    var chunk = DecodeChunk([.. code]);
                    _ = result.Append(chunk);
                    code = null;
                }
                else
                {
                    code.Add(data[i]);
                }
            }
            else
            {
                if (data[i] == '+')
                {
                    if (data[++i] == '-')
                    {
                        _ = result.Append('+');
                    }
                    else
                    {
                        code = [data[i]];
                    }
                }
                else
                {
                    _ = result.Append((char)data[i]);
                }
            }
        }
        return result.ToString();
    }

    /// <summary>Provides extended UTF-7 decoding (rfc 3501)</summary>
    public static string DecodeChunk(byte[] code)
    {
        var data = Base64.NoPadding.Decode(code);
        return new UTF16BE(data).ToString();
    }

    /// <summary>Provides extended UTF-7 encoding (rfc 2152)</summary>
    /// <param name="text">Text to be encoded</param>
    public static byte[] Encode(string text) => Encode(text, false);

    /// <summary>Provides extended UTF-7 encoding (rfc 2152)</summary>
    /// <param name="text">Text to be encoded</param>
    /// <param name="encodeOptionalCharacters">Obey (rfc 3501) restrictions on utf-7 text used at mail headers</param>
    public static byte[] Encode(string text, bool encodeOptionalCharacters)
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
            var encodeChar = encodeOptionalCharacters ?
                (int)ch is (>= 39 and <= 41) or (>= 44 and <= 58) or 63 or (>= 65 and <= 90) or (>= 97 and <= 122) :
                (int)ch is (>= 32 and <= 42) or (>= 44 and <= 125);
            if (!encodeChar)
            {
                if (ch == '+')
                {
                    if (code != null)
                    {
                        var chunk = EncodeChunk(code.ToString());
                        _ = result.Append($"+{chunk}-+-");
                        code = null;
                    }
                    else
                    {
                        _ = result.Append("+-");
                    }
                }
                else
                {
                    code ??= new();
                    _ = code.Append(ch);
                }
            }
            else
            {
                if (code != null)
                {
                    var chunk = EncodeChunk(code.ToString());
                    _ = result.Append($"+{chunk}-{ch}");
                    code = null;
                }
                else
                {
                    _ = result.Append(ch);
                }
            }
        }
        if (code != null)
        {
            var chunk = EncodeChunk(code.ToString());
            _ = result.Append($"+{chunk}-");
        }
        return ASCII.GetBytes(result.ToString());
    }

    /// <summary>Provides extended UTF-7 encoding (rfc 3501)</summary>
    public static string EncodeChunk(string text)
    {
        var data = ((UTF16BE)text).Data;
        return Base64.NoPadding.Encode(data);
    }

    /// <summary>Performs an implicit conversion from <see cref="UTF7"/> to <see cref="string"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator string(UTF7 s) => s.ToString();

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="UTF7"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator UTF7(string s) => new(s);

    /// <summary>Concatenates two strings.</summary>
    /// <param name="left">First string.</param>
    /// <param name="right">Second string.</param>
    /// <returns>Returns a new instance.</returns>
    public static UTF7 operator +(UTF7 left, UTF7 right) => new(Encode(left.ToString() + right.ToString()));

    /// <inheritdoc/>
    public override IUnicode FromArray(byte[] data, int start = 0, int length = -1) => new UTF7(data.GetRange(start, length));

    /// <inheritdoc/>
    public override IUnicode FromCodepoints(int[] codepoints, int start = 0, int length = -1) => new UTF7(ToString(codepoints.GetRange(start, length)));

    /// <inheritdoc/>
    public override IUnicode FromString(string text) => new UTF7(text);

    /// <inheritdoc/>
    public override string ToString() => Decode(Data);

    #endregion Public Methods
}
