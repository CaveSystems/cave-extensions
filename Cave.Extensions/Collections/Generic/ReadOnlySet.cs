using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a read only Set.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <seealso cref="IItemSet{T}" />
    [SuppressMessage("Naming", "CA1710")]
    public sealed class ReadOnlySet<T> : IItemSet<T>
    {
        readonly IItemSet<T> Set;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ReadOnlySet{T}" /> class.</summary>
        /// <param name="set">The Set.</param>
        public ReadOnlySet(IItemSet<T> set) => this.Set = set;

        #endregion

        #region IItemSet<T> Members

        /// <inheritdoc />
        public void CopyTo(Array array, int arrayIndex) => Set.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool IsSynchronized => Set.IsSynchronized;

        /// <inheritdoc />
        public object SyncRoot => Set.SyncRoot;

        /// <inheritdoc />
        public bool Contains(T item) => Set.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => Set.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool IsReadOnly => true;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Set.GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => Set.GetEnumerator();

        /// <inheritdoc />
        public bool Equals(IItemSet<T> other) => Set.Equals(other);

        /// <inheritdoc />
        public bool ContainsRange(IEnumerable<T> items) => Set.ContainsRange(items);

        /// <inheritdoc />
        public int Count => Set.Count;

        /// <inheritdoc />
        public bool IsEmpty => Set.IsEmpty;

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is IItemSet<T> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Set != null ? Set.GetHashCode() : 0;

        #endregion

        #region Members

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone() => this;

        #endregion

        #region Functions with ReadOnlyExceptions

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void ICollection<T>.Add(T item) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void ICollection<T>.Clear() => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void IItemSet<T>.AddRange(IEnumerable<T> items) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void IItemSet<T>.AddRange(params T[] items) => throw new ReadOnlyException();

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
        [Obsolete("Not supported")]
        bool IItemSet<T>.TryRemove(T value) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        int IItemSet<T>.TryRemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void IItemSet<T>.RemoveRange(IEnumerable<T> items) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        bool ICollection<T>.Remove(T item) => throw new ReadOnlyException();

        /// <inheritdoc />
        [Obsolete("Not supported")]
        void IItemSet<T>.Remove(T item) => throw new ReadOnlyException();

        #endregion
    }
}
