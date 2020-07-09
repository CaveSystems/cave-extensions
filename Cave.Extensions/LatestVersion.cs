using System;

namespace Cave
{
    /// <summary>
    /// Provides access to LATESTVERSION files.
    /// </summary>
    public struct LatestVersion : IComparable<LatestVersion>, IEquatable<LatestVersion>
    {
        #region static functions

        /// <summary>
        /// Gets a new empty version.
        /// </summary>
        public static LatestVersion Empty
        {
            get
            {
                return new LatestVersion
                {
                    AssemblyVersion = new Version(0, 0),
                    FileVersion = new Version(0, 0),
                    SetupVersion = new Version(0, 0),
                    ReleaseDate = default(DateTime),
                    SetupPackage = string.Empty,
                    SetupArguments = string.Empty,
                    SoftwareName = string.Empty,
                    SoftwareFlags = SoftwareFlags.None,
                };
            }
        }

        /// <summary>
        /// Checks whether the specified latest version is newer then the current one.
        /// </summary>
        /// <param name="latest">latest version.</param>
        /// <param name="current">current version.</param>
        /// <returns>true if latest version is newer then current version.</returns>
        public static bool VersionIsNewer(Version latest, Version current)
        {
            if (latest == null)
            {
                throw new ArgumentNullException(nameof(latest));
            }

            return latest.CompareTo(current) > 0;
        }

        #region static comparison operators

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if first version is smaller.</returns>
        public static bool operator <(LatestVersion v1, LatestVersion v2)
        {
            if (v1.SoftwareName != v2.SoftwareName)
            {
                throw new ArgumentException(string.Format("Softwarename does not match!"));
            }

            return VersionIsNewer(v2.SetupVersion, v1.SetupVersion);
        }

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if first version is maller or equal.</returns>
        public static bool operator <=(LatestVersion v1, LatestVersion v2)
        {
            return (v1 < v2) || (v1 == v2);
        }

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if first version is greater.</returns>
        public static bool operator >(LatestVersion v1, LatestVersion v2)
        {
            if (v1.SoftwareName != v2.SoftwareName)
            {
                throw new ArgumentException(string.Format("Softwarename does not match!"));
            }

            return VersionIsNewer(v1.SetupVersion, v2.SetupVersion);
        }

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if first version is greater or equal.</returns>
        public static bool operator >=(LatestVersion v1, LatestVersion v2)
        {
            return (v1 > v2) || (v1 == v2);
        }

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if versions are equal.</returns>
        public static bool operator ==(LatestVersion v1, LatestVersion v2)
        {
            return v1.Equals(v2);
        }

        /// <summary>
        /// Provides <see cref="LatestVersion"/> comparison.
        /// </summary>
        /// <param name="v1">first version.</param>
        /// <param name="v2">second version.</param>
        /// <returns>true if versions are not equal.</returns>
        public static bool operator !=(LatestVersion v1, LatestVersion v2)
        {
            return !v1.Equals(v2);
        }
        #endregion

        #endregion

        #region public fields

        /// <summary>
        /// Gets or sets name of the Software this LatestVersion belongs to.
        /// </summary>
        public string SoftwareName { get; set; }

        /// <summary>
        /// Gets or sets the Assembly <see cref="Version"/>.
        /// </summary>
        public Version AssemblyVersion { get; set; }

        /// <summary>
        /// Gets or sets the File <see cref="Version"/>.
        /// </summary>
        public Version FileVersion { get; set; }

        /// <summary>
        /// Gets or sets the Installer <see cref="Version"/>. (This is the version used while comparing two instances).
        /// </summary>
        public Version SetupVersion { get; set; }

        /// <summary>
        /// Gets or sets path of update.
        /// </summary>
        public Uri UpdateURI { get; set; }

        /// <summary>
        /// Gets or sets the SoftwareFlags.
        /// </summary>
        public SoftwareFlags SoftwareFlags { get; set; }

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the SetupPackage fileName.
        /// </summary>
        public string SetupPackage { get; set; }

        /// <summary>
        /// Gets or sets the setup arguments.
        /// </summary>
        public string SetupArguments { get; set; }

        #endregion

        /// <summary>
        /// Returns "{<see cref="SoftwareName"/>} {<see cref="AssemblyVersion"/>}".
        /// </summary>
        /// <returns>software name and assembly version.</returns>
        public override string ToString()
        {
            return $"{SoftwareName} {AssemblyVersion}";
        }

        /// <summary>
        /// Gets the hashcode of this instance.
        /// </summary>
        /// <returns>the hashcode.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes,
        /// follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(LatestVersion other)
        {
            int result = AssemblyVersion.CompareTo(other.AssemblyVersion);
            if (result == 0)
            {
                result = FileVersion.CompareTo(other.FileVersion);
            }

            return result;
        }

        /// <summary>
        /// Checks two <see cref="LatestVersion"/> instances for equality.
        /// </summary>
        /// <param name="obj">the version to check for.</param>
        /// <returns>true if equal.</returns>
        public override bool Equals(object obj) => obj is LatestVersion other && Equals(other);

        /// <summary>
        /// Checks two <see cref="LatestVersion"/> instances for equality.
        /// </summary>
        /// <param name="other">the version to check for.</param>
        /// <returns>true if equal.</returns>
        public bool Equals(LatestVersion other) =>
            (SoftwareName == other.SoftwareName) &&
            (AssemblyVersion == other.AssemblyVersion) &&
            (FileVersion == other.FileVersion) &&
            (SetupVersion == other.SetupVersion) &&
            (SetupPackage == other.SetupPackage) &&
            (SoftwareFlags == other.SoftwareFlags);
    }
}
