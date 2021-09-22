#if NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

using System.Collections.Generic;

namespace System.Linq
{
    public interface IGrouping<TKey, TElement> : IEnumerable<TElement>

    {
        TKey Key { get; }
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif
