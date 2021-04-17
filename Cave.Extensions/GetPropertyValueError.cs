using System.Reflection;

namespace Cave
{
    /// <summary>Provides error codes for <see cref="ObjectExtension.TryGetPropertyValue(object, string, out object, BindingFlags)" /> functions.</summary>
    public enum GetPropertyValueError
    {
        /// <summary>No error</summary>
        None = 0,

        /// <summary>Invalid path. There is no property found matching the specified path.</summary>
        InvalidPath,

        /// <summary>Nullreference. The path could not be walked to the desired target because of a null reference.</summary>
        NullReference,

        /// <summary>Target type does not match.</summary>
        InvalidType
    }
}
