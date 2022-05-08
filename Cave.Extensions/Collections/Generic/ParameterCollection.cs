using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a list implementation for string parameters.</summary>
    [DebuggerDisplay("Count={Count}")]
    public class ParameterCollection : IEnumerable<string>, IEquatable<ParameterCollection>, ICollection<string>
    {
        #region Private Fields

        readonly string[] items;

        #endregion Private Fields

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ParameterCollection"/> class.</summary>
        /// <param name="items"></param>
        public ParameterCollection(params string[] items) => this.items = items;

        #endregion Constructors

        #region Properties

        /// <summary>Gets or sets the <see cref="string"/> at the specified index.</summary>
        /// <value>The <see cref="string"/>.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index] => items[index];

        #endregion Properties

        #region ICollection<string> Members

        /// <inheritdoc/>
        public int Count => items.Length;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public bool Contains(string item) => IndexOf(item) > -1;

        /// <inheritdoc/>
        public void CopyTo(string[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        void ICollection<string>.Add(string item) => throw new ReadOnlyException();

        void ICollection<string>.Clear() => throw new ReadOnlyException();

        bool ICollection<string>.Remove(string item) => throw new ReadOnlyException();

        #endregion ICollection<string> Members

        #region IEnumerable<string> Members

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)items).GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        #endregion IEnumerable<string> Members

        #region IEquatable<ParameterCollection> Members

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
                if (!Equals(this[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion IEquatable<ParameterCollection> Members

        #region Overrides

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as ParameterCollection);

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

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

                var containsSpace = parameter.Contains(' ');
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

        #endregion Overrides

        #region Members

        /// <inheritdoc/>
        public int IndexOf(string item) => Array.IndexOf(items, item);

        #endregion Members
    }
}
