using System;
using System.Diagnostics.CodeAnalysis;

namespace Cave
{
    /// <summary>
    /// Provides a field name <see cref="Attribute"/> for renaming fields at database rows
    /// (Using different name at struct and database).
    /// </summary>
#if !NET20 && !NETSTANDARD13
    [ExcludeFromCodeCoverage]
#endif
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
    public abstract class SoftwareAttribute : Attribute
    {
        /// <summary>
        /// Gets the flags.
        /// </summary>
        public abstract SoftwareFlags Flags { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoftwareAttribute"/> class.
        /// </summary>
        protected SoftwareAttribute()
        {
        }
    }
}
