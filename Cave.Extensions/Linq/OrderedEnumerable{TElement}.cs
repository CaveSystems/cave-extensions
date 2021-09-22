#if NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    sealed class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    {
        readonly IEnumerable<TElement> elements;

        public OrderedEnumerable(IEnumerable<TElement> elements) => this.elements = elements;

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            var dict = new SortedDictionary<TKey, List<TElement>>(comparer);
            foreach (var element in elements)
            {
                var key = keySelector(element);
                if (!dict.TryGetValue(key, out var list))
                {
                    dict[key] = list = new List<TElement>();
                }
                list.Add(element);
            }
            var sorted = dict.Values.ToList();
            if (descending)
            {
                sorted.Reverse();
            }

            return new OrderedEnumerable<TElement>(sorted.SelectMany(s => s));
        }

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();
    }
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif
