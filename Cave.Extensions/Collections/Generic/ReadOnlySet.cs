using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a read only set.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <seealso cref="IItemSet{T}" />
    [SuppressMessage("Naming", "CA1710")]
    public sealed class ReadOnlySet<T> : IItemSet<T>
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
        public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public void CopyTo(Array array, int arrayIndex) => set.CopyTo(array, arrayIndex);

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
