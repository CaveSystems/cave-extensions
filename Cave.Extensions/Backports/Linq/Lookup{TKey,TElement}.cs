#if NET20
#pragma warning disable CS1591
#pragma warning disable IDE0130
#nullable disable

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public class Lookup<TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>, ILookup<TKey, TElement>
    {
        #region Fields

        readonly IGrouping<TKey, TElement> defaultGroup;
        readonly Dictionary<TKey, IGrouping<TKey, TElement>> groups;

        #endregion Fields

        #region Constructors

        internal Lookup(Dictionary<TKey, List<TElement>> lookup, IEnumerable<TElement> defaultKeyElements)
        {
            groups = new(lookup.Comparer);
            foreach (var item in lookup)
            {
                groups.Add(item.Key, new Grouping<TKey, TElement>(item.Key, item.Value));
            }
            if (defaultKeyElements != null)
            {
                defaultGroup = new Grouping<TKey, TElement>(default, defaultKeyElements);
            }
        }

        #endregion Constructors

        #region IEnumerable<IGrouping<TKey,TElement>> Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        #endregion IEnumerable<IGrouping<TKey,TElement>> Members

        #region ILookup<TKey,TElement> Members

        public bool Contains(TKey key) => key == null ? defaultGroup != null : groups.ContainsKey(key);

        public int Count => defaultGroup == null ? groups.Count : groups.Count + 1;

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                if ((key == null) && (defaultGroup != null))
                {
                    return defaultGroup;
                }
                if (key != null)
                {
                    if (groups.TryGetValue(key, out var group))
                    {
                        return group;
                    }
                }
                return new TElement[0];
            }
        }

        #endregion ILookup<TKey,TElement> Members

        #region Members

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

        public bool TryGetGroup(TKey key, out IGrouping<TKey, TElement> group)
        {
            if (key == null)
            {
                group = defaultGroup;
                return group != null;
            }
            return groups.TryGetValue(key, out group);
        }

        #endregion Members
    }
}

#endif
