﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Cave.Collections.Generic;

/// <summary>Gets a readonly collection implementation for A items of <see cref="Set{A, B}" />.</summary>
/// <typeparam name="TValue1"></typeparam>
/// <typeparam name="TValue2"></typeparam>
[DebuggerDisplay("Count={Count}")]
public sealed class ReadOnlyListB<TValue1, TValue2> : IList<TValue2>
{
    #region Nested type: EnumeratorB

    sealed class EnumeratorB : IEnumerator<TValue2>
    {
        #region Fields

        readonly IItemSet<TValue1, TValue2> set;
        int index = -1;

        #endregion

        #region Constructors

        public EnumeratorB(IItemSet<TValue1, TValue2> items) => set = items;

        #endregion

        #region IEnumerator<TValue2> Members

        public void Dispose() { }

        object IEnumerator.Current => set[index].B;

        public bool MoveNext() => ++index < set.Count;

        public void Reset() => index = -1;

        public TValue2 Current => set[index].B;

        #endregion
    }

    #endregion

    #region Fields

    readonly IItemSet<TValue1, TValue2> set;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ReadOnlyListB{TValue1, TValue2}" /> class.</summary>
    /// <param name="items"></param>
    public ReadOnlyListB(IItemSet<TValue1, TValue2> items) => set = items;

    #endregion

    #region IList<TValue2> Members

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="item"></param>
    public void Add(TValue2 item) => throw new ReadOnlyException();

    /// <summary>Throws a ReadOnlyException.</summary>
    public void Clear() => throw new ReadOnlyException();

    /// <summary>Determines whether an element is part of the collection.</summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(TValue2 item) => set.IndexOfB(item) > -1;

    /// <summary>Copies the entire collection to a compatible one-dimensional array, starting at the beginning of the target array.</summary>
    /// <param name="array">The array to copy to.</param>
    /// <param name="arrayIndex">The index to start writing items at.</param>
    public void CopyTo(TValue2[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        for (var i = 0; i < set.Count; i++)
        {
            array[arrayIndex++] = set[i].B;
        }
    }

    /// <summary>Gets the number of elements present.</summary>
    public int Count => set.Count;

    /// <summary>Gets a value indicating whether the list is readonly or not.</summary>
    public bool IsReadOnly => true;

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(TValue2 item) => throw new ReadOnlyException();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => new EnumeratorB(set);

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns></returns>
    public IEnumerator<TValue2> GetEnumerator() => new EnumeratorB(set);

    /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.</summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(TValue2 item) => set.IndexOfB(item);

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void Insert(int index, TValue2 item) => throw new ReadOnlyException();

    /// <summary>Gets the item at the specified index. Setter throws a ReadOnlyException.</summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue2 this[int index] { get => set[index].B; set => throw new ReadOnlyException(); }

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => throw new ReadOnlyException();

    #endregion
}
