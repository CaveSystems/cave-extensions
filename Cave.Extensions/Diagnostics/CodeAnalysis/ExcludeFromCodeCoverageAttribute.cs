#if NETSTANDARD10 || NET35 || NET20
#pragma warning disable CS1591, IDE0055, IDE0130

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Gets the ExcludeFromCodeCoverage attribute missing in some older .net implementations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple
 = false, Inherited = false)]
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

#pragma warning restore CS1591, IDE0055, IDE0130
#endif
