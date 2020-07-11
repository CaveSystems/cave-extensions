using System;

namespace Cave.Reflection
{
    /// <summary>
    /// Update URI for the Assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyUpdateUriAttribute : Attribute
    {
        /// <summary>
        /// Gets the update URI.
        /// </summary>
        public Uri URI { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyUpdateUriAttribute"/> class.
        /// </summary>
        /// <param name="uri">Update URI for the Assembly.</param>
        public AssemblyUpdateUriAttribute(Uri uri)
        {
            URI = uri;
        }
    }
}
