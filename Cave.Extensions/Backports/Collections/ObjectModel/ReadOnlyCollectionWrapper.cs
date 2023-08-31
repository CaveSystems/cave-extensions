#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CA1710 // Will not rename compatibility class

#if NET20 || NET35 || NET40

using System.Collections.Generic;
using System.Data;

namespace System.Collections.ObjectModel;

public class ReadOnlyCollectionWrapper<TValue> : ICollection<TValue>
{
    #region Fields

    readonly ICollection<TValue> col;

    #endregion Fields

    #region Constructors

    public ReadOnlyCollectionWrapper(ICollection<TValue> collection) => col = collection;

    #endregion Constructors

    #region ICollection<TValue> Members

    public void Add(TValue item) => throw new ReadOnlyException();

    public void Clear() => throw new ReadOnlyException();

    public bool Contains(TValue item) => col.Contains(item);

    public void CopyTo(TValue[] array, int arrayIndex) => col.CopyTo(array, arrayIndex);

    public int Count => col.Count;

    public bool IsReadOnly => true;

    public bool Remove(TValue item) => throw new ReadOnlyException();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)col).GetEnumerator();

    public IEnumerator<TValue> GetEnumerator() => col.GetEnumerator();

    #endregion ICollection<TValue> Members
}

#endif
