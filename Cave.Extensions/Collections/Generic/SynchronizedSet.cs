using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cave.Collections.Generic
{
    /// <summary>Provides a synchronized set based on the <see cref="SynchronizedDictionary{T1, T2}" /> class.</summary>
    /// <typeparam name="T">Element type.</typeparam>
    [SuppressMessage("Design", "CA1000")]
    public class SynchronizedSet<T> : IItemSet<T>
    {
        #region private Member

        readonly SynchronizedDictionary<T, byte> dict = new();

        #endregion

        #region IItemSet<T> Members

        #region ICollection<T> Member

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable Member

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => dict.Keys.GetEnumerator();

        #endregion

        #region IEnumerable<T> Member

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => dict.Keys.GetEnumerator();

        #endregion

        /// <inheritdoc />
        public bool Equals(IItemSet<T> other) => (other != null) && (other.Count == Count) && ContainsRange(other);

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is IItemSet<T> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => dict.GetHashCode();

        #endregion

        #region Members

        /// <summary>Copies the items stored in the set to a new array.</summary>
        /// <returns>A new array containing a snapshot of all items.</returns>
        public T[] ToArray()
        {
            lock (SyncRoot)
            {
                var result = new T[dict.Count];
                dict.Keys.CopyTo(result, 0);
                return result;
            }
        }

        #endregion

        #region operators

        /// <summary>Gets a <see cref="SynchronizedSet{T}" /> containing all objects part of one of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> operator |(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => BitwiseOr(set1, set2);

        /// <summary>Gets a <see cref="SynchronizedSet{T}" /> containing only objects part of both of the specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> operator &(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => BitwiseAnd(set1, set2);

        /// <summary>
        /// Gets a <see cref="SynchronizedSet{T}" /> containing all objects part of the first set after removing all objects present at the
        /// second set.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> operator -(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => Subtract(set1, set2);

        /// <summary>Builds a new <see cref="SynchronizedSet{T}" /> containing only the items found exclusivly in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> operator ^(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => Xor(set1, set2);

        /// <summary>Checks two sets for equality.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns></returns>
        public static bool operator ==(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => set1 is null ? set2 is null : set2 is not null && set1.Equals(set2);

        /// <summary>Checks two sets for inequality.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns></returns>
        public static bool operator !=(SynchronizedSet<T> set1, SynchronizedSet<T> set2) => !(set1 == set2);

        #endregion

        #region static Member

        /// <summary>Builds the union of two specified <see cref="Set{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> BitwiseOr(SynchronizedSet<T> set1, SynchronizedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseOr(set2, set1);
            }

            var result = new SynchronizedSet<T>();
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

        /// <summary>Builds the intersection of two specified <see cref="SynchronizedSet{T}" />s.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> BitwiseAnd(SynchronizedSet<T> set1, SynchronizedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseAnd(set2, set1);
            }

            var result = new SynchronizedSet<T>();
            foreach (var itemsItem in set1)
            {
                if (set2.Contains(itemsItem))
                {
                    result.Add(itemsItem);
                }
            }

            return result;
        }

        /// <summary>
        /// Subtracts the specified <see cref="SynchronizedSet{T}" /> from this one and returns a new <see cref="SynchronizedSet{T}" /> containing
        /// the result.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> Subtract(SynchronizedSet<T> set1, SynchronizedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            var result = new SynchronizedSet<T>();
            foreach (var setItem in set1)
            {
                if (!set2.Contains(setItem))
                {
                    result.Add(setItem);
                }
            }

            return result;
        }

        /// <summary>Builds a new <see cref="Set{T}" /> containing only items found exclusivly in one of both specified sets.</summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}" /> containing the result.</returns>
        public static SynchronizedSet<T> Xor(SynchronizedSet<T> set1, SynchronizedSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException(nameof(set1));
            }

            if (set2 == null)
            {
                throw new ArgumentNullException(nameof(set2));
            }

            if (set1.Count < set2.Count)
            {
                return Xor(set2, set1);
            }

            var newSet2 = new LinkedList<T>(set2);
            var result = new SynchronizedSet<T>();
            foreach (var setItem in set1)
            {
                if (!set2.Contains(setItem))
                {
                    result.Add(setItem);
                }
                else
                {
                    newSet2.Remove(setItem);
                }
            }

            result.AddRange(newSet2);
            return result;
        }

        #endregion

        #region constructors

        /// <summary>Initializes a new instance of the <see cref="SynchronizedSet{T}" /> class.</summary>
        public SynchronizedSet() { }

        /// <summary>Initializes a new instance of the <see cref="SynchronizedSet{T}" /> class.</summary>
        /// <param name="items">Items to add to the set.</param>
        public SynchronizedSet(params T[] items) => AddRange(items);

        /// <summary>Initializes a new instance of the <see cref="SynchronizedSet{T}" /> class.</summary>
        /// <param name="items">Items to add to the set.</param>
        public SynchronizedSet(IEnumerable<T> items) => AddRange(items);

        #endregion

        #region public Member

        /// <summary>Builds the union of the specified and this <see cref="SynchronizedSet{T}" /> and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="SynchronizedSet{T}" /> used.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public SynchronizedSet<T> Union(SynchronizedSet<T> items) => BitwiseOr(this, items);

        /// <summary>Builds the intersection of the specified and this <see cref="SynchronizedSet{T}" /> and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="SynchronizedSet{T}" /> used.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public SynchronizedSet<T> Intersect(SynchronizedSet<T> items) => BitwiseAnd(this, items);

        /// <summary>Subtracts a specified <see cref="SynchronizedSet{T}" /> from this one and returns a new set with the result.</summary>
        /// <param name="items">Provides the other <see cref="SynchronizedSet{T}" /> used.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public SynchronizedSet<T> Subtract(SynchronizedSet<T> items) => Subtract(this, items);

        /// <summary>Builds a new <see cref="SynchronizedSet{T}" /> containing only items found exclusivly in one of both specified sets.</summary>
        /// <param name="items">Provides the other <see cref="SynchronizedSet{T}" /> used.</param>
        /// <returns>Returns a new <see cref="SynchronizedSet{T}" /> containing the result.</returns>
        public SynchronizedSet<T> ExclusiveOr(SynchronizedSet<T> items) => Xor(this, items);

        /// <inheritdoc />
        public bool Contains(T item) => dict.ContainsKey(item);

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool IsEmpty => dict.Count == 0;

        /// <inheritdoc />
        public void Add(T item)
        {
            if (!dict.TryAdd(item, 0))
            {
                throw new ArgumentException("An element with the same key already exists!");
            }
        }

        /// <inheritdoc />
        public void AddRange(T[] items) => AddRange((IEnumerable<T>)items);

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool Include(T item) => dict.TryAdd(item, 0);

        /// <inheritdoc />
        public int IncludeRange(T[] items) => IncludeRange((IEnumerable<T>)items);

        /// <inheritdoc />
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

        /// <inheritdoc cref="ICollection{T}" />
        bool ICollection<T>.Remove(T item)
        {
            Remove(item);
            return true;
        }

        /// <inheritdoc cref="IItemSet{T}" />
        public void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!dict.TryRemove(item, out _))
            {
                throw new KeyNotFoundException();
            }
        }

        /// <inheritdoc />
        public bool TryRemove(T value) => dict.TryRemove(value, out _);

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <summary>Clears the set.</summary>
        public void Clear() => dict.Clear();

        #endregion

        #region ICollection Member

        /// <summary>Copies all objects present at the set to the specified array, starting at a specified index.</summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => dict.Keys.CopyTo(array, arrayIndex);

        /// <summary>Gets the number of objects present at the set.</summary>
        public int Count => dict.Count;

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

        /// <summary>Gets a value indicating whether the set is synchronized or not.</summary>
        public bool IsSynchronized => true;

        /// <summary>Gets the synchronization root.</summary>
        public object SyncRoot => dict.SyncRoot;

        #endregion
    }
}
