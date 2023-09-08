using System;
using System.Collections.Generic;
using System.Linq;

namespace Cave;

/// <summary>Gets Base32 en-/decoding.</summary>
public class Base32 : BaseWithFixedBits
{
    #region Private Fields

    const int bitCount = 5;

    #endregion Private Fields

    #region Protected Constructors

    /// <summary>Initializes a new instance of the <see cref="Base32"/> class.</summary>
    /// <param name="dictionary">The dictionary containing 64 ascii characters used for encoding.</param>
    /// <param name="padding">The padding (use null to skip padding).</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    protected Base32(CharacterDictionary dictionary, char? padding) : base(dictionary, bitCount, padding) { }

    #endregion Protected Constructors

    #region Public Fields

    /// <summary>Gets the characters used by <see cref="Default"/> encoding.</summary>
    public const string CharactersDefault = "0123456789ABCDEFGHIJKLMNOPQRSTUV";

    /// <summary>Gets the characters used by <see cref="OTP"/> encoding.</summary>
    public const string CharactersOTP = "abcdefghijklmnopqrstuvwxyz234567";

    /// <summary>Gets the characters used by <see cref="Safe"/> encoding.</summary>
    public const string CharactersSafe = "abcdefghjkmnopqrstuwxyz123456789";

    #endregion Public Fields

    #region Public Properties

    /// <summary>Gets the default (uppercase) charset for base32 en-/decoding with padding.</summary>
    public static Base32 Default => new(new(CharactersDefault, false), '=');

    /// <summary>Gets the default (uppercase) charset for Base32 en-/decoding without padding.</summary>
    public static Base32 NoPadding => new(new(CharactersDefault, false), null);

    /// <summary>Gets the otp charset for Base32 en-/decoding (no padding).</summary>
    public static Base32 OTP => new(new(CharactersOTP, true), null);

    /// <summary>Gets the url safe dictatable (no i,l,v,0) charset for Base32 en-/decoding (no padding).</summary>
    public static Base32 Safe => new(new(CharactersSafe, false), null);

    #endregion Public Properties

    #region Public Methods

    /// <summary>Decodes a Base32 data array.</summary>
    /// <param name="data">The Base32 data to decode.</param>
    public override byte[] Decode(byte[] data)
    {
        if (CharacterDictionary == null)
        {
            throw new InvalidOperationException($"Property {nameof(CharacterDictionary)} has to be set!");
        }

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

        if (bits >= bitCount)
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
