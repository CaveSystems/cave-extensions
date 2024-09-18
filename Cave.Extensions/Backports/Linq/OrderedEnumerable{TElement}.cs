#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    sealed class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
    {
        #region Fields

        readonly IEnumerable<TElement> elements;

        #endregion Fields

        #region Constructors

        public OrderedEnumerable(IEnumerable<TElement> elements) => this.elements = elements;

        #endregion Constructors

        #region IOrderedEnumerable<TElement> Members

        IEnumerator IEnumerable.GetEnumerator() => elements.GetEnumerator();

        public IEnumerator<TElement> GetEnumerator() => elements.GetEnumerator();

        public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
        {
            var dict = new SortedDictionary<TKey, List<TElement>>(comparer);
            var nullList = new List<TElement>();
            foreach (var element in elements)
            {
                var key = keySelector(element);
                List<TElement> list;
                if (key is null)
                {
                    list = nullList;
                }
                else if (!dict.TryGetValue(key, out list))
                {
                    dict[key] = list = [];
                }
                list.Add(element);
            }
            var sorted = dict.Values.ToList();
            if (!descending)
            {
                return new OrderedEnumerable<TElement>(nullList.Concat(sorted.SelectMany(s => s)));
            }
            sorted.Reverse();
            return new OrderedEnumerable<TElement>(sorted.SelectMany(s => s).Concat(nullList));
        }

        #endregion IOrderedEnumerable<TElement> Members
    }
}

#endif
