using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a read only set.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <seealso cref="IItemSet{T}" />
    public class ReadOnlySet<T> : IItemSet<T>
    {
        readonly IItemSet<T> set;

        /// <summary>Initializes a new instance of the <see cref="ReadOnlySet{T}" /> class.</summary>
        /// <param name="set">The set.</param>
        public ReadOnlySet(IItemSet<T> set) => this.set = set;

        /// <inheritdoc />
        public int Count => set.Count;

        /// <inheritdoc />
        public bool IsReadOnly => true;

        /// <inheritdoc />
        public bool IsEmpty => set.IsEmpty;

        /// <inheritdoc />
        public bool IsSynchronized => set.IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => set.SyncRoot;

        /// <inheritdoc />
        public bool Contains(T item) => set.Contains(item);

        /// <inheritdoc />
        public bool ContainsRange(IEnumerable<T> items) => set.ContainsRange(items);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) { set.CopyTo(array, arrayIndex); }

        /// <inheritdoc />
        public void CopyTo(Array array, int arrayIndex) { set.CopyTo(array, arrayIndex); }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();

        /// <inheritdoc />
        public bool Equals(IItemSet<T> other) => set.Equals(other);

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IItemSet<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => set != null ? set.GetHashCode() : 0;

        #region Functions with ReadOnlyExceptions

        /// <summary>Adds an element to the current set and returns a value to indicate if the element was successfully added.</summary>
        /// <param name="item">The element to add to the set.</param>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public void Add(T item) { throw new ReadOnlyException(); }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public void Clear() { throw new ReadOnlyException(); }

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public void AddRange(IEnumerable<T> items) { throw new ReadOnlyException(); }

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public void AddRange(params T[] items) { throw new ReadOnlyException(); }

        /// <summary>Includes an item that is not already present in the set (others are ignored).</summary>
        /// <param name="item">The item to be included.</param>
        /// <returns>Returns true if the item was added, false if it was present already.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public bool Include(T item) => throw new ReadOnlyException();

        /// <summary>Includes items that are not already present in the set (others are ignored).</summary>
        /// <param name="items">The items to be included.</param>
        /// <returns>Returns the number of items added.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public int IncludeRange(IEnumerable<T> items) => throw new ReadOnlyException();

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public int IncludeRange(params T[] items) => throw new ReadOnlyException();

        /// <summary>Tries the remove the specified value.</summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>Returns true if the item was found and removed, false otherwise.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public bool TryRemove(T value) => throw new ReadOnlyException();

        /// <summary>Removes items from the set.</summary>
        /// <param name="items">Items to remove.</param>
        /// <returns>Returns the number of items removed.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public int TryRemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

        /// <summary>Removes items from the set.</summary>
        /// <param name="items">Items to remove.</param>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public void RemoveRange(IEnumerable<T> items) { throw new ReadOnlyException(); }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
        ///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        [Obsolete("Not supported")]
        public bool Remove(T item) => throw new ReadOnlyException();

        #endregion
    }
}
