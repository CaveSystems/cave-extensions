using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cave.Collections;

/// <summary>Gets a default comparer.</summary>
public struct DefaultComparer
{
    #region Private Fields

    /// <summary>Provides access to the current comparer value.</summary>
    int value;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>Creates a new instance of the <see cref="DefaultComparer"/> structure.</summary>
    public DefaultComparer() => value = 0;

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// This function compares each item in the first array to the item at the same index at the second array. The types of all items need to match and the
    /// first comparison not matching returns the result.
    /// </summary>
    /// <param name="first">First array</param>
    /// <param name="second">Second array</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int Combine<T>(T[] first, T[] second) where T : struct
    {
        if (ReferenceEquals(first, second)) return 0;
        var comparer = Comparer<T>.Default;
        for (var i = 0; i < first.Length; i++)
        {
            if (i >= second.Length) return 1;
            var result = comparer.Compare(first[i], second[i]);
            if (result != 0) return result;
        }
        return first.Length - second.Length;
    }

    /// <summary>Compares two byte arrays as fast as possible using the native bit size.</summary>
    /// <param name="array1">The first byte array to check for equality.</param>
    /// <param name="array2">The second byte array to check for equality.</param>
    /// <returns>Returns true if the object equal each other.</returns>
    public static unsafe bool Equals(byte[] array1, byte[] array2)
    {
        if (array1 == array2)
        {
            return true;
        }

        if ((array1 == null) || (array2 == null) || (array1.Length != array2.Length))
        {
            return false;
        }

        var len = array1.Length;
        if (IntPtr.Size == 8)
        {
            fixed (byte* p1 = array1, p2 = array2)
            {
                byte* x1 = p1, x2 = p2;
                if (len > 7)
                {
                    var e1 = (x1 + len) - 7;
                    for (; x1 < e1; x1 += 8, x2 += 8)
                    {
                        if (*(long*)x1 != *(long*)x2)
                        {
                            return false;
                        }
                    }
                }

                if ((len & 4) != 0)
                {
                    if (*(int*)x1 != *(int*)x2)
                    {
                        return false;
                    }

                    x1 += 4;
                    x2 += 4;
                }

                if ((len & 2) != 0)
                {
                    if (*(short*)x1 != *(short*)x2)
                    {
                        return false;
                    }

                    x1 += 2;
                    x2 += 2;
                }

                if ((len & 1) != 0)
                {
                    if (*x1 != *x2)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        fixed (byte* p1 = array1, p2 = array2)
        {
            byte* x1 = p1, x2 = p2;
            if (len > 3)
            {
                var e1 = (x1 + len) - 3;
                for (; x1 < e1; x1 += 4, x2 += 4)
                {
                    if (*(int*)x1 != *(int*)x2)
                    {
                        return false;
                    }
                }
            }

            if ((len & 2) != 0)
            {
                if (*(short*)x1 != *(short*)x2)
                {
                    return false;
                }

                x1 += 2;
                x2 += 2;
            }

            if ((len & 1) != 0)
            {
                if (*x1 != *x2)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>Compares two objects by type and if they are arrays item by item. Otherwise the objs1 equals function is used to compare it to obj2.</summary>
    /// <param name="value1">The first <see cref="object"/> to check for equality.</param>
    /// <param name="value2">The second <see cref="object"/> to check for equality.</param>
    /// <returns>Returns true if the object equal each other.</returns>
    public static new bool Equals(object value1, object value2)
    {
        if (ReferenceEquals(value1, value2))
        {
            return true;
        }

        // check references
        if (value1 is null)
        {
            return value2 is null;
        }

        if (value2 is null)
        {
            return false;
        }

        // check type
        var type1 = value1.GetType();
        if (type1 != value2.GetType())
        {
            return false;
        }

        if (value1 is DateTime dt1)
        {
            if (value2 is DateTime dt2)
            {
                return dt1.Kind == dt2.Kind
                    ? dt1.Ticks == dt2.Ticks
                    : dt1.Kind == DateTimeKind.Unspecified
                        ? dt1.Ticks == dt2.ToLocalTime().Ticks
                        : dt2.Kind == DateTimeKind.Unspecified
                            ? dt2.Ticks == dt1.ToLocalTime().Ticks
                            : object.Equals(dt1.ToUniversalTime(), dt2.ToUniversalTime());
            }

            return false;
        }

        // is array ?
        if (value1 is Array array1)
        {
            return value2 is Array array2 && ItemsEqual(array1, array2);
        }

        // is IEnumerable
        if (value1 is IEnumerable ie1)
        {
            return value2 is IEnumerable ie2 && ItemsEqual(ie1, ie2);
        }

        // check equals
        return object.Equals(value1, value2);
    }

    /// <summary>Compares items of two arrays item by item without checking the type of the array.</summary>
    /// <param name="array1">The first array its items are compared.</param>
    /// <param name="array2">The second array its items are compared.</param>
    /// <returns>Returns true if all items in both array equal each other and the number of items equals, too.</returns>
    public static bool Equals(IEnumerable array1, IEnumerable array2)
    {
        if (ReferenceEquals(array1, array2))
        {
            return true;
        }

        if (array1 is null)
        {
            return array2 is null;
        }

        return array2 is not null && ItemsEqual(array1, array2);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1>(T1 i1, T1 o1)
    {
        var result = Comparer<T1>.Default.Compare(i1, o1);
        return result == 0 ? 0 : result < 0 ? -1 : 1;
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2>(T1 i1, T1 o1, T2 i2, T2 o2)
    {
        var result = Get(i1, o1);
        return result != 0 ? result : Get(i2, o2);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3)
    {
        var result = Get(i1, o1, i2, o2);
        return result != 0 ? result : Get(i3, o3);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4)
    {
        var result = Get(i1, o1, i2, o2);
        return result != 0 ? result : Get(i3, o3, i4, o4);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5)
    {
        var result = Get(i1, o1, i2, o2);
        return result != 0 ? result : Get(i3, o3, i4, o4, i5, o5);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6)
    {
        var result = Get(i1, o1, i2, o2, i3, o3);
        return result != 0 ? result : Get(i4, o4, i5, o5, i6, o6);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7)
    {
        var result = Get(i1, o1, i2, o2, i3, o3);
        return result != 0 ? result : Get(i4, o4, i5, o5, i6, o6, i7, o7);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <typeparam name="T8">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    /// <param name="i8">this field value</param>
    /// <param name="o8">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7, T8 i8, T8 o8)
    {
        var result = Get(i1, o1, i2, o2, i3, o3, i4, o4);
        return result != 0 ? result : Get(i5, o5, i6, o6, i7, o7, i8, o8);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <typeparam name="T8">Type to compare</typeparam>
    /// <typeparam name="T9">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    /// <param name="i8">this field value</param>
    /// <param name="o8">other field value</param>
    /// <param name="i9">this field value</param>
    /// <param name="o9">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7, T8 i8, T8 o8,
        T9 i9, T9 o9)
    {
        var result = Get(i1, o1, i2, o2, i3, o3, i4, o4);
        return result != 0 ? result : Get(i5, o5, i6, o6, i7, o7, i8, o8, i9, o9);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <typeparam name="T8">Type to compare</typeparam>
    /// <typeparam name="T9">Type to compare</typeparam>
    /// <typeparam name="T10">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    /// <param name="i8">this field value</param>
    /// <param name="o8">other field value</param>
    /// <param name="i9">this field value</param>
    /// <param name="o9">other field value</param>
    /// <param name="i10">this field value</param>
    /// <param name="o10">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7, T8 i8, T8 o8,
        T9 i9, T9 o9, T10 i10, T10 o10)
    {
        var result = Get(i1, o1, i2, o2, i3, o3, i4, o4, i5, o5);
        return result != 0 ? result : Get(i6, o6, i7, o7, i8, o8, i9, o9, i10, o10);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <typeparam name="T8">Type to compare</typeparam>
    /// <typeparam name="T9">Type to compare</typeparam>
    /// <typeparam name="T10">Type to compare</typeparam>
    /// <typeparam name="T11">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    /// <param name="i8">this field value</param>
    /// <param name="o8">other field value</param>
    /// <param name="i9">this field value</param>
    /// <param name="o9">other field value</param>
    /// <param name="i10">this field value</param>
    /// <param name="o10">other field value</param>
    /// <param name="i11">this field value</param>
    /// <param name="o11">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7, T8 i8, T8 o8,
        T9 i9, T9 o9, T10 i10, T10 o10, T11 i11, T11 o11)
    {
        var result = Get(i1, o1, i2, o2, i3, o3, i4, o4, i5, o5);
        return result != 0 ? result : Get(i6, o6, i7, o7, i8, o8, i9, o9, i10, o10, i11, o11);
    }

    /// <summary>Gets the comparison between values. Uses <see cref="Get{T1}"/> on each pair and returns on the first difference.</summary>
    /// <typeparam name="T1">First type to compare</typeparam>
    /// <param name="i1">this.field1 to compare</param>
    /// <param name="o1">other.field^1 to compare</param>
    /// <returns>Returns -1, 0 or 1</returns>
    /// <typeparam name="T2">Type to compare</typeparam>
    /// <typeparam name="T3">Type to compare</typeparam>
    /// <typeparam name="T4">Type to compare</typeparam>
    /// <typeparam name="T5">Type to compare</typeparam>
    /// <typeparam name="T6">Type to compare</typeparam>
    /// <typeparam name="T7">Type to compare</typeparam>
    /// <typeparam name="T8">Type to compare</typeparam>
    /// <typeparam name="T9">Type to compare</typeparam>
    /// <typeparam name="T10">Type to compare</typeparam>
    /// <typeparam name="T11">Type to compare</typeparam>
    /// <typeparam name="T12">Type to compare</typeparam>
    /// <param name="i2">this field value</param>
    /// <param name="o2">other field value</param>
    /// <param name="i3">this field value</param>
    /// <param name="o3">other field value</param>
    /// <param name="i4">this field value</param>
    /// <param name="o4">other field value</param>
    /// <param name="i5">this field value</param>
    /// <param name="o5">other field value</param>
    /// <param name="i6">this field value</param>
    /// <param name="o6">other field value</param>
    /// <param name="i7">this field value</param>
    /// <param name="o7">other field value</param>
    /// <param name="i8">this field value</param>
    /// <param name="o8">other field value</param>
    /// <param name="i9">this field value</param>
    /// <param name="o9">other field value</param>
    /// <param name="i10">this field value</param>
    /// <param name="o10">other field value</param>
    /// <param name="i11">this field value</param>
    /// <param name="o11">other field value</param>
    /// <param name="i12">this field value</param>
    /// <param name="o12">other field value</param>
    [MethodImpl((MethodImplOptions)256)]
    public static int Get<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 i1, T1 o1, T2 i2, T2 o2, T3 i3, T3 o3, T4 i4, T4 o4, T5 i5, T5 o5, T6 i6, T6 o6, T7 i7, T7 o7, T8 i8, T8 o8,
        T9 i9, T9 o9, T10 i10, T10 o10, T11 i11, T11 o11, T12 i12, T12 o12)
    {
        var result = Get(i1, o1, i2, o2, i3, o3, i4, o4, i5, o5, i6, o6);
        return result != 0 ? result : Get(i7, o7, i8, o8, i9, o9, i10, o10, i11, o11, i12, o12);
    }

    /// <summary>Gets the current comparer value.</summary>
    /// <param name="comparer"></param>
    [MethodImpl((MethodImplOptions)256)]
    public static implicit operator int(DefaultComparer comparer) => comparer.value;

    /// <summary>Compares items of two arrays item by item without checking the type of the array.</summary>
    /// <param name="array1">The first array its items are compared.</param>
    /// <param name="array2">The second array its items are compared.</param>
    /// <returns>Returns true if all items in both array equal each other and the number of items equals, too.</returns>
    public static bool ItemsEqual(IEnumerable array1, IEnumerable array2)
    {
        if (array1 is null)
        {
            throw new ArgumentNullException(nameof(array1));
        }

        if (array2 is null)
        {
            throw new ArgumentNullException(nameof(array2));
        }

        var enumerator1 = array1.GetEnumerator();
        var enumerator2 = array2.GetEnumerator();
        enumerator1.Reset();
        enumerator2.Reset();
        while (true)
        {
            var moved1 = enumerator1.MoveNext();
            var moved2 = enumerator2.MoveNext();
            if (moved1 != moved2)
            {
                return false;
            }

            if (!moved1)
            {
                return true;
            }

            if (!object.Equals(enumerator1.Current, enumerator2.Current))
            {
                return false;
            }
        }
    }

    /// <summary>Calculates the comparison and returns whether a difference is found or not.</summary>
    /// <typeparam name="T">Type to compare.</typeparam>
    /// <param name="thisField">The first field to compare</param>
    /// <param name="otherField">The second field to compare</param>
    /// <returns>Returns true if the comparison does not result in equality</returns>
    [MethodImpl((MethodImplOptions)256)]
    public bool Add<T>(T thisField, T otherField)
    {
        if (value != 0)
        {
            throw new InvalidOperationException("Comparison already detected a difference. Do not call Add() if a previous call returned true!");
        }
        value = Get(thisField, otherField);
        return value != 0;
    }

    #endregion Public Methods
}
