#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        #region Fields

        internal readonly IEnumerable<TElement> Group;

        #endregion Fields

        #region Constructors

        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            Group = group;
            Key = key;
        }

        #endregion Constructors

        #region IGrouping<TKey,TElement> Members

        IEnumerator IEnumerable.GetEnumerator() => Group.GetEnumerator();

        public IEnumerator<TElement> GetEnumerator() => Group.GetEnumerator();

        public TKey Key { get; }

        #endregion IGrouping<TKey,TElement> Members
    }
}

#endif
