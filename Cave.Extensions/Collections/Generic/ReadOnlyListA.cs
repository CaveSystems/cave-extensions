using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Cave.Collections.Generic;

/// <summary>Gets a readonly collection implementation for A items of <see cref="Set{A, B}"/>.</summary>
/// <typeparam name="TValue1">The type of the first value in the set.</typeparam>
/// <typeparam name="TValue2">The type of the second value in the set.</typeparam>
[DebuggerDisplay("Count={Count}")]
public sealed class ReadOnlyListA<TValue1, TValue2> : IList<TValue1>
    where TValue1 : notnull
{
    #region Private Classes

    sealed class EnumeratorA : IEnumerator<TValue1>
    {
        #region Private Fields

        readonly IItemSet<TValue1, TValue2> set;
        int index = -1;

        #endregion Private Fields

        #region Public Constructors

        public EnumeratorA(IItemSet<TValue1, TValue2> items) => set = items;

        #endregion Public Constructors

        #region Public Properties

        public TValue1 Current => set[index].A;

        object IEnumerator.Current => set[index].A;

        #endregion Public Properties

        #region Public Methods

        public void Dispose() { }

        public bool MoveNext() => ++index < set.Count;

        public void Reset() => index = -1;

        #endregion Public Methods
    }

    #endregion Private Classes

    #region Private Fields

    readonly IItemSet<TValue1, TValue2> set;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="ReadOnlyListA{TValue1, TValue2}"/> class.</summary>
    /// <param name="items">Items to be added to the list.</param>
    public ReadOnlyListA(IItemSet<TValue1, TValue2> items) => set = items;

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets the number of elements present.</summary>
    public int Count => set.Count;

    /// <summary>Gets a value indicating whether the list is readonly or not.</summary>
    public bool IsReadOnly => true;

    #endregion Public Properties

    #region Public Indexers

    /// <summary>Gets the item at the specified index. Setter throws a ReadOnlyException.</summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>Returns the element at the specified index.</returns>
    public TValue1 this[int index] { get => set[index].A; set => throw new ReadOnlyException(); }

    #endregion Public Indexers

    #region Public Methods

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="item">The item to add.</param>
    public void Add(TValue1 item) => throw new ReadOnlyException();

    /// <summary>Throws a ReadOnlyException.</summary>
    public void Clear() => throw new ReadOnlyException();

    /// <summary>Determines whether an element is part of the collection.</summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>Returns true if the collection contains the specified element; otherwise, false.</returns>
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

        for (var i = 0; i < set.Count; i++)
        {
            array[arrayIndex++] = set[i].A;
        }
    }

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<TValue1> GetEnumerator() => new EnumeratorA(set);

    /// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.</summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>Returns the zero-based index of the first occurrence of the specified object within the entire collection; otherwise, -1.</returns>
    public int IndexOf(TValue1 item) => set.IndexOfA(item);

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="index">The index at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, TValue1 item) => throw new ReadOnlyException();

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>Always throws a ReadOnlyException.</returns>
    public bool Remove(TValue1 item) => throw new ReadOnlyException();

    /// <summary>Throws a ReadOnlyException.</summary>
    /// <param name="index">The index of the item to remove.</param>
    public void RemoveAt(int index) => throw new ReadOnlyException();

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() => new EnumeratorA(set);

    #endregion Public Methods
}
