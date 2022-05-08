using System;

#nullable enable

namespace Cave
{
    /// <summary>Provides semantic version numbers: <see href="https://semver.org/"/></summary>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IComparable{SemanticVersion}" />
    [Obsolete("Use SemVer instead!")]
    public class SemanticVersion : SemVer
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="preRelease">The pre release part without leading '-'.</param>
        /// <param name="build">The build part without leading '+'</param>
        /// <exception cref="ArgumentOutOfRangeException">major or minor or meta.</exception>
        public SemanticVersion(int major, int minor, int patch, string? preRelease = null, string? build = null) : base(major, minor, patch, preRelease, build) { }

        #endregion

        #region Members

        /// <summary>Gets the normalized version.</summary>
        /// <returns>the normalized version.</returns>
        [Obsolete("Use Core instead!")]
        public Version GetNormalizedVersion() => Core;

        #endregion
    }
}

