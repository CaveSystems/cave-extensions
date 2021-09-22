#if NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

using System.Collections.Generic;

namespace System.Linq
{
    public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
    {
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif
