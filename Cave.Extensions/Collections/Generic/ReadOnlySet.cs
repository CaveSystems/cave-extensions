using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Cave.Collections.Generic;

/// <summary>Provides a read only Set.</summary>
/// <typeparam name="T">Element type.</typeparam>
/// <seealso cref="IItemSet{T}" />
public sealed class ReadOnlySet<T> : IItemSet<T>
{
    #region Fields

    readonly IItemSet<T> set;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ReadOnlySet{T}" /> class.</summary>
    /// <param name="set">The Set.</param>
    public ReadOnlySet(IItemSet<T> set) => this.set = set;

    #endregion

    #region IItemSet<T> Members

    /// <inheritdoc />
    public void CopyTo(Array array, int arrayIndex) => set.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool IsSynchronized => set.IsSynchronized;

    /// <inheritdoc />
    public object SyncRoot => set.SyncRoot;

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void ICollection<T>.Add(T item) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void ICollection<T>.Clear() => throw new ReadOnlyException();

    /// <inheritdoc />
    public bool Contains(T item) => set.Contains(item);

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public bool IsReadOnly => true;

    /// <inheritdoc />
    [Obsolete("Not supported")]
    bool ICollection<T>.Remove(T item) => throw new ReadOnlyException();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => set.GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

    /// <inheritdoc />
    public bool Equals(IItemSet<T> other) => set.Equals(other);

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void IItemSet<T>.AddRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void IItemSet<T>.AddRange(params T[] items) => throw new ReadOnlyException();

    /// <inheritdoc />
    public bool ContainsRange(IEnumerable<T> items) => set.ContainsRange(items);

    /// <inheritdoc />
    public int Count => set.Count;

    /// <inheritdoc />
    [Obsolete("Not supported")]
    bool IItemSet<T>.Include(T item) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    int IItemSet<T>.IncludeRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    int IItemSet<T>.IncludeRange(params T[] items) => throw new ReadOnlyException();

    /// <inheritdoc />
    public bool IsEmpty => set.IsEmpty;

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void IItemSet<T>.Remove(T item) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    void IItemSet<T>.RemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    bool IItemSet<T>.TryRemove(T value) => throw new ReadOnlyException();

    /// <inheritdoc />
    [Obsolete("Not supported")]
    int IItemSet<T>.TryRemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

    #endregion

    #region Overrides

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is IItemSet<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => set != null ? set.GetHashCode() : 0;

    #endregion

    #region Members

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone() => this;

    #endregion
}
