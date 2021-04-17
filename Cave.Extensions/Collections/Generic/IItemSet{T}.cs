using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a set of items.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{T}" />
    [SuppressMessage("Naming", "CA1710")]
    public interface IItemSet<T> : ICollection<T>, ICollection, IEquatable<IItemSet<T>>
    {
        #region Properties

#pragma warning disable 108,114 // reintroduced for harmonization
        /// <summary>Gets the number of items in the set.</summary>
        int Count { get; }
#pragma warning restore 108,114
        /// <summary>Gets a value indicating whether the set is empty or not.</summary>
        bool IsEmpty { get; }

        #endregion

        #region Members

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <exception cref="ArgumentException">Item is already present!.</exception>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        void AddRange(IEnumerable<T> items);

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <exception cref="ArgumentException">Item is already present!.</exception>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        void AddRange(params T[] items);

        /// <summary>Checks whether all specified items are part of the set.</summary>
        /// <param name="items">Items to check.</param>
        /// <exception cref="ArgumentNullException">items.</exception>
        /// <returns>Returns true if all items are present.</returns>
        bool ContainsRange(IEnumerable<T> items);

        /// <summary>Includes an item that is not already present in the set (others are ignored).</summary>
        /// <param name="item">The item to be included.</param>
        /// <returns>Returns true if the item was added, false if it was present already.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        bool Include(T item);

        /// <summary>Includes items that are not already present in the set (others are ignored).</summary>
        /// <param name="items">The items to be included.</param>
        /// <returns>Returns the number of items added.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        int IncludeRange(IEnumerable<T> items);

        /// <summary>Adds a range of items to the set.</summary>
        /// <param name="items">The items to be added to the list.</param>
        /// <returns>Returns the number of items added.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        int IncludeRange(params T[] items);

        /// <summary>Removes an item from the set.</summary>
        /// <param name="item">The item to be removed.</param>
        /// <exception cref="KeyNotFoundException">Key cannot be found.</exception>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        new void Remove(T item);

        /// <summary>Removes items from the set.</summary>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        void RemoveRange(IEnumerable<T> items);

        /// <summary>Tries the remove the specified value.</summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>Returns true if the item was found and removed, false otherwise.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        bool TryRemove(T value);

        /// <summary>Removes items from the set.</summary>
        /// <param name="items">The items to be removed to the list.</param>
        /// <returns>Returns the number of items removed.</returns>
        /// <exception cref="ReadOnlyException">Set is readonly.</exception>
        int TryRemoveRange(IEnumerable<T> items);

        #endregion
    }
}
