using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Cave.Collections.Generic;

/// <summary>Provides a read only Set.</summary>
/// <typeparam name="T">Element type.</typeparam>
/// <seealso cref="IItemSet{T}"/>
public sealed class ReadOnlySet<T> : IItemSet<T>
{
    #region Private Fields

    readonly IItemSet<T> set;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes an empty instance.</summary>
    public ReadOnlySet() => set = new Set<T>();

    /// <summary>Initializes a new instance of the <see cref="ReadOnlySet{T}"/> class.</summary>
    /// <param name="set">The Set.</param>
    public ReadOnlySet(IItemSet<T> set) => this.set = set;

    #endregion Public Constructors

    #region Public Properties

    /// <inheritdoc/>
    public int Count => set.Count;

    /// <inheritdoc/>
    public bool IsEmpty => set.IsEmpty;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    public bool IsSynchronized => set.IsSynchronized;

    /// <inheritdoc/>
    public object SyncRoot => set.SyncRoot;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone() => this;

    /// <inheritdoc/>
    public bool Contains(T item) => set.Contains(item);

    /// <inheritdoc/>
    public bool ContainsRange(IEnumerable<T> items) => set.ContainsRange(items);

    /// <inheritdoc/>
    public void CopyTo(Array array, int arrayIndex) => set.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Equals(IItemSet<T>? other) => other is not null && set.Equals(other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is IItemSet<T> other && Equals(other);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => set != null ? set.GetHashCode() : 0;

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => throw new ReadOnlyException();

    /// <inheritdoc/>
    void IItemSet<T>.AddRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc/>
    void IItemSet<T>.AddRange(params T[] items) => throw new ReadOnlyException();

    /// <inheritdoc/>
    void ICollection<T>.Clear() => throw new ReadOnlyException();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();

    /// <inheritdoc/>
    bool IItemSet<T>.Include(T item) => throw new ReadOnlyException();

    /// <inheritdoc/>
    int IItemSet<T>.IncludeRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc/>
    int IItemSet<T>.IncludeRange(params T[] items) => throw new ReadOnlyException();

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item) => throw new ReadOnlyException();

    /// <inheritdoc/>
    void IItemSet<T>.Remove(T item) => throw new ReadOnlyException();

    /// <inheritdoc/>
    void IItemSet<T>.RemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc/>
    bool IItemSet<T>.TryRemove(T value) => throw new ReadOnlyException();

    /// <inheritdoc/>
    int IItemSet<T>.TryRemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

    #endregion Public Methods
}
