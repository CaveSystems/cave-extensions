#if NET20 || NET35

using System;

namespace NUnit.Framework
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!object.Equals(expected, actual))
            {
                throw new Exception("Items are not equal!");
            }
        }
    }
}

#endif