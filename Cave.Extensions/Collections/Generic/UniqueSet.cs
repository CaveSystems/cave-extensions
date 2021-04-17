using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Gets a typed 2D set. This class uses a Set for each dimension allowing only unique values in each dimension. Additionally to the
    /// fast A (Key1) and B (Key2) lookup it provides indexing like a list.
    /// </summary>
    /// <typeparam name="TKey1">The type of the key1.</typeparam>
    /// <typeparam name="TKey2">The type of the key2.</typeparam>
    /// <seealso cref="Cave.Collections.Generic.IItemSet{TKey1, TKey2}" />
    [DebuggerDisplay("Count={Count}")]
    [SuppressMessage("Naming", "CA1710")]
    public sealed class UniqueSet<TKey1, TKey2> : IItemSet<TKey1, TKey2>
    {
        readonly List<ItemPair<TKey1, TKey2>> List = new();
        readonly Dictionary<TKey1, ItemPair<TKey1, TKey2>> LookupA = new();
        readonly Dictionary<TKey2, ItemPair<TKey1, TKey2>> LookupB = new();

        #region Properties

        /// <summary>Obtains a enumeration of the A elements of the Set.</summary>
        /// <value>The keys a.</value>
        public IEnumerable<TKey1> KeysA => LookupA.Keys;

        /// <summary>Obtains a enumeration of the B elements of the Set.</summary>
        /// <value>The keys b.</value>
        public IEnumerable<TKey2> KeysB => LookupB.Keys;

        #endregion

        #region IItemSet<TKey1,TKey2> Members

        /// <summary>Adds an itempair to the set.</summary>
        /// <param name="item"></param>
        public void Add(ItemPair<TKey1, TKey2> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Add(item.A, item.B);
        }

        /// <summary>Clears the set.</summary>
        public void Clear()
        {
            List.Clear();
            LookupA.Clear();
            LookupB.Clear();
        }

        /// <summary>Checks whether the list contains an itempair or not.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ItemPair<TKey1, TKey2> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return Contains(item.A, item.B);
        }

        /// <summary>Copies the elements of the set to an Array, starting at a particular Array index.</summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ItemPair<TKey1, TKey2>[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);

        /// <summary>Gets the number of elements actually present at the Set.</summary>
        public int Count => List.Count;

        /// <summary>Gets a value indicating whether the set is readonly or not.</summary>
        public bool IsReadOnly => false;

        /// <summary>Removes an itempair from the set.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ItemPair<TKey1, TKey2> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                LookupA.Remove(item.A);
                LookupB.Remove(item.B);
                return List.Remove(item);
            }
            catch
            {
                Clear();
                throw;
            }
        }

        /// <summary>Returns an enumerator that iterates through the set.</summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the set.</summary>
        /// <returns>An IEnumerator object that can be used to iterate through the set.</returns>
        public IEnumerator<ItemPair<TKey1, TKey2>> GetEnumerator() => List.GetEnumerator();

        /// <summary>Determines whether the specified A is part of the set.</summary>
        /// <param name="key1">The a value to check for.</param>
        /// <returns><c>true</c> if present; otherwise, <c>false</c>.</returns>
        public bool ContainsA(TKey1 key1) => LookupA.ContainsKey(key1);

        /// <summary>Determines whether the specified B is part of the set.</summary>
        /// <param name="key2">The b value to check for.</param>
        /// <returns><c>true</c> if present; otherwise, <c>false</c>.</returns>
        public bool ContainsB(TKey2 key2) => LookupB.ContainsKey(key2);

        /// <summary>Gets the A element that is assigned to the specified B element. This method is an O(1) operation;.</summary>
        /// <param name="key1">The A index.</param>
        /// <returns></returns>
        public ItemPair<TKey1, TKey2> GetA(TKey1 key1) => LookupA[key1];

        /// <summary>Gets the A element that is assigned to the specified B element. This method is an O(1) operation;.</summary>
        /// <param name="key2">The B index.</param>
        /// <returns></returns>
        public ItemPair<TKey1, TKey2> GetB(TKey2 key2) => LookupB[key2];

        /// <summary>Gets the index of the specified A object. This is an O(1) operation.</summary>
        /// <param name="key1">'A' object to be found.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOfA(TKey1 key1) => !LookupA.ContainsKey(key1) ? -1 : List.IndexOf(LookupA[key1]);

        /// <summary>Gets the index of the specified B object. This is an O(1) operation.</summary>
        /// <param name="key2">'B' object to be found.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOfB(TKey2 key2) => !LookupB.ContainsKey(key2) ? -1 : List.IndexOf(LookupB[key2]);

        /// <summary>Gets a read only indexed list for the A elements of the Set. This method is an O(1) operation;.</summary>
        public IList<TKey1> ItemsA => new ReadOnlyListA<TKey1, TKey2>(this);

        /// <summary>Gets a read only indexed list for the B elements of the Set. This method is an O(1) operation;.</summary>
        public IList<TKey2> ItemsB => new ReadOnlyListB<TKey1, TKey2>(this);

        /// <summary>Removes the item with the specified A key.</summary>
        /// <param name="key1">The A key.</param>
        /// <exception cref="KeyNotFoundException">
        /// The exception that is thrown when the key specified for accessing an element in a collection does
        /// not match any key in the collection.
        /// </exception>
        public void RemoveA(TKey1 key1) => Remove(GetA(key1));

        /// <summary>Removes the item with the specified B key.</summary>
        /// <param name="key2">The A key.</param>
        /// <exception cref="KeyNotFoundException">
        /// The exception that is thrown when the key specified for accessing an element in a collection does
        /// not match any key in the collection.
        /// </exception>
        public void RemoveB(TKey2 key2) => Remove(GetB(key2));

        /// <summary>Gets the index of the specified ItemPair. This is an O(1) operation.</summary>
        /// <param name="item">The ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(ItemPair<TKey1, TKey2> item) => List.IndexOf(item);

        /// <summary>Inserts a new ItemPair at the specified index. This method needs a full index rebuild and is an O(n) operation, where n is Count.</summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The ItemPair to insert.</param>
        public void Insert(int index, ItemPair<TKey1, TKey2> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            LookupA.Add(item.A, item);
            try
            {
                LookupB.Add(item.B, item);
            }
            catch
            {
                LookupA.Remove(item.A);
                throw;
            }

            List.Insert(index, item);
        }

        /// <summary>
        /// Accesses the ItemPair at the specified index. The getter is a O(1) operation. The setter needs a full index rebuild and is an O(n)
        /// operation, where n is Count.
        /// </summary>
        /// <param name="index">The index of the ItemPair to be accessed.</param>
        /// <returns></returns>
        public ItemPair<TKey1, TKey2> this[int index]
        {
            get => List[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                var old = List[index];
                if (!LookupA.Remove(old.A))
                {
                    throw new KeyNotFoundException();
                }

                if (!LookupB.Remove(old.B))
                {
                    throw new KeyNotFoundException();
                }

                LookupA.Add(value.A, value);
                try
                {
                    LookupB.Add(value.B, value);
                }
                catch
                {
                    Clear();
                    throw;
                }

                List[index] = value;
            }
        }

        /// <summary>Removes the ItemPair at the specified index. This method needs a full index rebuild and is an O(n) operation, where n is Count.</summary>
        /// <param name="index">The index to remove the item.</param>
        public void RemoveAt(int index)
        {
            try
            {
                var node = List[index];
                if (!LookupA.Remove(node.A))
                {
                    throw new KeyNotFoundException();
                }

                if (!LookupB.Remove(node.B))
                {
                    throw new KeyNotFoundException();
                }
            }
            catch
            {
                Clear();
                throw;
            }
        }

        #endregion

        #region Members

        /// <summary>Adds an item pair to the end of the List. This is an O(1) operation.</summary>
        /// <param name="key1">The A object to be added.</param>
        /// <param name="key2">The B object to be added.</param>
        public void Add(TKey1 key1, TKey2 key2)
        {
            var node = new ItemPair<TKey1, TKey2>(key1, key2);
            LookupA.Add(key1, node);
            try
            {
                LookupB.Add(key2, node);
            }
            catch
            {
                LookupA.Remove(key1);
                throw;
            }

            List.Add(node);
        }

        /// <summary>Checks whether the list contains an itempair or not.</summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public bool Contains(TKey1 key1, TKey2 key2) => LookupA.ContainsKey(key1) && Equals(LookupA[key1].B, key2);

        /// <summary>Gets the index of the specified ItemPair. This is an O(1) operation.</summary>
        /// <param name="key1">The A value of the ItemPair to search for.</param>
        /// <param name="key2">The B value of the ItemPair to search for.</param>
        /// <returns>The index of the ItemPair if found in the list; otherwise, -1.</returns>
        public int IndexOf(TKey1 key1, TKey2 key2) => IndexOf(new ItemPair<TKey1, TKey2>(key1, key2));

        /// <summary>Inserts a new ItemPair at the specified index. This method needs a full index rebuild and is an O(n) operation, where n is Count.</summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="key1">The A value of the ItemPair to insert.</param>
        /// <param name="key2">The B value of the ItemPair to insert.</param>
        public void Insert(int index, TKey1 key1, TKey2 key2) => Insert(index, new ItemPair<TKey1, TKey2>(key1, key2));

        /// <summary>Removes an itempair from the set.</summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public bool Remove(TKey1 key1, TKey2 key2) => Remove(new ItemPair<TKey1, TKey2>(key1, key2));

        /// <summary>Reverses the index of the set.</summary>
        public void Reverse()
        {
            List.Reverse();
            RebuildIndex();
        }

        /// <summary>Tries to the get the a key.</summary>
        /// <param name="key1">The a key.</param>
        /// <param name="key2">The b key.</param>
        /// <returns></returns>
        public bool TryGetA(TKey1 key1, out TKey2 key2)
        {
            if (LookupA.TryGetValue(key1, out var item))
            {
                key2 = item.B;
                return true;
            }

            key2 = default;
            return false;
        }

        /// <summary>Tries to the get the key b.</summary>
        /// <param name="key2">The b key.</param>
        /// <param name="key1">The a key.</param>
        /// <returns></returns>
        public bool TryGetB(TKey2 key2, out TKey1 key1)
        {
            if (LookupB.TryGetValue(key2, out var item))
            {
                key1 = item.A;
                return true;
            }

            key1 = default;
            return false;
        }

        /// <summary>Rebuilds the index after an operation that destroys (one of) them. This is an O(n) operation, where n is Count.</summary>
        void RebuildIndex()
        {
            LookupA.Clear();
            LookupB.Clear();
            foreach (var node in List)
            {
                LookupA.Add(node.A, node);
                LookupB.Add(node.B, node);
            }
        }

        #endregion
    }
}
