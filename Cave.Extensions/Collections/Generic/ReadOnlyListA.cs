﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections.Generic
{
    /// <summary>Gets a readonly collection implementation for A items of <see cref="Set{A, B}" />.</summary>
    /// <typeparam name="TValue1"></typeparam>
    /// <typeparam name="TValue2"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    [SuppressMessage("Naming", "CA1710")]
    public sealed class ReadOnlyListA<TValue1, TValue2> : IList<TValue1>
    {
        #region Nested type: EnumeratorA

        class EnumeratorA : IEnumerator<TValue1>
        {
            readonly IItemSet<TValue1, TValue2> Set;
            int index = -1;

            #region Constructors

            public EnumeratorA(IItemSet<TValue1, TValue2> items) => Set = items;

            #endregion

            #region IEnumerator<TValue1> Members

            public void Dispose() { }

            object IEnumerator.Current => Set[index].A;

            public bool MoveNext() => ++index < Set.Count;

            public void Reset() => index = -1;

            public TValue1 Current => Set[index].A;

            #endregion
        }

        #endregion

        readonly IItemSet<TValue1, TValue2> Set;

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ReadOnlyListA{TValue1, TValue2}" /> class.</summary>
        /// <param name="items">Items to be added to the list.</param>
        public ReadOnlyListA(IItemSet<TValue1, TValue2> items) => Set = items;

        #endregion

        #region IList<TValue1> Members

        /// <summary>Throws a ReadOnlyException.</summary>
        /// <param name="item"></param>
        public void Add(TValue1 item) => throw new ReadOnlyException();

        /// <summary>Throws a ReadOnlyException.</summary>
        public void Clear() => throw new ReadOnlyException();

        /// <summary>Determines whether an element is part of the collection.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(TValue1 item) => IndexOf(item) > -1;

        /// <summary>Copies the entire collection to a compatible one-dimensional array, starting at the beginning of the target array.</summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start writing items at.</param>
        public void CopyTo(TValue1[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            for (var i = 0; i < Set.Count; i++)
            {
                array[arrayIndex++] = Set[i].A;
            }
        }

        /// <summary>Gets the number of elements present.</summary>
        public int Count => Set.Count;

        /// <summary>Gets a value indicating whether the list is readonly or not.</summary>
        public bool IsReadOnly => true;

        /// <summary>Throws a ReadOnlyException.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TValue1 item) => throw new ReadOnlyException();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => new EnumeratorA(Set);

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns></returns>
        public IEnumerator<TValue1> GetEnumerator() => new EnumeratorA(Set);

        /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.</summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TValue1 item) => Set.IndexOfA(item);

        /// <summary>Throws a ReadOnlyException.</summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, TValue1 item) => throw new ReadOnlyException();

        /// <summary>Gets the item at the specified index. Setter throws a ReadOnlyException.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue1 this[int index] { get => Set[index].A; set => throw new ReadOnlyException(); }

        /// <summary>Throws a ReadOnlyException.</summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) => throw new ReadOnlyException();

        #endregion
    }
}
