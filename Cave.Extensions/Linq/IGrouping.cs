#pragma warning disable IDE0079
#pragma warning disable CS1591, IDE0055 // we will not document back ports
#if NET20

using System.Collections.Generic;

namespace System.Linq
{
    public interface IGrouping<TKey, TElement> : IEnumerable<TElement>

    {
        TKey Key { get; }
    }
}

#endif
#pragma warning restore CS1591, IDE0055, IDE0079
