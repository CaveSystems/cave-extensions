#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20_OR_GREATER && !NETSTANDARD2_1_OR_GREATER && !NET5_0_OR_GREATER

using System.Collections.Generic;

namespace System.Linq;

public static class SkipLastExtension
{
    #region SkipLast

    public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var iterator = source.GetEnumerator();
        var hasRemainingItems = false;
        var cache = new Queue<TSource>(count + 1);
        do
        {
            if (hasRemainingItems = iterator.MoveNext())
            {
                cache.Enqueue(iterator.Current);
                if (cache.Count > count) yield return cache.Dequeue();
            }
        } while (hasRemainingItems);
    }

    #endregion SkipLast
}

#endif
