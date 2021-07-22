using System;
using System.Collections.Generic;
using System.IO;

namespace Cave
{
    /// <summary>Gets a dictionary for <see cref="Base64" /> implementations.</summary>
    public sealed class Base64Dictionary
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Base64Dictionary" /> class.</summary>
        public Base64Dictionary(string charset)
        {
            if (charset == null)
            {
                throw new ArgumentNullException(nameof(charset));
            }

            if (charset.Length != 64)
            {
                throw new ArgumentOutOfRangeException(nameof(charset), "Charset of 64 7 bit ascii characters expected!");
            }

            foreach (var c in charset)
            {
                if (c is < (char)1 or > (char)127)
                {
                    throw new InvalidDataException($"Invalid character 0x{(int)c:x}!");
                }

                this.charset[count] = c;
                values[c] = ++count;
            }
        }

        #endregion

        #region Members

        #region ICloneable Member

        /// <summary>Clones the <see cref="Base64Dictionary" />.</summary>
        /// <returns>Returns a copy.</returns>
        public Base64Dictionary Clone() => new(this);

        #endregion ICloneable Member

        #endregion

        #region Public Indexers

        /// <summary>Gets the value for the specified character.</summary>
        /// <param name="c">The <see cref="char" /> to look up.</param>
        /// <returns>Returns the value (index) for the char.</returns>
        /// <exception cref="ArgumentException">Thrown if the dictionary was not jet completed.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the symbol could not be found.</exception>
        public int this[char c]
        {
            get
            {
                if (count != 64)
                {
                    throw new ArgumentException("Dictionary does not contain 64 key valid combinations!");
                }

                var result = values[c] - 1;
                if (result < 0)
                {
                    throw new KeyNotFoundException($"Invalid symbol '{c}'!");
                }

                return result;
            }
        }

        /// <summary>Gets the character for the specified value.</summary>
        /// <param name="value">The value to look up.</param>
        /// <returns>Returns the character for the value.</returns>
        public char this[int value] => charset[value];

        #endregion Public Indexers

        #region private implementation

        readonly char[] charset = new char[64];
        readonly int count;
        readonly int[] values = new int[128];

        Base64Dictionary(Base64Dictionary source)
        {
            charset = (char[])source.charset.Clone();
            values = (int[])source.values.Clone();
            count = source.count;
        }

        #endregion private implementation
    }
}
