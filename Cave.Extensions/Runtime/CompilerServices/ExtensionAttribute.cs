#if NET20
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
#endif
