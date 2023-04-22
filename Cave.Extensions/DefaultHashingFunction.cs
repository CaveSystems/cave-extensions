﻿using System;
using System.Runtime.CompilerServices;

namespace Cave;

/// <summary>Provides the single instance used to combine all hashes at generated code.</summary>
public static class DefaultHashingFunction
{
    #region Static

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1>(T1 i1)
    {
        var hash = Create();
        hash.Add(i1);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2>(T1 i1, T2 i2)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3>(T1 i1, T2 i2, T3 i3)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        hash.Add(i5);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        hash.Add(i5);
        hash.Add(i6);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        hash.Add(i5);
        hash.Add(i6);
        hash.Add(i7);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        hash.Add(i5);
        hash.Add(i6);
        hash.Add(i7);
        hash.Add(i8);
        return hash.ToHashCode();
    }

    /// <summary>Combines the hashes of the specified instances.</summary>
    /// <returns>Returns the combined hashes of all specified instances.</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8, T9 i9)
    {
        var hash = Create();
        hash.Add(i1);
        hash.Add(i2);
        hash.Add(i3);
        hash.Add(i4);
        hash.Add(i5);
        hash.Add(i6);
        hash.Add(i7);
        hash.Add(i8);
        hash.Add(i9);
        return hash.ToHashCode();
    }

    /// <summary>Provides the function used to create the <see cref="IHashingFunction" /> used to combine hashes.</summary>
    public static Func<IHashingFunction> Create { get; set; } = () => new XxHash32();

    #endregion
}
