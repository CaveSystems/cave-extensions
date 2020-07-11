using System;
using System.Collections.Generic;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a set of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{T}" />
    public interface IItemSet<T> : ICollection<T>
    {
        /// <summary>
        /// Checks whether all specified items are part of the set.
        /// </summary>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <returns>Returns true if all items are present.</returns>
        bool ContainsRange(IEnumerable<T> items);

        /// <summary>
        /// Returns true if the set is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds a range of items to the set.
        /// </summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <exception cref="ArgumentException">Item is already present!.</exception>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        /// Adds a range of items to the set.
        /// </summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <exception cref="ArgumentException">Item is already present!.</exception>
        void AddRange(params T[] items);

        /// <summary>
        /// Includes an item that is not already present in the set (others are ignored).
        /// </summary>
        /// <param name="item">The item to be included.</param>
        /// <returns>Returns true if the item was added, false if it was present already.</returns>
        bool Include(T item);

        /// <summary>
        /// Includes items that are not already present in the set (others are ignored).
        /// </summary>
        /// <param name="items">The items to be included.</param>
        /// <returns>Returns the number of items added.</returns>
        int IncludeRange(IEnumerable<T> items);

        /// <summary>
        /// Adds a range of items to the set.
        /// </summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <returns>Returns the number of items added.</returns>
        int IncludeRange(params T[] items);

        /// <summary>Tries the remove the specified value.</summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>Returns true if the item was found and removed, false otherwise.</returns>
        bool TryRemove(T value);

        /// <summary>
        /// Removes items from the set.
        /// </summary>
        /// <returns>Returns the number of items removed.</returns>
        int TryRemoveRange(IEnumerable<T> items);

        /// <summary>Removes an item from the set.</summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>Returns always true.</returns>
        /// <exception cref="KeyNotFoundException">Key not found!.</exception>
        new void Remove(T item);

        /// <summary>
        /// Removes items from the set.
        /// </summary>
        void RemoveRange(IEnumerable<T> items);
    }
}
