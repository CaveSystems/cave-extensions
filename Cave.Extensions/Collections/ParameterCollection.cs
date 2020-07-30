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
        readonly string[] items;

        /// <summary>Initializes a new instance of the <see cref="ParameterCollection" /> class.</summary>
        /// <param name="items"></param>
        public ParameterCollection(params string[] items) => this.items = items;

        /// <inheritdoc />
        public int Count => items.Length;

        /// <inheritdoc />
        public bool IsReadOnly => true;

        /// <summary>Gets or sets the <see cref="string" /> at the specified index.</summary>
        /// <value>The <see cref="string" />.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index] => items[index];

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>) items).GetEnumerator();

        /// <inheritdoc />
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
                if (!Equals(this[i], other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public int IndexOf(string item) => Array.IndexOf(items, item);

        /// <inheritdoc />
        public bool Contains(string item) => IndexOf(item) > -1;

        /// <inheritdoc />
        public void CopyTo(string[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

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

        void ICollection<string>.Add(string item) => throw new ReadOnlyException();

        void ICollection<string>.Clear() => throw new ReadOnlyException();

        bool ICollection<string>.Remove(string item) => throw new ReadOnlyException();

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ParameterCollection);

        /// <inheritdoc />
        public override int GetHashCode() => ToString().GetHashCode();
    }
}
