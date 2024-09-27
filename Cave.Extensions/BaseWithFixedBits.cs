using System;

namespace Cave;

/// <summary>Provides abstract functions for all bit based base conversions (1, 2, .., 32, 64, ...).</summary>
public abstract class BaseWithFixedBits : BaseX
{
    #region Protected Constructors

    /// <summary>Initializes a new instance of the <see cref="BaseWithFixedBits"/> class.</summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="bitCount">The bit count.</param>
    /// <param name="padding">The padding character used.</param>
    /// <exception cref="ArgumentOutOfRangeException">Invalid dictionary length or bit count.</exception>
    protected BaseWithFixedBits(CharacterDictionary dictionary, int bitCount, char? padding) : base(dictionary, padding)
    {
        if (bitCount is < 1 or > 32)
        {
            throw new ArgumentOutOfRangeException(nameof(bitCount), "BitCount in range 1..32 required!");
        }

        BitsPerCharacter = bitCount;
        if (dictionary.Length != (1 << bitCount))
        {
            throw new ArgumentOutOfRangeException(nameof(dictionary), "Invalid dictionary length!");
        }
    }

    #endregion Protected Constructors

    #region Public Properties

    /// <summary>Gets the bits per character.</summary>
    /// <value>The bits per character.</value>
    public int BitsPerCharacter { get; }

    #endregion Public Properties
}
