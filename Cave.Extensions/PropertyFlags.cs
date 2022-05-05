using System.Reflection;

namespace Cave
{
    /// <summary>
    /// Flags used at <see cref="ObjectExtension.GetProperties(object, PropertyFlags, BindingFlags, PropertyDataFilter)"/>
    /// </summary>
    public enum PropertyFlags
    {
        /// <summary>
        /// No flags: iterate all properties without recursion
        /// </summary>
        None = 0,

        /// <summary>
        /// Show only properties != null.
        /// </summary>
        FilterUnset = 1 << 0,

        /// <summary>
        /// Recurse into properties of properties.
        /// </summary>
        Recursive = 1 << 1,
    }
}
