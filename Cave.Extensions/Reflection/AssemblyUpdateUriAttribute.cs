using System;

namespace Cave.Reflection
{
    /// <summary>Update URI for the Assembly.</summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyUpdateUriAttribute : Attribute
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="AssemblyUpdateUriAttribute" /> class.</summary>
        /// <param name="uri">Update URI for the Assembly.</param>
        public AssemblyUpdateUriAttribute(Uri uri) => URI = uri;

        #endregion

        #region Properties

        /// <summary>Gets the update URI.</summary>
        public Uri URI { get; }

        #endregion
    }
}
