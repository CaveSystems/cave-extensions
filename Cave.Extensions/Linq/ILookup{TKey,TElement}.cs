#if NET20
#pragma warning disable CS1591 // we will not document back ports

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    [SuppressMessage("Naming", "CA1710")]
    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        int Count { get; }
        IEnumerable<TElement> this[TKey key] { get; }
        bool Contains(TKey key);
    }
}

#pragma warning restore CS1591
#endif
