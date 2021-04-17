using System;
using System.Collections.Generic;

namespace Cave
{
    /// <summary>Gets a ascii character dictionary (this is used for example at the <see cref="Base64" /> implementation).</summary>
    public sealed class CharacterDictionary
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="CharacterDictionary" /> class.</summary>
        /// <param name="charset">Characters to use as charset.</param>
        public CharacterDictionary(string charset)
        {
            if (charset == null)
            {
                throw new ArgumentNullException(nameof(charset));
            }

            if (charset.Length > 128)
            {
                throw new ArgumentOutOfRangeException(nameof(charset), "Less than or equal to 128 characters expected!");
            }

            for (var i = 0; i < 128; i++)
            {
                Values[i] = -1;
            }

            Charset = charset.ToCharArray();
            for (var i = 0; i < Charset.Length; i++)
            {
                Values[Charset[i]] = i;
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public int Length => Charset.Length;

        #endregion

        #region Public Methods

        /// <summary>Clones the <see cref="CharacterDictionary" />.</summary>
        /// <returns>Returns a copy.</returns>
        public CharacterDictionary Clone() => new(this);

        /// <summary>Gets the character for the specified value.</summary>
        /// <param name="value">The value to look up.</param>
        /// <returns>Returns the character for the value.</returns>
        public char GetCharacter(int value) => Charset[value];

        /// <summary>Gets the value for the specified character.</summary>
        /// <param name="character">The <see cref="char" /> to look up.</param>
        /// <returns>Returns the value (index) for the char.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the symbol could not be found.</exception>
        public int GetValue(char character)
        {
            var result = Values[character];
            if (result < 0)
            {
                throw new KeyNotFoundException($"Invalid symbol '{character}'!");
            }

            return result;
        }

        /// <summary>Tries to get the value for the given character. If no values is available defaultValue will be returned.</summary>
        /// <param name="character">The character.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public int TryGetValue(char character, int defaultValue)
        {
            if ((character < 0) || (character >= Values.Length))
            {
                return defaultValue;
            }

            var result = Values[character];
            return result < 0 ? defaultValue : result;
        }

        #endregion Public Methods

        #region private implementation

        readonly char[] Charset;
        readonly int[] Values = new int[128];

        CharacterDictionary(CharacterDictionary cloneData)
        {
            Charset = (char[])cloneData.Charset.Clone();
            Values = (int[])cloneData.Values.Clone();
        }

        #endregion private implementation
    }
}
