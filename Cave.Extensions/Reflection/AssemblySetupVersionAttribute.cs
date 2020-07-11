using System;

namespace Cave.Reflection
{
    /// <summary>
    /// Setup Version Attribute for the Assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblySetupVersionAttribute : Attribute
    {
        /// <summary>
        /// Gets the license number.
        /// </summary>
        public Version SetupVersion { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="AssemblySetupVersionAttribute"/> class.</summary>
        /// <param name="version">The version.</param>
        public AssemblySetupVersionAttribute(Version version)
        {
            SetupVersion = version;
        }

        /// <summary>Initializes a new instance of the <see cref="AssemblySetupVersionAttribute"/> class.</summary>
        /// <param name="version">The version.</param>
        public AssemblySetupVersionAttribute(string version)
            : this(new Version(version))
        {
        }
    }
}
