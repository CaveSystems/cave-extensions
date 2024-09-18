#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20

using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>Gets a generic typed set of objects.</summary>
    [DebuggerDisplay("Count={Count}")]
    public sealed class HashSet<T> : ISet<T>
    {
        #region Fields

        #region private Member

        Dictionary<T, byte> dict = [];

        #endregion private Member

        #endregion Fields

        #region ISet<T> Members

        #region ICollection<T> Member

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        #endregion ICollection<T> Member

        #region IEnumerable Member

        /// <summary>Gets an <see cref="IEnumerator"/> for this set.</summary>
        IEnumerator IEnumerable.GetEnumerator() => dict.Keys.GetEnumerator();

        #endregion IEnumerable Member

        #region IEnumerable<T> Member

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();

        #endregion IEnumerable<T> Member

        #endregion ISet<T> Members

        #region Members

        #region ICloneable Member

        /// <summary>Creates a copy of this set.</summary>
        public object Clone() => new HashSet<T>(dict.Keys);

        #endregion ICloneable Member

        /// <summary>Gets an array of all elements present.</summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            var result = new T[Count];
            CopyTo(result, 0);
            return result;
        }

        [MethodImpl((MethodImplOptions)256)]
        void Include(T item) => dict[item] = 1;

        [MethodImpl((MethodImplOptions)256)]
        void IncludeRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Include(item);
            }
        }

        #endregion Members

        #region constructors

        /// <summary>Initializes a new instance of the <see cref="HashSet{T}"/> class.</summary>
        public HashSet() { }

        /// <summary>Initializes a new instance of the <see cref="HashSet{T}"/> class.</summary>
        public HashSet(params T[] items) : this() => IncludeRange(items);

        /// <summary>Initializes a new instance of the <see cref="HashSet{T}"/> class.</summary>
        public HashSet(IEnumerable<T> items) : this() => IncludeRange(items);

        /// <summary>Initializes a new instance of the <see cref="HashSet{T}"/> class.</summary>
        public HashSet(params IEnumerable<T>[] blocks)
            : this()
        {
            foreach (var items in blocks)
            {
                IncludeRange(items);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="HashSet{T}"/> class.</summary>
        public HashSet(T item, params IEnumerable<T>[] blocks)
            : this()
        {
            Include(item);
            foreach (var items in blocks)
            {
                IncludeRange(items);
            }
        }

        #endregion constructors

        #region public Member

        /// <inheritdoc/>
        public bool Contains(T item) => dict.ContainsKey(item);

        /// <inheritdoc/>
        public void Add(T item) => Include(item);

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other) => IncludeRange(other);

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            var otherSet = AsSet(other);
            foreach (var item in dict.Keys.ToList())
            {
                if (!otherSet.Contains(item))
                {
                    dict.Remove(item);
                }
            }
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                dict.Remove(item);
            }
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                dict.TryGetValue(item, out var count);
                dict[item] = (byte)(count + 1);
            }
            dict = dict.Where(i => i.Value == 1).ToDictionary(i => i.Key, i => i.Value);
        }

        static ISet<T> AsSet(IEnumerable<T> items) => items is ISet<T> set ? set : new HashSet<T>(items);

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other) => AsSet(other).IsSupersetOf(this);

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other) => other.All(item => dict.ContainsKey(item));

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other) => IsSupersetOf(other) && !Equals(AsSet(other));

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other) => IsSubsetOf(other) && !Equals(AsSet(other));

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other) => other.Any(item => dict.ContainsKey(item));

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other)
        {
            var count = 0;
            foreach (var item in other)
            {
                if (!dict.ContainsKey(item))
                {
                    return false;
                }
                count++;
            }
            return Count == count;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return dict.Remove(item);
        }

        bool ISet<T>.Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var result = !dict.ContainsKey(item);
            Include(item);
            return result;
        }

        /// <inheritdoc/>
        public void Clear() => dict.Clear();

        #endregion public Member

        #region ICollection Member

        /// <summary>Copies all items present at the set to the specified array, starting at a specified index.</summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => dict.Keys.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public int Count => dict.Count;

        #endregion ICollection Member
    }
}

#endif
