#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20

using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

sealed class Grouping<TKey, TElement>(TKey key, IEnumerable<TElement> group) : IGrouping<TKey, TElement>
{
    #region Fields

    internal IEnumerable<TElement> Group => group;

    #endregion Fields

    #region IGrouping<TKey,TElement> Members

    IEnumerator IEnumerable.GetEnumerator() => Group.GetEnumerator();

    public IEnumerator<TElement> GetEnumerator() => Group.GetEnumerator();

    public TKey Key => key;

    #endregion IGrouping<TKey,TElement> Members
}

#endif
