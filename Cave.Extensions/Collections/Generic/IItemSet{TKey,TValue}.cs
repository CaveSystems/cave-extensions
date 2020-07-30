using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>
    ///     Gets an interface for 2D set implementations (Each set A and B may only contain each value once. If typeof(A)
    ///     == typeof(B) a value may be present once at each set. Each value in set a is linked to a value in set b via its
    ///     index).
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [SuppressMessage("Naming", "CA1710")]
    public interface IItemSet<TKey, TValue> : IList<ItemPair<TKey, TValue>>
    {
        /// <summary>Gets a list of all A items.</summary>
        IList<TKey> ItemsA { get; }

        /// <summary>Gets a list of all B items.</summary>
        IList<TValue> ItemsB { get; }

        /// <summary>Gets the index of the specified A item.</summary>
        /// <exception cref="NotSupportedException">Thrown if the set does not support indexing.</exception>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a A index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <param name="key">The A key.</param>
        /// <returns>Returns the index of the item.</returns>
        int IndexOfA(TKey key);

        /// <summary>Gets the index of the specified B item.</summary>
        /// <param name="value">The B index.</param>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a B index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <returns>Returns the index of the item.</returns>
        int IndexOfB(TValue value);

        /// <summary>Checks whether a specified A key is present.</summary>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a A index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <param name="key">The A key.</param>
        /// <returns>Returns true if the key is present.</returns>
        bool ContainsA(TKey key);

        /// <summary>Checks whether a specified B key is present.</summary>
        /// <param name="value">The B index.</param>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a B index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <returns>Returns true if the key is present.</returns>
        bool ContainsB(TValue value);

        /// <summary>Removes the item with the specified A key.</summary>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a A index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <param name="key">The A key.</param>
        void RemoveA(TKey key);

        /// <summary>Removes the item with the specified B key.</summary>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a B index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <param name="value">The B value.</param>
        void RemoveB(TValue value);

        /// <summary>Gets the item with the specified A index.</summary>
        /// <param name="key">The A index.</param>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a A index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <returns>Returns the <see cref="ItemPair{A, B}" />.</returns>
        ItemPair<TKey, TValue> GetA(TKey key);

        /// <summary>Gets the item with the specified B index.</summary>
        /// <param name="value">The B index.</param>
        /// <exception cref="NotSupportedException">The exception that is thrown when the set does not support a B index.</exception>
        /// <exception cref="KeyNotFoundException">
        ///     The exception that is thrown when the key specified for accessing an element in
        ///     a collection does not match any key in the collection.
        /// </exception>
        /// <returns>Returns the <see cref="ItemPair{A, B}" />.</returns>
        ItemPair<TKey, TValue> GetB(TValue value);
    }
}
