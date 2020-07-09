#if NET20 || NET35

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!(expected is string))
            {
                if (expected is IEnumerable ie1)
                {
                    if (actual is IEnumerable ie2)
                    {
                        var e1 = ie1.GetEnumerator();
                        var e2 = ie2.GetEnumerator();
                        e1.Reset();
                        e2.Reset();

                        for (int i = 0; ; i++)
                        {
                            if (e1.MoveNext())
                            {
                                if (e2.MoveNext())
                                {
                                    if (!object.Equals(e1.Current, e2.Current))
                                    {
                                        throw new Exception("Items are not equal!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Items are not equal!");
                                }
                            }
                            else
                            {
                                if (e2.MoveNext())
                                {
                                    throw new Exception("Items are not equal!");
                                }
                                return;
                            }
                        }
                    }
                    throw new Exception("Items are not equal!");
                }
            }
            if (!object.Equals(expected, actual))
            {
                throw new Exception("Items are not equal!");
            }
        }
    }
}

#endif