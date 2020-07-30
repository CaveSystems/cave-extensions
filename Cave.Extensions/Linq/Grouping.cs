#if NET20
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        internal readonly IEnumerable<TElement> Group;

        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            Group = group;
            Key = key;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator() => Group.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Group.GetEnumerator();
    }
}

#endif
