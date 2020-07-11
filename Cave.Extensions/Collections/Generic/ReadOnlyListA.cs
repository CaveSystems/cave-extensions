﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a readonly collection implementation for A items of <see cref="Set{A, B}"/>.
    /// </summary>
    /// <typeparam name="TValue1"></typeparam>
    /// <typeparam name="TValue2"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    public sealed class ReadOnlyListA<TValue1, TValue2> : IList<TValue1>
    {
        IItemSet<TValue1, TValue2> set;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="items"></param>
        public ReadOnlyListA(IItemSet<TValue1, TValue2> items)
        {
            set = items;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TValue1 item)
        {
            return set.IndexOfA(item);
        }

        /// <summary>
        /// Throws a ReadOnlyException.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TValue1 item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Throws a ReadOnlyException.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Obtains the item at the specified index.
        /// Setter throws a ReadOnlyException.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue1 this[int index]
        {
            get => set[index].A;
            set => throw new ReadOnlyException();
        }

        /// <summary>
        /// Throws a ReadOnlyException.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TValue1 item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Throws a ReadOnlyException.
        /// </summary>
        public void Clear()
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Determines whether an element is part of the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TValue1 item)
        {
            return IndexOf(item) > -1;
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start writing items at.</param>
        public void CopyTo(TValue1[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            for (int i = 0; i < set.Count; i++)
            {
                array[arrayIndex++] = set[i].A;
            }
        }

        /// <summary>
        /// Returns the number of elements present.
        /// </summary>
        public int Count => set.Count;

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Throws a ReadOnlyException.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TValue1 item)
        {
            throw new ReadOnlyException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue1> GetEnumerator()
        {
            return new EnumeratorA(set);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorA(set);
        }

        class EnumeratorA : IEnumerator<TValue1>
        {
            IItemSet<TValue1, TValue2> set;
            int index = -1;

            public EnumeratorA(IItemSet<TValue1, TValue2> items)
            {
                set = items;
            }

            public TValue1 Current => set[index].A;

            public void Dispose()
            {
            }

            object IEnumerator.Current => set[index].A;

            public bool MoveNext()
            {
                return ++index < set.Count;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
