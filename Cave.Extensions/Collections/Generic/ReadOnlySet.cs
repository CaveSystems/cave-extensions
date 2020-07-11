using System;
using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a read only set.</summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IItemSet{T}" />
    public class ReadOnlySet<T> : IItemSet<T>
    {
        IItemSet<T> set;

        /// <summary>Initializes a new instance of the <see cref="ReadOnlySet{T}"/> class.</summary>
        /// <param name="set">The set.</param>
        public ReadOnlySet(IItemSet<T> set)
        {
            this.set = set;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count => set.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <remarks>This is always true.</remarks>
        public bool IsReadOnly => true;

        /// <summary>Returns true if the set is empty.</summary>
        public bool IsEmpty => set.IsEmpty;

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        /// <summary>Checks whether all specified items are part of the set.</summary>
        /// <param name="items"></param>
        /// <returns>Returns true if all items are present.</returns>
        public bool ContainsRange(IEnumerable<T> items)
        {
            return set.ContainsRange(items);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            set.CopyTo(array, arrayIndex);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return set.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return set.GetEnumerator();
        }

        #region not supported

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>true if the element is added to the set; false if the element is already in the set.</returns>
        /// <exception cref="NotSupportedException">Thrown on any call to this function.</exception>
        [Obsolete("Not supported")]
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="NotSupportedException">Thrown on any call to this function.</exception>
        [Obsolete("Not supported")]
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>Removes an item from the set.</summary>
        /// <param name="item">The item to be removed.</param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public void Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public void AddRange(IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public void AddRange(params T[] items)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Includes an item that is not already present in the set (others are ignored).
        /// </summary>
        /// <param name="item">The item to be included.</param>
        /// <returns>Returns true if the item was added, false if it was present already.</returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public bool Include(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Includes items that are not already present in the set (others are ignored).
        /// </summary>
        /// <param name="items">The items to be included.</param>
        /// <returns>Returns the number of items added.</returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public int IncludeRange(IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public int IncludeRange(params T[] items)
        {
            throw new NotSupportedException();
        }

        /// <summary>Tries the remove the specified value.</summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>Returns true if the item was found and removed, false otherwise.</returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public bool TryRemove(T value)
        {
            throw new NotSupportedException();
        }

        /// <summary>Removes items from the set.</summary>
        /// <param name="items"></param>
        /// <returns>Returns the number of items removed.</returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public int TryRemoveRange(IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        /// <summary>Removes items from the set.</summary>
        /// <param name="items"></param>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        public void RemoveRange(IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="NotSupportedException"></exception>
        [Obsolete("Not supported")]
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        [Obsolete("Not supported")]
        public object Clone()
        {
            return this;
        }

        #endregion
    }
}
