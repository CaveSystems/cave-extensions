#if NET35 || NETSTANDARD10
#elif NET20
#pragma warning disable CS1591, IDE0055, IDE0079, IDE0130

namespace System
{
    /// <summary>
    /// Represents the method that performs an function with return value on the specified object.
    /// (Backport from net 4.0).
    /// </summary>
    public delegate T Func<T>();

    /// <summary>
    /// Represents the method that performs an function with return value on the specified object.
    /// (Backport from net 4.0).
    /// </summary>
    public delegate TResult Func<in TParam, out TResult>(TParam arg);

    /// <summary>
    /// Represents the method that performs an function with return value on the specified object.
    /// (Backport from net 4.0).
    /// </summary>
    public delegate TResult Func<in TParam, in TParam2, out TResult>(TParam arg, TParam2 arg2);
}

#pragma warning restore CS1591, IDE0055, IDE0079, IDE0130
#endif

