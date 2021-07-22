using System;
#if !NET20 && !NETSTANDARD13
using System.Diagnostics.CodeAnalysis;
#endif

namespace Cave
{
    /// <summary>Gets a field name <see cref="Attribute" /> for renaming fields at database rows (Using different name at struct and database).</summary>
#if !NET20 && !NETSTANDARD13
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
#endif
    public abstract class SoftwareAttribute : Attribute
    {
        #region Properties

        /// <summary>Gets the flags.</summary>
        public abstract SoftwareFlags Flags { get; }

        #endregion
    }
}

