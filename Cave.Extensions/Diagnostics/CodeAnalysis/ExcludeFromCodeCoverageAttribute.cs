#if NETSTANDARD10 || NET35 || NET20
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Provides the ExcludeFromCodeCoverage attribute missing in some older .net implementations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeFromCodeCoverageAttribute"/> class.
        /// </summary>
        public ExcludeFromCodeCoverageAttribute()
        {
        }
    }
}
#endif
