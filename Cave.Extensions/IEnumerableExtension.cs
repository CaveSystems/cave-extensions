using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cave.Collections.Generic;

namespace Cave;

/// <summary>Some additional linq extensions.</summary>
public static class IEnumerableExtension
{
    #region Static

    /// <summary>Returns all items after the specified <paramref name="item"/>. Items are checked using <see cref="object.Equals(object, object)"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The items to iterate</param>
    /// <param name="item">Item to find</param>
    /// <returns>A new sequence of items.</returns>
    public static IEnumerable<T> After<T>(this IEnumerable<T> enumerable, T item)
    {
        //don't throw exception Skip(1) throws on empty list
        var first = true;
        foreach (var result in AfterInclusive(enumerable, item))
        {
            if (first)
            {
                first = false;
            }
            else
            {
                yield return result;
            }
        }
    }

    /// <summary>Returns all items after the specified <paramref name="item"/> including the first matching item.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The items to iterate</param>
    /// <param name="item">Item to find</param>
    /// <returns>A new sequence of items.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static IEnumerable<T> AfterInclusive<T>(this IEnumerable<T> enumerable, T item) => enumerable.SkipWhile(i => !Equals(item, i));

    /// <summary>Returns a read-only wrapper for the specified list.</summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    /// <param name="items">The items to wrap in a read-only wrapper.</param>
    /// <returns>A read-only wrapper for the specified array.</returns>
    public static IList<T> AsReadOnly<T>(this IEnumerable<T> items) => items is IList<T> list ? list.AsReadOnly() : items.AsList().AsReadOnly();

    /// <summary>Returns all items after the specified <paramref name="item"/>. Items are checked using <see cref="object.Equals(object, object)"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The items to iterate</param>
    /// <param name="item">Item to find</param>
    /// <returns>A new sequence of items.</returns>
    public static IEnumerable<T> Before<T>(this IEnumerable<T> enumerable, T item) => enumerable.TakeWhile(i => !Equals(i, item));

    /// <summary>Returns all items after the specified <paramref name="item"/> including item. Items are checked using <see cref="object.Equals(object, object)"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The items to iterate</param>
    /// <param name="item">Item to find</param>
    /// <returns>A new sequence of items.</returns>
    public static IEnumerable<T> BeforeInclusive<T>(this IEnumerable<T> enumerable, T item)
    {
        foreach (var i in enumerable)
        {
            yield return i;
            if (Equals(i, item)) break;
        }
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryAnd<T>(this IEnumerable<T> items, Func<T, long> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = -1;
        foreach (var item in items)
        {
            value &= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryAnd<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0xffffffffffffffff;
        foreach (var item in items)
        {
            value &= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryAnd<T>(this IEnumerable<T> items, Func<T, int> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = -1;
        foreach (var item in items)
        {
            value &= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryAnd<T>(this IEnumerable<T> items, Func<T, uint> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0xffffffff;
        foreach (var item in items)
        {
            value &= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryAnd(this IEnumerable<long> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = -1;
        foreach (var item in items)
        {
            value &= item;
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryAnd(this IEnumerable<ulong> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0xffffffffffffffff;
        foreach (var item in items)
        {
            value &= item;
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryAnd(this IEnumerable<int> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = -1;
        foreach (var item in items)
        {
            value &= item;
        }

        return value;
    }

    /// <summary>Computes the binary and result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary and result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryAnd(this IEnumerable<uint> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0xffffffff;
        foreach (var item in items)
        {
            value &= item;
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryOr<T>(this IEnumerable<T> items, Func<T, long> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = 0;
        foreach (var item in items)
        {
            value |= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryOr<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        ulong value = 0;
        foreach (var item in items)
        {
            value |= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryOr<T>(this IEnumerable<T> items, Func<T, int> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0;
        foreach (var item in items)
        {
            value |= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryOr<T>(this IEnumerable<T> items, Func<T, uint> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        uint value = 0;
        foreach (var item in items)
        {
            value |= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryOr(this IEnumerable<long> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = 0;
        foreach (var item in items)
        {
            value |= item;
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryOr(this IEnumerable<ulong> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        ulong value = 0;
        foreach (var item in items)
        {
            value |= item;
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryOr(this IEnumerable<int> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0;
        foreach (var item in items)
        {
            value |= item;
        }

        return value;
    }

    /// <summary>Computes the binary or result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryOr(this IEnumerable<uint> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        uint value = 0;
        foreach (var item in items)
        {
            value |= item;
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryXor<T>(this IEnumerable<T> items, Func<T, long> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = 0;
        foreach (var item in items)
        {
            value ^= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryXor<T>(this IEnumerable<T> items, Func<T, ulong> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        ulong value = 0;
        foreach (var item in items)
        {
            value ^= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryXor<T>(this IEnumerable<T> items, Func<T, int> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0;
        foreach (var item in items)
        {
            value ^= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryXor<T>(this IEnumerable<T> items, Func<T, uint> predicate)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        uint value = 0;
        foreach (var item in items)
        {
            value ^= predicate(item);
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long BinaryXor(this IEnumerable<long> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        long value = 0;
        foreach (var item in items)
        {
            value |= item;
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong BinaryXor(this IEnumerable<ulong> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        ulong value = 0;
        foreach (var item in items)
        {
            value ^= item;
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int BinaryXor(this IEnumerable<int> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var value = 0;
        foreach (var item in items)
        {
            value ^= item;
        }

        return value;
    }

    /// <summary>Computes the binary xor result of a sequence of numeric values.</summary>
    /// <param name="items">The items.</param>
    /// <returns>The binary or result of the values in the sequence.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint BinaryXor(this IEnumerable<uint> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        uint value = 0;
        foreach (var item in items)
        {
            value ^= item;
        }

        return value;
    }

    /// <summary>Calculates the hash for all fields of the specified items.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items.</param>
    /// <returns>Returns the combined hashcode for all fields of all items.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculateFieldHash<T>(this IEnumerable<T> items)
        => CalculateFieldHash(items, 0);

    /// <summary>Calculates the hash for all fields of the specified items.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="bindingFlags">Flags used to search for fields.</param>
    /// <returns>Returns the combined hashcode for all fields of all items.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculateFieldHash<T>(this IEnumerable<T> items, BindingFlags bindingFlags)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (bindingFlags == 0)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        var hasher = DefaultHashingFunction.Create();
        var fields = typeof(T).GetFields(bindingFlags);
        foreach (var item in items)
        {
            foreach (var field in fields)
            {
                hasher.Add(field.GetValue(item));
            }
        }

        return hasher.ToHashCode();
    }

    /// <summary>Calculates the hash for all properties of the specified object.</summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="bindingFlags">Flags used to search for fields.</param>
    /// <returns>Returns the combined hashcode for all fields.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculateFieldHash<T>(this object obj, BindingFlags bindingFlags)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        if (bindingFlags == 0)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        var hasher = DefaultHashingFunction.Create();
        var fields = typeof(T).GetFields(bindingFlags);
        foreach (var field in fields)
        {
            var val = field.GetValue(obj);
            hasher.Add(val);
        }

        return hasher.ToHashCode();
    }

    /// <summary>Calculates the hash for all properties of the specified items.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="bindingFlags">Flags used to search for properties.</param>
    /// <returns>Returns the combined hashcode for all properties of all items.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculatePropertyHash<T>(this IEnumerable<T> items, BindingFlags bindingFlags)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }
        if (bindingFlags == 0)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        var properties = typeof(T).GetProperties(bindingFlags);
        var hasher = DefaultHashingFunction.Create();
        foreach (var item in items)
        {
            foreach (var property in properties)
            {
#if NET20 || NET35 || NET40
                var val = property.GetValue(item, null);
#elif (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_3_OR_GREATER)
                var val = property.GetValueOf(item);
#else
                var val = property.GetValue(item);
#endif
                hasher.Add(val);
            }
        }

        return hasher.ToHashCode();
    }

    /// <summary>Calculates the hash for all properties of the specified items.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items.</param>
    /// <returns>Returns the combined hashcode for all properties of all items.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculatePropertyHash<T>(this IEnumerable<T> items)
        => CalculatePropertyHash(items, 0);

    /// <summary>Calculates the hash for all properties of the specified object.</summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="bindingFlags">Flags used to search for properties.</param>
    /// <returns>Returns the combined hashcode for all properties.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long CalculatePropertyHash<T>(this object obj, BindingFlags bindingFlags)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        if (bindingFlags == 0)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        }
        var hasher = DefaultHashingFunction.Create();
        var properties = typeof(T).GetProperties(bindingFlags);
        foreach (var property in properties)
        {
#if NET20 || NET35 || NET40
            hasher.Add(property.GetValue(obj, null));
#elif (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD1_3_OR_GREATER)
            hasher.Add(property.GetValueOf(obj));
#else
            hasher.Add(property.GetValue(obj));
#endif
        }

        return hasher.ToHashCode();
    }

    /// <summary>Combines the hashes of all items using the items <see cref="object.GetHashCode"/> function.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="items">The items.</param>
    /// <returns>Returns the combined hashcode for hashes of all items.</returns>
    public static long CombineHashes<T>(this IEnumerable<T> items)
    {
        var hasher = DefaultHashingFunction.Create();
        foreach (var item in items) hasher.Add(item);
        return hasher.ToHashCode();
    }

    /// <summary>Iterates the enumeration to get the count of items.</summary>
    /// <param name="enumerable"></param>
    /// <returns>Returns the number of items.</returns>
    public static int Count(this IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        var count = 0;
        while (enumerator.MoveNext()) count++;
        return count;
    }

    /// <summary>Runs an action for each item within the specified <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">Items to enumerate</param>
    /// <param name="action">Action to run.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>Shuffles items with the specified seed. The same seed will always result in the same order.</summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="items">The items to shuffle.</param>
    /// <param name="seed">The seed.</param>
    /// <returns>List of shuffled items.</returns>
    public static IList<T> Shuffle<T>(this IEnumerable<T> items, int seed = 0)
    {
        unchecked
        {
            var result = items.ToList();
            var count = result.Count;
            var random = new Random(seed == 0 ? (int)DateTime.UtcNow.Ticks : seed);
            for (var i = result.Count - 1; i > 0; i--)
            {
                var n = random.Next(i + 1);
                var swap = result[n];
                result[n] = result[i];
                result[i] = swap;
            }
            return result;
        }
    }

    /// <summary>Returns the single item present or throws an <see cref="InvalidOperationException"/> with the specified <paramref name="errorMessage"/>.</summary>
    /// <typeparam name="T">Item type</typeparam>
    /// <param name="enumerable">Enumeration</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Returns the single item present</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T SingleOrError<T>(this IEnumerable<T> enumerable, string errorMessage)
    {
        var result = enumerable.ToList();
        if (result.Count == 1) return result[0];
        throw new InvalidOperationException(errorMessage);
    }

    /// <summary>
    /// Returns the first element for each group defined by the given key selector.
    /// </summary>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    /// <typeparam name="TKey">The type of the grouping key.</typeparam>
    /// <param name="enumerable">The source sequence.</param>
    /// <param name="predicate">A function that extracts the grouping key from each element.</param>
    /// <returns>
    /// A sequence containing the first element of each group, in the order of appearance.
    /// </returns>
    public static IEnumerable<TValue> TakeFirst<TValue, TKey>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> predicate)
    {
        var seen = new HashSet<TKey>();
        foreach (var item in enumerable)
        {
            var key = predicate(item);

            if (seen.Add(key))
                yield return item;
        }
    }

    /// <summary>
    /// Returns the last element for each group defined by the given key selector.
    /// </summary>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    /// <typeparam name="TKey">The type of the grouping key.</typeparam>
    /// <param name="enumerable">The source sequence.</param>
    /// <param name="predicate">A function that extracts the grouping key from each element.</param>
    /// <returns>
    /// A sequence containing the last element of each group.
    /// </returns>
    public static IEnumerable<TValue> TakeLast<TValue, TKey>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> predicate)
        where TKey : IEquatable<TKey>
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in enumerable)
        {
            var key = predicate(item);
            dict[key] = item; // last wins
        }

        foreach (var item in dict.Values)
            yield return item;
    }

    /// <summary>
    /// Returns one random element for each group defined by the given key selector.
    /// Groups are created using IEnumerable.ToLookup{TSource, TKey}.
    /// </summary>
    /// <typeparam name="TValue">The type of the elements.</typeparam>
    /// <typeparam name="TKey">The type of the grouping key.</typeparam>
    /// <param name="enumerable">The source sequence.</param>
    /// <param name="predicate">A function that extracts the grouping key from each element.</param>
    /// <param name="seed">
    /// Optional random seed. If zero, a non‑deterministic seed is used.
    /// </param>
    /// <returns>
    /// A sequence containing one randomly selected element per group.
    /// </returns>
    public static IEnumerable<TValue> TakeOne<TValue, TKey>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> predicate, int seed = 0)
        where TKey : IEquatable<TKey>
    {
        var random = seed == 0 ? new Random() : new Random(seed);
        var lookup = enumerable.ToLookup(predicate);
        foreach (var group in lookup)
        {
            var list = group.ToList();
            var index = random.Next(list.Count);
            yield return list[index];
        }
    }
    #endregion Static
}
