using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cave;

/// <summary>Some additional linq extensions.</summary>
public static class IEnumerableExtension
{
    #region Static

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

    /// <summary>Runs an action for each item within the specified <paramref name="enumerable" />.</summary>
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

    #endregion
}
