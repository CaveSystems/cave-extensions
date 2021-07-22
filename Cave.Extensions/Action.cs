
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
    /// Represents the method that performs an action on the specified object.
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Represents the method that performs an action on the specified object.
    /// </summary>
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// Represents the method that performs an action on the specified object.
    /// </summary>
    public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Represents the method that performs an action on the specified object.
    /// </summary>
    public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
#endif

