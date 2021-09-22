#if NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

using System.Collections.Generic;

namespace System.Linq
{
    public interface ILookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
    {
        int Count { get; }
        IEnumerable<TElement> this[TKey key] { get; }
        bool Contains(TKey key);
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif
