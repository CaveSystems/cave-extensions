using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cave
{
    /// <summary>Gets semantic version numbers.</summary>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IComparable{SemanticVersion}" />
    public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IComparable
    {
        #region Static

        /// <summary>Gets the valid chars.</summary>
        /// <value>The valid chars.</value>
        public const string ValidChars = "0123456789abcdefghijklmnopqrstuvwxyz.-+";

        /// <summary>converts a version to a semantic version.</summary>
        /// <param name="version">The version to convert.</param>
        /// <returns>Returns a new semantic version instance.</returns>
        public static implicit operator SemanticVersion(Version version) => new(version.Major, version.Minor, version.Build, version.Revision> -1 ? $".{version.Revision}" : null);

        /// <summary>Parses the specified value major.minor[.patch][-meta[.pre]].</summary>
        /// <param name="value">The value.</param>
        /// <returns>the semantic version.</returns>
        /// <exception cref="InvalidDataException">Error on parsing.</exception>
        public static SemanticVersion Parse(string value)
        {
            TryParse(value, true, out var result);
            return result;
        }

        /// <summary>Tries to parse the specified value major.minor[.patch][-meta[.pre]].</summary>
        /// <param name="value">The value.</param>
        /// <returns>the semantic version.</returns>
        public static SemanticVersion TryParse(string value)
        {
            TryParse(value, false, out var result);
            return result;
        }

        /// <summary>Parses the specified value major.minor[.patch][-meta[.pre]].</summary>
        /// <param name="value">The value.</param>
        /// <param name="throwEx">if set to <c>true</c> [throw exception on parser error].</param>
        /// <param name="version">The version.</param>
        /// <returns>Returns true if the version was parsed successfully, false otherwise.</returns>
        /// <exception cref="InvalidDataException">error on parsing.</exception>
        public static bool TryParse(string value, bool throwEx, out SemanticVersion version)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            version = null;
            var parts = value.SplitKeepSeparators('.', '-', '+');
            var itemsCount = (parts.Length + 1) / 2;

            //get major version
            if (itemsCount < 2 || !int.TryParse(parts[0], out var major))
            {
                if (throwEx)
                {
                    throw new InvalidDataException($"Invalid major version at {value}!");
                }
                return false;
            }
            // get minor version
            if (!int.TryParse(parts[2], out var minor))
            {
                if (throwEx)
                {
                    throw new InvalidDataException($"Invalid minor version number {parts[2]}!");
                }
                return false;
            }

            if (itemsCount < 3)
            {
                if (parts[1] == ".")
                {
                    version = new SemanticVersion(major, minor);
                    return true;
                }
                if (throwEx)
                {
                    throw new InvalidDataException($"Invalid minor version delimeter at {version}!");
                }
                return false;
            }

            string meta;
            if (!int.TryParse(parts[4], out var patch) || parts[3] != ".")
            {
                meta = parts.Skip(3).Join();
                version = new SemanticVersion(major, minor, meta: meta);
            }
            else
            {
                meta = parts.Skip(5).Join();
                version = new SemanticVersion(major, minor, patch, meta);
            }
            return true;
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SemanticVersion version1, SemanticVersion version2) => Equals(version1, version2);

        /// <summary>Implements the operator &gt;.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            if (version2 is null)
            {
                throw new ArgumentNullException(nameof(version2));
            }

            return version1.CompareTo(version2) > 0;
        }

        /// <summary>Implements the operator &gt;=.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            if (version2 is null)
            {
                throw new ArgumentNullException(nameof(version2));
            }

            return version1.CompareTo(version2) >= 0;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SemanticVersion version1, SemanticVersion version2) => !Equals(version1, version2);

        /// <summary>Implements the operator &lt;.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            if (version2 is null)
            {
                throw new ArgumentNullException(nameof(version2));
            }

            return version1.CompareTo(version2) < 0;
        }

        /// <summary>Implements the operator &lt;=.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            if (version2 is null)
            {
                throw new ArgumentNullException(nameof(version2));
            }

            return version1.CompareTo(version2) <= 0;
        }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="SemanticVersion" /> class.</summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="meta">The meta data to append.</param>
        /// <exception cref="ArgumentOutOfRangeException">major or minor or meta.</exception>
        public SemanticVersion(int major, int minor, int? patch = null, string meta = null)
        {
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(major));
            }

            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minor));
            }

            Major = major;
            Minor = minor;
            Patch = patch.GetValueOrDefault(-1);
            Meta = string.IsNullOrEmpty(meta) ? null : meta;
            IsMetaValid = Meta == null || (!meta.HasInvalidChars(ValidChars) && meta.First() is '-' or '+');
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the meta data contains only valid chars or not.
        /// </summary>
        public bool IsMetaValid { get; }

        /// <summary>Gets the major version number.</summary>
        /// <value>The major.</value>
        public int Major { get; }

        /// <summary>Gets the meta data.</summary>
        /// <value>The meta data.</value>
        public string Meta { get; }

        /// <summary>Gets the minor version number.</summary>
        /// <value>The minor.</value>
        public int Minor { get; }

        /// <summary>Gets the patch version number (this may be -1 if there is no patch version number set).</summary>
        /// <value>The patch version number.</value>
        public int Patch { get; }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current
        /// instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object other) => other is SemanticVersion version ? CompareTo(version) : 1;

        #endregion

        #region IComparable<SemanticVersion> Members

        static readonly Comparer<int> comparer = Comparer<int>.Default;
        static readonly StringComparer metaComparer = StringComparer.Ordinal;

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current
        /// instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(SemanticVersion other)
        {
            var result = comparer.Compare(Major, other.Major);
            if (result != 0) return result;

            var minor = (Minor < 0 ? 0 : Minor);
            var otherMinor = (other.Minor < 0 ? 0 : other.Minor);
            result = comparer.Compare(minor, otherMinor);
            if (result != 0) return result;

            var patch = (Patch < 0 ? 0 : Patch);
            var otherPatch = (other.Patch < 0 ? 0 : other.Patch);
            result = comparer.Compare(patch, otherPatch);
            if (result != 0) return result;

            return metaComparer.Compare(Meta ?? string.Empty, other.Meta ?? string.Empty);
        }

        #endregion

        #region IEquatable<SemanticVersion> Members

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(SemanticVersion other) => CompareTo(other) == 0;

        #endregion

        #region Overrides

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj switch
        {
            SemanticVersion semVer => Equals(semVer),
            Version version => Equals(version),
            _ => false,
        };

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Major}.{Minor}");
            if (Patch > -1)
            {
                sb.Append($".{Patch}");
            }

            if (Meta != null)
            {
                sb.Append(Meta);
            }

            return sb.ToString();
        }

        #endregion

        #region Members

        /// <summary>Gets the classic version (calculates a build number based on the number of characters).</summary>
        /// <returns>the classic version.</returns>
        [Obsolete("Use GetNormalizedVersion() instead.")]
        public Version GetClassicVersion() =>
            Patch < 0 ? new Version(Major, Minor) :
            Meta == null ? new Version(Major, Minor, Patch) :
            new Version(Major, Minor, Patch, Meta.Length);

        /// <summary>Gets the normalized version.</summary>
        /// <returns>the normalized version.</returns>
        public Version GetNormalizedVersion() => Patch < 0 ? new Version(Major, Minor) : new Version(Major, Minor, Patch);

        /// <summary>Gets an absolute value for this version.</summary>
        /// <returns>Returns an absolute value.</returns>
        [Obsolete("This only works for very short us ascii meta information.")]
        public decimal ToAbsolute()
        {
            decimal main = Major;
            main *= ushort.MaxValue;
            main += Minor;
            main *= ushort.MaxValue;
            main += Patch;
            decimal fraction = 0;
            if (Meta != null)
            {
                foreach (var c in Meta)
                {
                    fraction += c / 256m;
                }
            }
            return main + fraction;
        }

        #endregion
    }
}
