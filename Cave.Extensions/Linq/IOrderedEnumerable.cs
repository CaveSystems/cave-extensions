#pragma warning disable IDE0079
#pragma warning disable CS1591, IDE0055 // we will not document back ports
#if NET20

using System.Collections.Generic;

namespace System.Linq
{
    public interface IOrderedEnumerable<TElement> : IEnumerable<TElement>
    {
        IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending);
    }
}

#endif
#pragma warning restore CS1591, IDE0055, IDE0079
