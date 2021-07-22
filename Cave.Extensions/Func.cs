
/* Nicht gemergte Änderung aus Projekt "Cave.Extensions (net20)"
Vor:
#pragma warning disable SA1402 // File may only contain a single type

#if NET35 || NETSTANDARD10
Nach:
#if NET35 || NETSTANDARD10
*/


#if NET35 || NETSTANDARD10
#elif NET20
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
#endif

