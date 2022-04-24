#if NET20

#pragma warning disable CS1591, IDE0055, IDE0130

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Gets a backport of the ExtensionAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ExtensionAttribute : Attribute
    {
    }
}

#pragma warning restore CS1591, IDE0055, IDE0130

#endif
