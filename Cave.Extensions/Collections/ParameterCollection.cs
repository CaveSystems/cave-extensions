using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a list implementation for string parameters.</summary>
    [DebuggerDisplay("Count={Count}")]
    public class ParameterCollection : IEnumerable<string>, IEquatable<ParameterCollection>
    {
        readonly string[] items;

        /// <summary>Initializes a new instance of the <see cref="ParameterCollection" /> class.</summary>
        /// <param name="items"></param>
        public ParameterCollection(params string[] items) => this.items = items;

        /// <summary>Gets the number of elements within the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        public int Count => items.Length;

        /// <summary>Gets a value indicating whether the collection is readonly or not.</summary>
        public bool IsReadOnly => true;

        /// <summary>Gets or sets the <see cref="string" /> at the specified index.</summary>
        /// <value>The <see cref="string" />.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index] => items[index];

        /// <summary>Gibt einen Enumerator zur�ck, der die Auflistung durchl�uft.</summary>
        /// <returns>
        ///     Ein <see cref="T:System.Collections.Generic.IEnumerator`1" />, der zum Durchlaufen der Auflistung verwendet
        ///     werden kann.
        /// </returns>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>) items).GetEnumerator();

        /// <summary>Gibt einen Enumerator zur�ck, der eine Auflistung durchl�uft.</summary>
        /// <returns>
        ///     Ein <see cref="T:System.Collections.IEnumerator" />-Objekt, das zum Durchlaufen der Auflistung verwendet
        ///     werden kann.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ParameterCollection other)
        {
            if (other is null)
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                if (!this[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Bestimmt den Index eines bestimmten Elements in der <see cref="T:System.Collections.Generic.IList`1" />.</summary>
        /// <param name="item">Das im <see cref="T:System.Collections.Generic.IList`1" /> zu suchende Objekt.</param>
        /// <returns>Der Index von <paramref name="item" />, wenn das Element in der Liste gefunden wird, andernfalls -1.</returns>
        int IndexOf(string item) => Array.IndexOf(items, item);

        /// <summary>Bestimmt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> einen bestimmten Wert enth�lt.</summary>
        /// <param name="item">Das im <see cref="T:System.Collections.Generic.ICollection`1" /> zu suchende Objekt.</param>
        /// <returns>
        ///     true, wenn sich <paramref name="item" /> in <see cref="T:System.Collections.Generic.ICollection`1" />
        ///     befindet, andernfalls false.
        /// </returns>
        public bool Contains(string item) => IndexOf(item) > -1;

        /// <summary>
        ///     Kopiert die Elemente von <see cref="T:System.Collections.Generic.ICollection`1" /> in ein
        ///     <see cref="T:System.Array" />, beginnend bei einem bestimmten <see cref="T:System.Array" />-Index.
        /// </summary>
        /// <param name="array">
        ///     Das eindimensionale <see cref="T:System.Array" />, das das Ziel der aus
        ///     <see cref="T:System.Collections.Generic.ICollection`1" /> kopierten Elemente ist.F�r <see cref="T:System.Array" />
        ///     muss eine nullbasierte Indizierung verwendet werden.
        /// </param>
        /// <param name="arrayIndex">Der nullbasierte Index in <paramref name="array" />, an dem das Kopieren beginnt.</param>
        void CopyTo(string[] array, int arrayIndex) { items.CopyTo(array, arrayIndex); }

        /// <summary>Gets a string containing all parameters.</summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var parameter in this)
            {
                if (result.Length > 0)
                {
                    result.Append(' ');
                }

                var containsSpace = parameter.IndexOf(' ') >= 0;
                if (containsSpace)
                {
                    result.Append('"');
                }

                result.Append(parameter);
                if (containsSpace)
                {
                    result.Append('"');
                }
            }

            return result.ToString();
        }
    }
}
