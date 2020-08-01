#if NET20
#pragma warning disable CS1591 // we will not document back ports

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    [SuppressMessage("Naming", "CA1720", Scope = "Member")]
    public static class BackportedExtensions
    {
        static TResult ThrowSequenceEmpty<TResult>() => throw new InvalidOperationException("The source sequence is empty.");

        #region Aggregate

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var empty = true;
            var accu = seed;
            foreach (var item in source)
            {
                empty = false;
                accu = func(accu, item);
            }

            if (empty)
            {
                throw new ArgumentException("Empty sequence!");
            }

            return resultSelector(accu);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func)
            => Aggregate(source, seed, func, (a) => a);

        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
            => Aggregate(source, default, func, (a) => a);

        #endregion

        #region All

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => !source.Any((i) => !predicate(i));

        #endregion

        #region Any

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var item in source)
            {
                return true;
            }

            return false;
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region AsEnumerable

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source) => source;

        #endregion

        #region Average

        static TResult CalcAverage<TSource, TAggregate, TResult>(this IEnumerable<TSource> source, Func<TAggregate, TSource, TAggregate> totalAddValue,
            Func<TAggregate, long, TResult> totalDivCount)
            where TSource : struct
            where TAggregate : struct
            where TResult : struct
        {
            var total = default(TAggregate);
            long counter = 0;
            foreach (var element in source)
            {
                total = totalAddValue(total, element);
                counter++;
            }

            if (counter == 0)
            {
                ThrowSequenceEmpty<TResult>();
            }

            return totalDivCount(total, counter);
        }

        [SuppressMessage("Design", "CA1062")]
        public static double Average(this IEnumerable<int> source)
            => CalcAverage<int, long, double>(source, (value, total) => value + total, (total, count) => total / (double) count);

        [SuppressMessage("Design", "CA1062")]
        public static double Average(this IEnumerable<long> source)
            => CalcAverage<long, long, double>(source, (value, total) => value + total, (total, count) => total / (double) count);

        [SuppressMessage("Design", "CA1062")]
        public static double Average(this IEnumerable<double> source)
            => CalcAverage<double, double, double>(source, (value, total) => value + total, (total, count) => total / count);

        [SuppressMessage("Design", "CA1062")]
        public static float Average(this IEnumerable<float> source)
            => CalcAverage<float, float, float>(source, (value, total) => value + total, (total, count) => total / count);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Average(this IEnumerable<decimal> source)
            => CalcAverage<decimal, decimal, decimal>(source, (value, total) => value + total, (total, count) => total / count);

        static TResult? CalcAverage<TSource, TAggregate, TResult>(this IEnumerable<TSource?> source, Func<TAggregate, TSource, TAggregate> totalAddValue,
            Func<TAggregate, long, TResult> totalDivCount)
            where TSource : struct
            where TAggregate : struct
            where TResult : struct
        {
            var total = default(TAggregate);
            long counter = 0;
            foreach (var element in source)
            {
                if (!element.HasValue)
                {
                    continue;
                }

                total = totalAddValue(total, element.Value);
                counter++;
            }

            return counter == 0 ? null : new TResult?(totalDivCount(total, counter));
        }

        [SuppressMessage("Design", "CA1062")]
        public static double? Average(this IEnumerable<int?> source)
            => CalcAverage<int, long, double>(source, (value, total) => value + total, (total, count) => total / (double) count);

        [SuppressMessage("Design", "CA1062")]
        public static double? Average(this IEnumerable<long?> source)
            => CalcAverage<long, long, double>(source, (value, total) => value + total, (total, count) => total / (double) count);

        [SuppressMessage("Design", "CA1062")]
        public static double? Average(this IEnumerable<double?> source)
            => CalcAverage<double, double, double>(source, (value, total) => value + total, (total, count) => total / count);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Average(this IEnumerable<decimal?> source)
            => CalcAverage<decimal, decimal, decimal>(source, (value, total) => value + total, (total, count) => total / count);

        [SuppressMessage("Design", "CA1062")]
        public static float? Average(this IEnumerable<float?> source)
            => CalcAverage<float, float, float>(source, (value, total) => value + total, (total, count) => total / count);

        static TResult CalcAverageWithSelector<TSource, TAggregate, TSelected, TResult>(this IEnumerable<TSource> source,
            Func<TAggregate, TSelected, TAggregate> totalAddValue, Func<TAggregate, long, TResult> totalDivCount, Func<TSource, TSelected> selector)
            where TAggregate : struct
        {
            TAggregate total = default;
            long counter = 0;
            foreach (var element in source)
            {
                var selected = selector(element);
                if (selected == null)
                {
                    continue;
                }

                total = totalAddValue(total, selected);
                counter++;
            }

            if (counter == 0)
            {
                TResult result = default;
                if (result != null)
                {
                    ThrowSequenceEmpty<TResult>();
                }

                return result;
            }

            return totalDivCount(total, counter);
        }

        [SuppressMessage("Design", "CA1062")]
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => CalcAverageWithSelector<TSource, long, int, double>(source, (total, value) => checked(total + value), (total, count) => total / count, selector);

        [SuppressMessage("Design", "CA1062")]
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
            => CalcAverageWithSelector<TSource, long, int?, double?>(source, (total, value) => checked(total + value ?? 0), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => CalcAverageWithSelector<TSource, long, long, double>(source, (total, value) => checked(total + value), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
            => CalcAverageWithSelector<TSource, long, long?, double?>(source, (total, value) => checked(total + value ?? 0), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => CalcAverageWithSelector<TSource, double, double, double>(source, (total, value) => checked(total + value), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
            => CalcAverageWithSelector<TSource, double, double?, double?>(source, (total, value) => checked(total + value ?? 0),
                (total, count) => total / count, selector);

        [SuppressMessage("Design", "CA1062")]
        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => CalcAverageWithSelector<TSource, float, float, float>(source, (total, value) => checked(total + value), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
            => CalcAverageWithSelector<TSource, float, float?, float?>(source, (total, value) => checked(total + value ?? 0), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => CalcAverageWithSelector<TSource, decimal, decimal, decimal>(source, (total, value) => checked(total + value), (total, count) => total / count,
                selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
            => CalcAverageWithSelector<TSource, decimal, decimal?, decimal?>(source, (total, value) => checked(total + value ?? 0),
                (total, count) => total / count, selector);

        #endregion

        #region Cast

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source is IEnumerable<TResult> result)
            {
                return result;
            }

            IEnumerable<TResult> Iterator()
            {
                foreach (TResult element in source)
                {
                    yield return element;
                }
            }

            return Iterator();
        }

        #endregion

        #region Concat

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            IEnumerable<TSource> Iterator()
            {
                foreach (var element in first)
                {
                    yield return element;
                }

                foreach (var element in second)
                {
                    yield return element;
                }
            }

            return Iterator();
        }

        #endregion

        #region Contains

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return source is ICollection<TSource> collection ? collection.Contains(value) : Contains(source, value, null);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            return source.Any((i) => comparer.Equals(i, value));
        }

        #endregion

        #region Count

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source is ICollection<TSource> collection)
            {
                return collection.Count;
            }

            var counter = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    checked
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var counter = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    checked
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        #endregion

        #region DefaultIfEmpty

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source) => DefaultIfEmpty(source, default);

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                var empty = true;
                foreach (var item in source)
                {
                    empty = false;
                    yield return item;
                }

                if (empty)
                {
                    yield return defaultValue;
                }
            }

            return Iterator();
        }

        #endregion

        #region Distinct

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source) => Distinct<TSource>(source, null);

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            IEnumerable<TSource> Iterator()
            {
                var items = new Dictionary<TSource, object>(comparer);
                foreach (var element in source)
                {
                    if (!items.ContainsKey(element))
                    {
                        items.Add(element, null);
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region ElementAt

        static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index, Func<TSource> defaultValue)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (source is IList<TSource> list)
            {
                return list[index];
            }

            long counter = 0;
            foreach (var element in source)
            {
                if (index == counter++)
                {
                    return element;
                }
            }

            return defaultValue();
        }

        [SuppressMessage("Design", "CA1062")]
        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
            => source.ElementAt(index, () => throw new ArgumentOutOfRangeException(nameof(index)));

        #endregion

        #region ElementAtOrDefault

        [SuppressMessage("Design", "CA1062")]
        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
            => source.ElementAt(index, () => default);

        #endregion

        #region Empty

        public static IEnumerable<TResult> Empty<TResult>() => new TResult[0];

        #endregion

        #region Except

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
            => Except(first, second, null);

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            IEnumerable<TSource> Iterator()
            {
                var items = new Dictionary<TSource, object>(comparer);
                foreach (var item in first)
                {
                    items.Add(item, null);
                }

                foreach (var item in first)
                {
                    if (!items.ContainsKey(item))
                    {
                        items.Add(item, null);
                        yield return item;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region First

        static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource> defaultValue)
        {
            foreach (var element in source)
            {
                if (predicate?.Invoke(element) ?? true)
                {
                    return element;
                }
            }

            return defaultValue();
        }

        [SuppressMessage("Design", "CA1062")]
        public static TSource First<TSource>(this IEnumerable<TSource> source)
            => First(source, null, ThrowSequenceEmpty<TSource>);

        [SuppressMessage("Design", "CA1062")]
        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => First(source, predicate, ThrowSequenceEmpty<TSource>);

        #endregion

        #region FirstOrDefault

        [SuppressMessage("Design", "CA1062")]
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
            => First(source, null, () => default);

        [SuppressMessage("Design", "CA1062")]
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => First(source, predicate, () => default);

        #endregion

        #region GroupBy

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<IGrouping<TKey, TSource>> Iterator()
            {
                var groups = new Dictionary<TKey, List<TSource>>(comparer);
                List<TSource> defaultList = null;
                foreach (var element in source)
                {
                    var key = keySelector(element);
                    if (key == null)
                    {
                        if (defaultList == null)
                        {
                            defaultList = new List<TSource>();
                        }

                        defaultList.Add(element);
                    }
                    else
                    {
                        if (!groups.TryGetValue(key, out var group))
                        {
                            group = new List<TSource>();
                            groups.Add(key, group);
                        }

                        group.Add(element);
                    }
                }

                if (defaultList != null)
                {
                    yield return new Grouping<TKey, TSource>(default, defaultList);
                }

                foreach (var group in groups)
                {
                    yield return new Grouping<TKey, TSource>(group.Key, group.Value);
                }
            }

            return Iterator();
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => GroupBy(source, keySelector, null);

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

            IEnumerable<IGrouping<TKey, TElement>> Iterator()
            {
                var groups = new Dictionary<TKey, List<TElement>>(comparer);
                List<TElement> defaultList = null;
                foreach (var item in source)
                {
                    var key = keySelector(item);
                    var element = elementSelector(item);
                    if (key == null)
                    {
                        if (defaultList == null)
                        {
                            defaultList = new List<TElement>();
                        }

                        defaultList.Add(element);
                    }
                    else
                    {
                        if (!groups.TryGetValue(key, out var group))
                        {
                            group = new List<TElement>();
                            groups.Add(key, group);
                        }

                        group.Add(element);
                    }
                }

                if (defaultList != null)
                {
                    yield return new Grouping<TKey, TElement>(default, defaultList);
                }

                foreach (var group in groups)
                {
                    yield return new Grouping<TKey, TElement>(group.Key, group.Value);
                }
            }

            return Iterator();
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            => GroupBy(source, keySelector, elementSelector, null);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            => GroupBy(source, keySelector, elementSelector, resultSelector, null);

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            IEnumerable<TResult> Iterator()
            {
                var groups = GroupBy(source, keySelector, elementSelector, comparer);
                foreach (var group in groups)
                {
                    yield return resultSelector(group.Key, group);
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            IEnumerable<TResult> Iterator()
            {
                var groups = GroupBy(source, keySelector, comparer);
                foreach (var group in groups)
                {
                    yield return resultSelector(group.Key, group);
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            => GroupBy(source, keySelector, resultSelector, null);

        #endregion

        #region GroupJoin

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            => GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (outerKeySelector == null) throw new ArgumentNullException(nameof(outerKeySelector));
            if (innerKeySelector == null) throw new ArgumentNullException(nameof(innerKeySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            IEnumerable<TResult> Iterator()
            {
                var innerKeys = ToLookup(inner, innerKeySelector, comparer);
                foreach (var element in outer)
                {
                    var outerKey = outerKeySelector(element);
                    yield return outerKey != null && innerKeys.Contains(outerKey)
                        ? resultSelector(element, innerKeys[outerKey])
                        : resultSelector(element, Empty<TInner>());
                }
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<TKey>.Default;
            }

            return Iterator();
        }

        #endregion

        #region Intersect

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            IEnumerable<TSource> Iterator()
            {
                var items = new Dictionary<TSource, object>(comparer);
                foreach (var item in second)
                {
                    items.Add(item, null);
                }

                foreach (var element in first)
                {
                    if (items.Remove(element))
                    {
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
            => Intersect(first, second, null);

        #endregion

        #region Join

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (outer == null) throw new ArgumentNullException(nameof(outer));
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (outerKeySelector == null) throw new ArgumentNullException(nameof(outerKeySelector));
            if (innerKeySelector == null) throw new ArgumentNullException(nameof(innerKeySelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector)); 
            if (comparer == null)
            {
                comparer = EqualityComparer<TKey>.Default;
            }

            IEnumerable<TResult> Iterator()
            {
                var innerKeys = ToLookup(inner, innerKeySelector, comparer);
                foreach (var element in outer)
                {
                    var outerKey = outerKeySelector(element);
                    if (outerKey != null && innerKeys.Contains(outerKey))
                    {
                        foreach (var innerElement in innerKeys[outerKey])
                        {
                            yield return resultSelector(element, innerElement);
                        }
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            => outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);

        #endregion

        #region Last

        static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource> defaultValue)
        {
            if (predicate == null && source is IList<TSource> list)
            {
                return list[list.Count - 1];
            }

            var empty = true;
            var item = default(TSource);

            foreach (var element in source)
            {
                if (predicate != null && !predicate(element))
                {
                    continue;
                }

                item = element;
                empty = false;
            }

            if (empty)
            {
                item = defaultValue();
            }

            return item;
        }

        [SuppressMessage("Design", "CA1062")]
        public static TSource Last<TSource>(this IEnumerable<TSource> source)
            => Last(source, null, ThrowSequenceEmpty<TSource>);

        [SuppressMessage("Design", "CA1062")]
        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Last(source, predicate, ThrowSequenceEmpty<TSource>);

        #endregion

        #region LastOrDefault

        [SuppressMessage("Design", "CA1062")]
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Last(source, predicate, default);

        #endregion

        #region LongCount

        public static long LongCount<TSource>(this IEnumerable<TSource> source)
        {
            return source is Array array ? array.LongLength : LongCount(source, null);
        }

        public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            long counter = 0;
            foreach (var item in source)
            {
                if (predicate?.Invoke(item) ?? true)
                {
                    checked
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        #endregion

        #region Max

        static TResult CalcMinOrMax<TSource, TResult>(this IEnumerable<TSource> source, Func<TResult, TResult, TResult> func, Func<TResult> defaultValue,
            Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var empty = true;
            TResult max = default;
            foreach (var element in source)
            {
                var selected = selector(element);
                if (selected == null)
                {
                    continue;
                }

                if (empty)
                {
                    max = selected;
                    empty = false;
                }
                else
                {
                    max = func(selected, max);
                }
            }

            return empty ? defaultValue() : max;
        }

        [SuppressMessage("Design", "CA1062")]
        public static int Max(this IEnumerable<int> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<int>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static long Max(this IEnumerable<long> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<long>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Max(this IEnumerable<decimal> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<decimal>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static double Max(this IEnumerable<double> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<double>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static float Max(this IEnumerable<float> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<float>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static long? Max(this IEnumerable<long?> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static int? Max(this IEnumerable<int?> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Max(this IEnumerable<decimal?> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static double? Max(this IEnumerable<double?> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static float? Max(this IEnumerable<float?> source)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (typeof(IComparable<TResult>).IsAssignableFrom(typeof(TResult)))
            {
                return CalcMinOrMax(source, (element, max) => ((IComparable<TResult>) element).CompareTo(max) > 0 ? element : max, ThrowSequenceEmpty<TResult>,
                    selector);
            }

            if (typeof(IComparable).IsAssignableFrom(typeof(TResult)))
            {
                return CalcMinOrMax(source, (element, max) => ((IComparable) element).CompareTo(max) > 0 ? element : max, ThrowSequenceEmpty<TResult>,
                    selector);
            }

            throw new InvalidOperationException("IComparable or IComparable<TSource> required!");
        }

        [SuppressMessage("Design", "CA1062")]
        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<long>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<int>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<decimal>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<double>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, ThrowSequenceEmpty<float>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
            => CalcMinOrMax(source, (element, max) => element > max ? element : max, () => null, selector);

        #endregion

        #region Min

        [SuppressMessage("Design", "CA1062")]
        public static int Min(this IEnumerable<int> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<int>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static long Min(this IEnumerable<long> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<long>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Min(this IEnumerable<decimal> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<decimal>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static double Min(this IEnumerable<double> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<double>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static float Min(this IEnumerable<float> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<float>, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static long? Min(this IEnumerable<long?> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static int? Min(this IEnumerable<int?> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Min(this IEnumerable<decimal?> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static double? Min(this IEnumerable<double?> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static float? Min(this IEnumerable<float?> source)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, (i) => i);

        [SuppressMessage("Design", "CA1062")]
        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (typeof(IComparable<TResult>).IsAssignableFrom(typeof(TResult)))
            {
                return CalcMinOrMax(source, (element, max) => ((IComparable<TResult>) element).CompareTo(max) > 0 ? element : max, ThrowSequenceEmpty<TResult>,
                    selector);
            }

            if (typeof(IComparable).IsAssignableFrom(typeof(TResult)))
            {
                return CalcMinOrMax(source, (element, max) => ((IComparable) element).CompareTo(max) > 0 ? element : max, ThrowSequenceEmpty<TResult>,
                    selector);
            }

            throw new InvalidOperationException("IComparable or IComparable<TSource> required!");
        }

        [SuppressMessage("Design", "CA1062")]
        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<long>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<int>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<decimal>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<double>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, ThrowSequenceEmpty<float>, selector);

        [SuppressMessage("Design", "CA1062")]
        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, selector);

        [SuppressMessage("Design", "CA1062")]
        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
            => CalcMinOrMax(source, (element, max) => element < max ? element : max, () => null, selector);

        #endregion

        #region OfType

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                foreach (var element in source)
                {
                    if (element is TResult result)
                    {
                        yield return result;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region OrderBy

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => OrderBy(source, keySelector, null);

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
            => new OrderedEnumerable<TSource>(source).CreateOrderedEnumerable(keySelector, comparer, false);

        #endregion

        #region OrderByDescending

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => OrderByDescending(source, keySelector, null);

        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
            => new OrderedEnumerable<TSource>(source).CreateOrderedEnumerable(keySelector, comparer, true);

        #endregion

        #region Range

        public static IEnumerable<int> Range(int start, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            IEnumerable<int> Iterator()
            {
                var n = start;
                for (var i = 0; i < count; i++, n = checked(n + 1))
                {
                    yield return n;
                }
            }

            return Iterator();
        }

        #endregion

        #region Repeat

        public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            IEnumerable<TResult> Iterator()
            {
                for (var i = 0; i < count; i++)
                {
                    yield return element;
                }
            }

            return Iterator();
        }

        #endregion

        #region Reverse

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            var list = source.ToList();
            list.Reverse();
            return list;
        }

        #endregion

        #region Select

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                foreach (var element in source)
                {
                    yield return selector(element);
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                var counter = 0;
                foreach (var element in source)
                {
                    yield return selector(element, counter);
                    counter++;
                }
            }

            return Iterator();
        }

        #endregion

        #region SelectMany

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                foreach (var element in source)
                {
                    foreach (var item in selector(element))
                    {
                        yield return item;
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                var counter = 0;
                foreach (var element in source)
                {
                    foreach (var item in selector(element, counter))
                    {
                        yield return item;
                    }

                    counter++;
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element))
                    {
                        yield return resultSelector(element, collection);
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TResult> Iterator()
            {
                var counter = 0;
                foreach (var element in source)
                {
                    foreach (var collection in collectionSelector(element, counter++))
                    {
                        yield return resultSelector(element, collection);
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region Single

        static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource> defaultValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var found = false;
            var item = default(TSource);
            foreach (var element in source)
            {
                if (!predicate(element))
                {
                    continue;
                }

                if (found)
                {
                    throw new InvalidOperationException("More than one element satisfies the condition in predicate.");
                }

                found = true;
                item = element;
            }

            if (!found)
            {
                item = defaultValue();
            }

            return item;
        }

        [SuppressMessage("Design", "CA1062")]
        public static TSource Single<TSource>(this IEnumerable<TSource> source)
            => Single(source, (i) => true, ThrowSequenceEmpty<TSource>);

        [SuppressMessage("Design", "CA1062")]
        public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Single(source, predicate, () => throw new InvalidOperationException("No element satisfies the condition in predicate."));

        #endregion

        #region SingleOrDefault

        [SuppressMessage("Design", "CA1062")]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
            => Single(source, (i) => true, () => default);


        [SuppressMessage("Design", "CA1062")]
        public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
            => Single(source, predicate, () => default);

        #endregion

        #region Skip

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                using (var enumerator = source.GetEnumerator())
                {
                    while (count-- > 0)
                    {
                        if (!enumerator.MoveNext())
                        {
                            yield break;
                        }
                    }

                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region SkipWhile

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                var yield = false;
                foreach (var element in source)
                {
                    if (yield)
                    {
                        yield return element;
                    }
                    else if (!predicate(element))
                    {
                        yield = true;
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                var counter = 0;
                var yield = false;

                foreach (var element in source)
                {
                    if (yield)
                    {
                        yield return element;
                    }
                    else if (!predicate(element, counter))
                    {
                        yield = true;
                        yield return element;
                    }

                    counter++;
                }
            }

            return Iterator();
        }

        #endregion

        #region Sum

        static TSource CalcSum<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> calc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            TSource result = default;
            foreach (var element in source)
            {
                result = calc(result, element);
            }

            return result;
        }

        static TResult CalcSum<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Func<TResult, TResult, TResult> calc)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            TResult result = default;
            foreach (var element in source)
            {
                var selected = selector(element);
                if (selected != null)
                {
                    result = calc(result, selected);
                }
            }

            return result;
        }

        [SuppressMessage("Design", "CA1062")]
        public static int Sum(this IEnumerable<int> source)
            => CalcSum(source, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static int? Sum(this IEnumerable<int?> source)
            => CalcSum(source, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
            => CalcSum(source, selector, (result, element) => result + element);
        
        [SuppressMessage("Design", "CA1062")]
        public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
            => CalcSum(source, selector, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static long Sum(this IEnumerable<long> source)
            => CalcSum(source, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static long? Sum(this IEnumerable<long?> source)
            => CalcSum(source, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
            => CalcSum(source, selector, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
            => CalcSum(source, selector, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static double Sum(this IEnumerable<double> source)
            => CalcSum(source, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static double? Sum(this IEnumerable<double?> source)
            => CalcSum(source, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
            => CalcSum(source, selector, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
            => CalcSum(source, selector, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static float Sum(this IEnumerable<float> source)
            => CalcSum(source, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static float? Sum(this IEnumerable<float?> source)
            => CalcSum(source, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
            => CalcSum(source, selector, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
            => CalcSum(source, selector, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Sum(this IEnumerable<decimal> source)
            => CalcSum(source, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Sum(this IEnumerable<decimal?> source)
            => CalcSum(source, (result, element) => element.HasValue ? result ?? 0 + element : result);

        [SuppressMessage("Design", "CA1062")]
        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
            => CalcSum(source, selector, (result, element) => result + element);

        [SuppressMessage("Design", "CA1062")]
        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
            => CalcSum(source, selector, (result, element) => element.HasValue ? result ?? 0 + element : result);

        #endregion

        #region Take

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                if (count <= 0)
                {
                    yield break;
                }

                var counter = 0;
                foreach (var element in source)
                {
                    if (counter++ == count)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region TakeWhile

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                foreach (var element in source)
                {
                    if (predicate(element))
                    {
                        yield return element;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                var counter = 0;
                foreach (var element in source)
                {
                    if (predicate(element, counter++))
                    {
                        yield return element;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region ThenBy

        [SuppressMessage("Design", "CA1062")]
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => ThenBy(source, keySelector, null);

        [SuppressMessage("Design", "CA1062")]
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
            => source.CreateOrderedEnumerable(keySelector, comparer, false);

        #endregion

        #region ThenByDescending

        #endregion

        #region ToArray

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source is ICollection collection)
            {
                var result = new TSource[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }

            return source.ToList().ToArray();
        }

        #endregion

        #region ToDictionary

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var result = comparer == null ? new Dictionary<TKey, TElement>() : new Dictionary<TKey, TElement>(comparer);
            foreach (var e in source)
            {
                result.Add(keySelector(e), elementSelector(e));
            }

            return result;
        }

        [SuppressMessage("Design", "CA1062")]
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            => ToDictionary(source, keySelector, elementSelector, null);

        [SuppressMessage("Design", "CA1062")]
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => ToDictionary(source, keySelector, null);

        [SuppressMessage("Design", "CA1062")]
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
            => ToDictionary(source, keySelector, (i) => i, comparer);

        #endregion

        #region ToList

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source) => source is List<TSource> list ? list : new List<TSource>(source);

        #endregion

        #region ToLookup

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            List<TElement> defaultKeyElements = null;
            var lookup = new Dictionary<TKey, List<TElement>>(comparer ?? EqualityComparer<TKey>.Default);
            foreach (var element in source)
            {
                List<TElement> list;
                var key = keySelector(element);
                if (key == null)
                {
                    if (defaultKeyElements == null)
                    {
                        defaultKeyElements = new List<TElement>();
                    }

                    list = defaultKeyElements;
                }
                else if (!lookup.TryGetValue(key, out list))
                {
                    list = new List<TElement>();
                    lookup.Add(key, list);
                }

                list.Add(elementSelector(element));
            }

            return new Lookup<TKey, TElement>(lookup, defaultKeyElements);
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
            => ToLookup(source, keySelector, (i) => i, null);

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
            => ToLookup(source, keySelector, (i) => i, comparer);

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
            => ToLookup(source, keySelector, elementSelector, null);

        #endregion

        #region SequenceEqual

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            using (var firstEnumerator = first.GetEnumerator())
            using (var secondEnumerator = second.GetEnumerator())
            {
                while (firstEnumerator.MoveNext())
                {
                    if (!secondEnumerator.MoveNext())
                    {
                        return false;
                    }

                    if (!comparer.Equals(firstEnumerator.Current, secondEnumerator.Current))
                    {
                        return false;
                    }
                }

                return !secondEnumerator.MoveNext();
            }
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second) => first.SequenceEqual(second, null);

        #endregion

        #region Union

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
            => first.Union(second, null);

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null) throw new ArgumentNullException(nameof(first)); 
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (comparer == null)
            {
                comparer = EqualityComparer<TSource>.Default;
            }

            IEnumerable<TSource> Iterator()
            {
                var items = new Dictionary<TSource, object>(comparer);
                foreach (var element in first)
                {
                    if (!items.ContainsKey(element))
                    {
                        items.Add(element, null);
                        yield return element;
                    }
                }

                foreach (var element in second)
                {
                    if (!items.ContainsKey(element))
                    {
                        items.Add(element, null);
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        #endregion

        #region Where

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                foreach (var element in source)
                {
                    if (predicate(element))
                    {
                        yield return element;
                    }
                }
            }

            return Iterator();
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            IEnumerable<TSource> Iterator()
            {
                var counter = 0;
                foreach (var element in source)
                {
                    if (predicate(element, counter))
                    {
                        yield return element;
                    }

                    counter++;
                }
            }

            return Iterator();
        }

        #endregion
    }
}

#pragma warning restore CS1591
#endif
