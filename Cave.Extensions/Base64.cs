using System;
using System.Collections.Generic;

namespace Cave;

/// <summary>Gets Base64 en-/decoding.</summary>
public class Base64 : BaseWithFixedBits
{
    #region Private Fields

    const int bitCount = 6;

    #endregion Private Fields

    #region Public Fields

    /// <summary>Gets the characters used by <see cref="Default"/> encoding.</summary>
    public const string CharactersDefault = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    /// <summary>Gets the characters used by <see cref="UrlChars"/> encoding.</summary>
    public const string CharactersUrl = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    #endregion Public Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="Base64"/> class.</summary>
    /// <param name="dict">The dictionary containing 64 ascii characters used for encoding.</param>
    /// <param name="padding">The padding (use null to skip padding).</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException">Invalid padding character.</exception>
    public Base64(CharacterDictionary dict, char? padding) : base(dict, bitCount, padding) { }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the default charset for base64 en-/decoding with padding.</summary>
    public static Base64 Default => new(new(CharactersDefault, true), '=');

    /// <summary>Gets the default charset for base64 en-/decoding without padding.</summary>
    public static Base64 NoPadding => new(new(CharactersDefault, true), null);

    /// <summary>Gets the url safe charset for base64 en-/decoding (no padding).</summary>
    public static Base64 UrlChars => new(new(CharactersUrl, true), null);

    #endregion Public Properties

    #region Public Methods

    /// <summary>Decodes a base64 data array.</summary>
    /// <param name="data">The base64 data to decode.</param>
    public override byte[] Decode(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (Padding != null)
        {
            int paddingChar = (char)Padding;
            if (paddingChar is < 0 or > 127)
            {
                throw new InvalidOperationException("Invalid padding character!");
            }
        }

        // decode data
        var result = new List<byte>(data.Length);
        var value = 0;
        var bits = 0;
        foreach (var b in data)
        {
            if (b == Padding)
            {
                break;
            }

            value <<= bitCount;
            bits += bitCount;
            value |= CharacterDictionary.GetValue((char)b);
            if (bits >= 8)
            {
                bits -= 8;
                var outValue = value >> bits;
                value &= ~(0xFFFF << bits);
                result.Add((byte)outValue);
            }
        }
        return result.ToArray();
    }

    /// <summary>Encodes the specified data.</summary>
    /// <param name="data">The data to encode.</param>
    public override string Encode(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }
        var result = new List<char>(data.Length * 2);
        var value = 0;
        var bits = 0;
        foreach (var b in data)
        {
            value = (value << 8) | b;
            bits += 8;
            while (bits >= bitCount)
            {
                bits -= bitCount;
                var outValue = value >> bits;
                value &= ~(0xFFFF << bits);
                result.Add(CharacterDictionary.GetCharacter(outValue));
            }
        }

        if (bits > bitCount)
        {
            bits -= bitCount;
            var outValue = value >> bits;
            value &= ~(0xFFFF << bits);
            result.Add(CharacterDictionary.GetCharacter(outValue));
        }

        if (bits > 0)
        {
            var shift = bitCount - bits;
            var outValue = value << shift;
            result.Add(CharacterDictionary.GetCharacter(outValue));
            bits -= bitCount;
        }

        if (Padding != null)
        {
            var padding = (char)Padding;
            while ((bits % 8) != 0)
            {
                result.Add(padding);
                bits -= bitCount;
            }
        }

        return new(result.ToArray());
    }

    #endregion Public Methods
}
