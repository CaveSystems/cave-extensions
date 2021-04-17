using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
                if ((c < 1) || (c > 127))
                {
                    throw new InvalidDataException($"Invalid character 0x{(int)c:x}!");
                }

                Charset[Count] = c;
                Values[c] = ++Count;
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
        [SuppressMessage("Design", "CA1043")]
        public int this[char c]
        {
            get
            {
                if (Count != 64)
                {
                    throw new ArgumentException("Dictionary does not contain 64 key valid combinations!");
                }

                var result = Values[c] - 1;
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
        public char this[int value] => Charset[value];

        #endregion Public Indexers

        #region private implementation

        readonly char[] Charset = new char[64];
        readonly int Count;
        readonly int[] Values = new int[128];

        Base64Dictionary(Base64Dictionary source)
        {
            Charset = (char[])source.Charset.Clone();
            Values = (int[])source.Values.Clone();
            Count = source.Count;
        }

        #endregion private implementation
    }
}
