using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cave.Collections.Generic;

namespace Cave.Collections.Concurrent;

/// <summary>Provides a concurrent set based on the <see cref="ConcurrentDictionary{T1,T2}"/> class.</summary>
/// <typeparam name="T">Element type.</typeparam>
[SuppressMessage("Design", "CA1000")]
public class ConcurrentSet<T> : IItemSet<T> where T : notnull
{
    #region Private Fields

    readonly ConcurrentDictionary<T, byte> list = new();

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="ConcurrentSet{T}"/> class.</summary>
    public ConcurrentSet() { }

    /// <summary>Initializes a new instance of the <see cref="ConcurrentSet{T}"/> class.</summary>
    /// <param name="items">Items to add to the set.</param>
    public ConcurrentSet(params T[] items) => AddRange(items);

    /// <summary>Initializes a new instance of the <see cref="ConcurrentSet{T}"/> class.</summary>
    /// <param name="items">Items to add to the set.</param>
    public ConcurrentSet(IEnumerable<T> items) => AddRange(items);

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the number of objects present at the set.</summary>
    public int Count => list.Count;

    /// <inheritdoc/>
    public bool IsEmpty => list.IsEmpty;

    /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
    public bool IsReadOnly => false;

    /// <summary>Gets a value indicating whether the set is synchronized or not.</summary>
    public bool IsSynchronized => false;

    /// <summary>Gets the synchronization root.</summary>
    public object SyncRoot => this;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Builds the intersection of two specified <see cref="ConcurrentSet{T}"/> s.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> BitwiseAnd(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
    {
        if (set1 is null)
        {
            throw new ArgumentNullException(nameof(set1));
        }

        if (set2 is null)
        {
            throw new ArgumentNullException(nameof(set2));
        }

        if (set1.Count < set2.Count)
        {
            return BitwiseAnd(set2, set1);
        }

        var result = new ConcurrentSet<T>();
        foreach (var itemsItem in set1)
        {
            if (set2.Contains(itemsItem))
            {
                result.Add(itemsItem);
            }
        }

        return result;
    }

    /// <summary>Builds the union of two specified <see cref="Set{T}"/> s.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> BitwiseOr(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
    {
        if (set1 is null)
        {
            throw new ArgumentNullException(nameof(set1));
        }

        if (set2 is null)
        {
            throw new ArgumentNullException(nameof(set2));
        }

        if (set1.Count < set2.Count)
        {
            return BitwiseOr(set2, set1);
        }

        var result = new ConcurrentSet<T>();
        result.AddRange(set2);
        foreach (var item in set1)
        {
            if (set2.Contains(item))
            {
                continue;
            }

            result.Add(item);
        }

        return result;
    }

    /// <summary>Gets a <see cref="ConcurrentSet{T}"/> containing all objects part of the first set after removing all objects present at the second set.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> operator -(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => Subtract(set1, set2);

    /// <summary>Checks two sets for inequality.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns></returns>
    public static bool operator !=(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => !(set1 == set2);

    /// <summary>Gets a <see cref="ConcurrentSet{T}"/> containing only objects part of both of the specified sets.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> operator &(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => BitwiseAnd(set1, set2);

    /// <summary>Builds a new <see cref="ConcurrentSet{T}"/> containing only the items found exclusivly in one of both specified sets.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> operator ^(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => Xor(set1, set2);

    /// <summary>Checks two sets for equality.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns></returns>
    public static bool operator ==(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => set1 is null ? set2 is null : set2 is not null && set1.Equals(set2);

    /// <summary>Subtracts the specified <see cref="ConcurrentSet{T}"/> from this one and returns a new <see cref="ConcurrentSet{T}"/> containing the result.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> Subtract(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
    {
        if (set1 is null)
        {
            throw new ArgumentNullException(nameof(set1));
        }

        if (set2 is null)
        {
            throw new ArgumentNullException(nameof(set2));
        }

        var result = new ConcurrentSet<T>();
        foreach (var setItem in set1)
        {
            if (!set2.Contains(setItem))
            {
                result.Add(setItem);
            }
        }

        return result;
    }

    /// <summary>Builds a new <see cref="Set{T}"/> containing only items found exclusivly in one of both specified sets.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> Xor(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
    {
        if (set1 is null)
        {
            throw new ArgumentNullException(nameof(set1));
        }

        if (set2 is null)
        {
            throw new ArgumentNullException(nameof(set2));
        }

        if (set1.Count < set2.Count)
        {
            return Xor(set2, set1);
        }

        var newSet2 = new LinkedList<T>(set2);
        var result = new ConcurrentSet<T>();
        foreach (var setItem in set1)
        {
            if (!set2.Contains(setItem))
            {
                result.Add(setItem);
            }
            else
            {
                _ = newSet2.Remove(setItem);
            }
        }

        result.AddRange(newSet2);
        return result;
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        if (!list.TryAdd(item, (byte)0))
        {
            throw new ArgumentException("An element with the same key already exists!");
        }
    }

    /// <inheritdoc/>
    public void AddRange(T[] items) => AddRange((IEnumerable<T>)items);

    /// <inheritdoc/>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var obj in items)
        {
            Add(obj);
        }
    }

    /// <summary>Clears the set.</summary>
    public void Clear() => list.Clear();

    /// <inheritdoc/>
    public bool Contains(T item) => list.ContainsKey(item);

    /// <inheritdoc/>
    public bool ContainsRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var allFound = true;
        foreach (var item in items)
        {
            allFound &= Contains(item);
        }

        return allFound;
    }

    /// <summary>Copies all objects present at the set to the specified array, starting at a specified index.</summary>
    /// <param name="array">one-dimensional array to copy to.</param>
    /// <param name="index">the zero-based index in array at which copying begins.</param>
    public void CopyTo(Array array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        foreach (var item in this)
        {
            array.SetValue(item, index++);
        }
    }

    /// <summary>Copies all objects present at the set to the specified array, starting at a specified index.</summary>
    /// <param name="array">one-dimensional array to copy to.</param>
    /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex) => list.Keys.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Equals(IItemSet<T>? other) => (other != null) && (other.Count == Count) && ContainsRange(other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IItemSet<T> other && Equals(other);

    /// <summary>Builds a new <see cref="ConcurrentSet{T}"/> containing only items found exclusivly in one of both specified sets.</summary>
    /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public ConcurrentSet<T> ExclusiveOr(ConcurrentSet<T> items) => Xor(this, items);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => list.Keys.GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => list.GetHashCode();

    /// <inheritdoc/>
    public bool Include(T item) => list.TryAdd(item, (byte)0);

    /// <inheritdoc/>
    public int IncludeRange(T[] items) => IncludeRange((IEnumerable<T>)items);

    /// <inheritdoc/>
    public int IncludeRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var count = 0;
        foreach (var obj in items)
        {
            if (Include(obj))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Builds the intersection of the specified and this <see cref="ConcurrentSet{T}"/> and returns a new set with the result.</summary>
    /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public ConcurrentSet<T> Intersect(ConcurrentSet<T> items) => BitwiseAnd(this, items);

    /// <inheritdoc cref="IItemSet{T}"/>
    public void Remove(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (!list.TryRemove(item, out _))
        {
            throw new KeyNotFoundException();
        }
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        foreach (var obj in items)
        {
            Remove(obj);
        }
    }

    /// <summary>Subtracts a specified <see cref="ConcurrentSet{T}"/> from this one and returns a new set with the result.</summary>
    /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public ConcurrentSet<T> Subtract(ConcurrentSet<T> items) => Subtract(this, items);

    /// <summary>Copies the items stored in the set to a new array.</summary>
    /// <returns>A new array containing a snapshot of all items.</returns>
    public T[] ToArray()
    {
        lock (this)
        {
            var result = new T[list.Count];
            list.Keys.CopyTo(result, 0);
            return result;
        }
    }

    /// <inheritdoc/>
    public bool TryRemove(T value) => list.TryRemove(value, out _);

    /// <inheritdoc/>
    public int TryRemoveRange(IEnumerable<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var count = 0;
        foreach (var obj in items)
        {
            if (TryRemove(obj))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Builds the union of the specified and this <see cref="ConcurrentSet{T}"/> and returns a new set with the result.</summary>
    /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public ConcurrentSet<T> Union(ConcurrentSet<T> items) => BitwiseOr(this, items);

    /// <summary>Gets a <see cref="ConcurrentSet{T}"/> containing all objects part of one of the specified sets.</summary>
    /// <param name="set1">The first set used to calculate the result.</param>
    /// <param name="set2">The second set used to calculate the result.</param>
    /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
    public static ConcurrentSet<T> operator |(ConcurrentSet<T> set1, ConcurrentSet<T> set2) => BitwiseOr(set1, set2);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => list.Keys.GetEnumerator();

    /// <inheritdoc cref="ICollection{T}"/>
    bool ICollection<T>.Remove(T item)
    {
        Remove(item);
        return true;
    }

    #endregion Public Methods
}
