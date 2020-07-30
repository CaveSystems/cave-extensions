#if NET20
#pragma warning disable CS1591 // we will not document back ports

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    [SuppressMessage("Naming", "CA1710")]
    public class Lookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, ILookup<TKey, TElement>
    {
        readonly IGrouping<TKey, TElement> defaultGroup;
        readonly Dictionary<TKey, IGrouping<TKey, TElement>> groups;

        internal Lookup(Dictionary<TKey, List<TElement>> lookup, IEnumerable<TElement> defaultKeyElements)
        {
            groups = new Dictionary<TKey, IGrouping<TKey, TElement>>(lookup.Comparer);
            foreach (var item in lookup)
            {
                groups.Add(item.Key, new Grouping<TKey, TElement>(item.Key, item.Value));
            }
            if (defaultKeyElements != null)
            {
                defaultGroup = new Grouping<TKey, TElement>(default, defaultKeyElements);
            }
        }

        public int Count => defaultGroup == null ? groups.Count : groups.Count + 1;

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                if (key == null && defaultGroup != null)
                {
                    return defaultGroup;
                }
                else if (key != null)
                {
                    if (groups.TryGetValue(key, out var group))
                    {
                        return group;
                    }
                }
                return new TElement[0];
            }
        }

        public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            if (defaultGroup != null)
            {
                yield return resultSelector(defaultGroup.Key, defaultGroup);
            }
            foreach (var group in groups.Values)
            {
                yield return resultSelector(group.Key, group);
            }
        }

        public bool Contains(TKey key) => key == null ? defaultGroup != null : groups.ContainsKey(key);

        public bool TryGetGroup(TKey key, out IGrouping<TKey, TElement> group)
        {
            if (key == null)
            {
                group = defaultGroup;
                return group != null;
            }
            else
            {
                return groups.TryGetValue(key, out group);
            }
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            if (defaultGroup != null)
            {
                yield return defaultGroup;
            }
            foreach (var group in groups.Values)
            {
                yield return group;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

#pragma warning restore CS1591
#endif
