#if !NET35 && !NET20
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cave.Collections.Generic;
#endif

namespace Cave.Collections.Concurrent
{
#if NET35 || NET20
    // This class is not available in NET20
#else
    /// <summary>
    /// Provides a concurrent set based on the <see cref="ConcurrentDictionary{T1, T2}"/> class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentSet<T> : ICollection<T>, ICollection, IEquatable<ConcurrentSet<T>>
    {
    #region operators

        /// <summary>
        /// Obtains a <see cref="ConcurrentSet{T}"/> containing all objects part of one of the specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> operator |(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            return BitwiseOr(set1, set2);
        }

        /// <summary>
        /// Obtains a <see cref="ConcurrentSet{T}"/> containing only objects part of both of the specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> operator &(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            return BitwiseAnd(set1, set2);
        }

        /// <summary>
        /// Obtains a <see cref="ConcurrentSet{T}"/> containing all objects part of the first set after removing all objects present at the second set.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> operator -(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            return Subtract(set1, set2);
        }

        /// <summary>
        /// Builds a new <see cref="ConcurrentSet{T}"/> containing only the items found exclusivly in one of both specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> operator ^(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            return Xor(set1, set2);
        }

        /// <summary>
        /// Checks two sets for equality.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator ==(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            if (set1 is null)
            {
                return set2 is null;
            }

            if (set2 is null)
            {
                return false;
            }

            return set1.Equals(set2);
        }

        /// <summary>
        /// Checks two sets for inequality.
        /// </summary>
        /// <param name="set1"></param>
        /// <param name="set2"></param>
        /// <returns></returns>
        public static bool operator !=(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            return !(set1 == set2);
        }
    #endregion

    #region static Member

        /// <summary>
        /// Builds the union of two specified <see cref="Set{T}"/>s.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> BitwiseOr(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException("set1");
            }

            if (set2 == null)
            {
                throw new ArgumentNullException("set2");
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseOr(set2, set1);
            }

            ConcurrentSet<T> result = new ConcurrentSet<T>();
            result.AddRange(set2);
            foreach (T item in set1)
            {
                if (set2.Contains(item))
                {
                    continue;
                }

                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// Builds the intersection of two specified <see cref="ConcurrentSet{T}"/>s.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> BitwiseAnd(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException("set1");
            }

            if (set2 == null)
            {
                throw new ArgumentNullException("set2");
            }

            if (set1.Count < set2.Count)
            {
                return BitwiseAnd(set2, set1);
            }

            ConcurrentSet<T> result = new ConcurrentSet<T>();
            foreach (T itemsItem in set1)
            {
                if (set2.Contains(itemsItem))
                {
                    result.Add(itemsItem);
                }
            }
            return result;
        }

        /// <summary>
        /// Subtracts the specified <see cref="ConcurrentSet{T}"/> from this one and returns a new <see cref="ConcurrentSet{T}"/> containing the result.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> Subtract(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException("set1");
            }

            if (set2 == null)
            {
                throw new ArgumentNullException("set2");
            }

            ConcurrentSet<T> result = new ConcurrentSet<T>();
            foreach (T setItem in set1)
            {
                if (!set2.Contains(setItem))
                {
                    result.Add(setItem);
                }
            }
            return result;
        }

        /// <summary>
        /// Builds a new <see cref="Set{T}"/> containing only items found exclusivly in one of both specified sets.
        /// </summary>
        /// <param name="set1">The first set used to calculate the result.</param>
        /// <param name="set2">The second set used to calculate the result.</param>
        /// <returns>Returns a new <see cref="Set{T}"/> containing the result.</returns>
        public static ConcurrentSet<T> Xor(ConcurrentSet<T> set1, ConcurrentSet<T> set2)
        {
            if (set1 == null)
            {
                throw new ArgumentNullException("set1");
            }

            if (set2 == null)
            {
                throw new ArgumentNullException("set2");
            }

            if (set1.Count < set2.Count)
            {
                return Xor(set2, set1);
            }

            LinkedList<T> newSet2 = new LinkedList<T>(set2);
            ConcurrentSet<T> result = new ConcurrentSet<T>();
            foreach (T setItem in set1)
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

    #region private Member

        ConcurrentDictionary<T, byte> list = new ConcurrentDictionary<T, byte>();

    #endregion

    #region constructors

        /// <summary>
        /// Creates a new empty set.
        /// </summary>
        public ConcurrentSet()
        {
        }

        /// <summary>
        /// Creates a new set with the specified items.
        /// </summary>
        public ConcurrentSet(params T[] items)
        {
            AddRange(items);
        }

        /// <summary>
        /// Creates a new set with the specified items.
        /// </summary>
        public ConcurrentSet(IEnumerable<T> items)
        {
            AddRange(items);
        }

    #endregion

    #region public Member

        /// <summary>
        /// Builds the union of the specified and this <see cref="ConcurrentSet{T}"/> and returns a new set with the result.
        /// </summary>
        /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public ConcurrentSet<T> Union(ConcurrentSet<T> items)
        {
            return BitwiseOr(this, items);
        }

        /// <summary>
        /// Builds the intersection of the specified and this <see cref="ConcurrentSet{T}"/> and returns a new set with the result.
        /// </summary>
        /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public ConcurrentSet<T> Intersect(ConcurrentSet<T> items)
        {
            return BitwiseAnd(this, items);
        }

        /// <summary>
        /// Subtracts a specified <see cref="ConcurrentSet{T}"/> from this one and returns a new set with the result.
        /// </summary>
        /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public ConcurrentSet<T> Subtract(ConcurrentSet<T> items)
        {
            return Subtract(this, items);
        }

        /// <summary>
        /// Builds a new <see cref="ConcurrentSet{T}"/> containing only items found exclusivly in one of both specified sets.
        /// </summary>
        /// <param name="items">Provides the other <see cref="ConcurrentSet{T}"/> used.</param>
        /// <returns>Returns a new <see cref="ConcurrentSet{T}"/> containing the result.</returns>
        public ConcurrentSet<T> ExclusiveOr(ConcurrentSet<T> items)
        {
            return Xor(this, items);
        }

        /// <summary>
        /// Checks whether a specified object is part of the set.
        /// </summary>
        public bool Contains(T item)
        {
            return list.ContainsKey(item);
        }

        /// <summary>
        /// Checks whether all specified objects are part of the set.
        /// </summary>
        public bool ContainsRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            bool allFound = true;
            foreach (T item in items)
            {
                allFound &= Contains(item);
            }
            return allFound;
        }

        /// <summary>
        /// Gets a value indicating whether the set was empty or not.
        /// </summary>
        public bool IsEmpty => list.Count == 0;

        /// <summary>
        /// Adds a specified object to the set.
        /// </summary>
        /// <param name="item">The object to be added to the set.</param>
        public void Add(T item)
        {
            if (!list.TryAdd(item, 0))
            {
                throw new ArgumentException("An element with the same key already exists!");
            }
        }

        /// <summary>
        /// Adds a range of objects to the set.
        /// </summary>
        /// <param name="items">The objects to be added to the list.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (T obj in items)
            {
                Add(obj);
            }
        }

        /// <summary>Includes an object that is not already present in the set (others are ignored).</summary>
        /// <param name="obj">The object to be included.</param>
        /// <returns>Returns true if the object was new, false otherwise.</returns>
        public bool Include(T obj)
        {
            return list.TryAdd(obj, 0);
        }

        /// <summary>
        /// Includes objects that are not already present in the set (others are ignored).
        /// </summary>
        /// <param name="items">The objects to be included.</param>
        public void IncludeRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (T obj in items)
            {
                Include(obj);
            }
        }

        /// <summary>
        /// Removes an object from the set.
        /// </summary>
        /// <param name="item">The object to be removed.</param>
        public bool Remove(T item)
        {
            if (!list.TryRemove(item, out byte b))
            {
                throw new KeyNotFoundException();
            }
            return true;
        }

        /// <summary>
        /// Removes objects from the set.
        /// </summary>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (T obj in items)
            {
                Remove(obj);
            }
        }

        /// <summary>
        /// Clears the set.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
    #endregion

    #region ICollection Member

        /// <summary>
        /// Copies all objects present at the set to the specified array, starting at a specified index.
        /// </summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="arrayIndex">the zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.Keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Obtains the number of objects present at the set.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Copies all objects present at the set to the specified array, starting at a specified index.
        /// </summary>
        /// <param name="array">one-dimensional array to copy to.</param>
        /// <param name="index">the zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            foreach (int item in this)
            {
                array.SetValue(item, index++);
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Returns this.
        /// </summary>
        public object SyncRoot => this;
    #endregion

    #region IEnumerable Member

        /// <summary>
        /// Obtains an <see cref="IEnumerator"/> for this set.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return list.Keys.GetEnumerator();
        }

    #endregion

    #region ICollection<T> Member

        /// <summary>
        /// Gets a value indicating whether the set is readonly or not.
        /// </summary>
        public bool IsReadOnly => false;

    #endregion

    #region IEnumerable<T> Member

        /// <summary>
        /// Obtains an <see cref="IEnumerator"/> for this set.
        /// </summary>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.Keys.GetEnumerator();
        }

    #endregion

        /// <summary>
        /// Obtains an array of all elements present.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[list.Count];
            list.Keys.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Checks another Set{T} instance for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ConcurrentSet<T> other = obj as ConcurrentSet<T>;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        /// <summary>
        /// Checks another Set{T} instance for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ConcurrentSet<T> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Count != Count)
            {
                return false;
            }

            return ContainsRange(other);
        }

        /// <summary>
        /// Obtains the hash code of the base list.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return list.GetHashCode();
        }
    }
#endif
}
