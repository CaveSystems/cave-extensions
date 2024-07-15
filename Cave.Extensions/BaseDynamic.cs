using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Cave;

/// <summary>Provides a dynamic base x class for generating human read and writeable strings with a specified character set.</summary>
public class BaseDynamic : BaseX
{
    #region Public Fields

    /// <summary>Gets the characters used by <see cref="Base36"/> encoding.</summary>
    public const string Base36Characters = "0123456789AbCdeFGHiJkLmNoPqRsTuVwxyz";

    /// <summary>Gets the padding character</summary>
    public const char Base36PaddingCharacter = '~';

    #endregion Public Fields

    /// <summary>Gets the current base</summary>
    public int Base { get; }

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="BaseX"/> class.</summary>
    /// <param name="characters">Character set</param>
    /// <param name="obeyCasing">Obey case during decoding</param>
    /// <param name="padding">Padding character to use</param>
    public BaseDynamic(string characters, bool obeyCasing, char? padding) : base(characters, obeyCasing, padding) => Base = CharacterDictionary.Length;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the default charset for base64 en-/decoding with padding.</summary>
    public static BaseDynamic Base36 => new(Base36Characters, false, Base36PaddingCharacter);

    /// <summary>Gets the default charset for base64 en-/decoding without padding.</summary>
    public static BaseDynamic Base36NoPadding => new(Base36Characters, false, null);

    #endregion Public Properties

    #region Public Methods

    /// <inheritdoc/>
    public override byte[] Decode(byte[] data)
    {
        var decode = data.Select(b => DecodeCharacter((char)b)).ToArray();
        var realSize = (int)(decode.Length / Math.Log(256, Base));
        Array.Reverse(decode);
        var result = new byte[realSize];
        {
            foreach (var symbol in decode)
            {
                var overflow = (int)symbol;
                for (var i = result.Length - 1; i >= 0; i--)
                {
                    var value = (result[i] * Base) + overflow;
                    overflow = value >> 8;
                    result[i] = (byte)value;
                }
            }
        }
        return result;
    }

#if !NET20 && !NET35
    /// <inheritdoc/>
    public override BigInteger DecodeBigInteger(byte[] data)
    {
        BigInteger value = 0;
        foreach (var b in data)
        {
            var c = (char)b;
            if (c == Padding)
            {
                break;
            }
            value = (value * Base) + (int)DecodeCharacter(c);
        }
        return value;
    }
#endif

    /// <inheritdoc/>
    public override long DecodeValue(byte[] data)
    {
        long value = 0;
        foreach (var b in data)
        {
            var c = (char)b;
            if (c == Padding)
            {
                break;
            }
            value = (value * Base) + (int)DecodeCharacter(c);
        }
        return value;
    }

    /// <inheritdoc/>
    public override string Encode(byte[] data)
    {
        data = (byte[])data.Clone();
        var currentBase = CharacterDictionary.Length;
        StringBuilder result = new();
        var lenPerSymbol = Math.Log(256, currentBase);
        var fullLength = data.Length * lenPerSymbol;
        while (result.Length < fullLength)
        {
            var remainder = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var value = (remainder << 8) + data[i];
                remainder = value % currentBase;
                value /= currentBase;
                data[i] = (byte)value;
            }
            result.Append(CharacterDictionary.GetCharacter(remainder));
        }
        return result.ToString();
    }

#if !NET20 && !NET35
    /// <inheritdoc/>
    public override string EncodeBigInteger(BigInteger value)
    {
        var currentBase = CharacterDictionary.Length;
        var result = new List<char>();
        while (value != 0)
        {
            var symbol = (int)(value % currentBase);
            result.Add(EncodeCharacter(symbol));
            value /= currentBase;
        }
        result.Reverse();
        if (Padding is char padding) { result.Add(padding); }
        return new(result.ToArray());
    }
#endif

    /// <inheritdoc/>
    public override string EncodeValue(long value) => EncodeValue((ulong)value);

    /// <inheritdoc/>
    public override string EncodeValue(ulong value)
    {
        var currentBase = (byte)CharacterDictionary.Length;
        var result = new List<char>();
        while (value != 0)
        {
            var symbol = (int)(value % currentBase);
            result.Add(EncodeCharacter(symbol));
            value /= currentBase;
        }
        result.Reverse();
        if (Padding is char padding) { result.Add(padding); }
        return new(result.ToArray());
    }

    #endregion Public Methods
}
