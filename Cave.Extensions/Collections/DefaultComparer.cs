using System;
using System.Collections;

namespace Cave.Collections
{
    /// <summary>Gets a default comparer.</summary>
    public static class DefaultComparer
    {
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
                            if (*(long*) x1 != *(long*) x2)
                            {
                                return false;
                            }
                        }
                    }

                    if ((len & 4) != 0)
                    {
                        if (*(int*) x1 != *(int*) x2)
                        {
                            return false;
                        }

                        x1 += 4;
                        x2 += 4;
                    }

                    if ((len & 2) != 0)
                    {
                        if (*(short*) x1 != *(short*) x2)
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
                        if (*(int*) x1 != *(int*) x2)
                        {
                            return false;
                        }
                    }
                }

                if ((len & 2) != 0)
                {
                    if (*(short*) x1 != *(short*) x2)
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

        /// <summary>
        ///     Compares two objects by type and if they are arrays item by item. Otherwise the objs1 equals function is used
        ///     to compare it to obj2.
        /// </summary>
        /// <param name="value1">The first <see cref="object" /> to check for equality.</param>
        /// <param name="value2">The second <see cref="object" /> to check for equality.</param>
        /// <returns>Returns true if the object equal each other.</returns>
        public new static bool Equals(object value1, object value2)
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
                    if (dt1.Kind == dt2.Kind) return dt1.Ticks == dt2.Ticks;
                    if (dt1.Kind == DateTimeKind.Unspecified) return dt1.Ticks == dt2.ToLocalTime().Ticks;
                    return dt2.Kind == DateTimeKind.Unspecified
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

            return array2 is null ? false : ItemsEqual(array1, array2);
        }

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
    }
}
