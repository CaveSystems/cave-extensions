using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Cave
{
    /// <summary>Gets semantic version numbers.</summary>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IComparable{SemanticVersion}" />
    [SuppressMessage("Globalization", "CA1308")]
    public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IComparable
    {
        /// <summary>Gets the valid chars.</summary>
        /// <value>The valid chars.</value>
        public const string ValidChars = "0123456789abcdefghijklmnopqrstuvwxyz.-+";

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

            if ((meta != null) && meta.HasInvalidChars(ValidChars))
            {
                throw new ArgumentOutOfRangeException(nameof(meta));
            }

            Major = major;
            Minor = minor;
            Patch = patch.GetValueOrDefault(-1);
            Meta = meta;
        }

        /// <summary>Gets the major version number.</summary>
        /// <value>The major.</value>
        public int Major { get; }

        /// <summary>Gets the minor version number.</summary>
        /// <value>The minor.</value>
        public int Minor { get; }

        /// <summary>Gets the patch version number (this may be -1 if there is no patch version number set).</summary>
        /// <value>The patch version number.</value>
        public int Patch { get; }

        /// <summary>Gets the meta data.</summary>
        /// <value>The meta data.</value>
        public string Meta { get; }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object other) => other is SemanticVersion ? CompareTo((SemanticVersion)other) : 1;

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(SemanticVersion other)
        {
            return other is null ? 1 : ToAbsolute().CompareTo(other.ToAbsolute());
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise,
        ///     false.
        /// </returns>
        public bool Equals(SemanticVersion other)
        {
            return other is null ? false : (other.Major == Major) && (other.Minor == Minor) && (other.Patch == Patch) && (other.Meta == Meta);
        }

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

            return version1.ToAbsolute() < version2.ToAbsolute();
        }

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

            return version1.ToAbsolute() > version2.ToAbsolute();
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

            return version1.ToAbsolute() <= version2.ToAbsolute();
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

            return version1.ToAbsolute() >= version2.ToAbsolute();
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SemanticVersion version1, SemanticVersion version2) => Equals(version1, version2);

        /// <summary>Implements the operator !=.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SemanticVersion version1, SemanticVersion version2) => !Equals(version1, version2);

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
            if (value == null) throw new ArgumentNullException(nameof(value));
            var result = true;
            var parts = value.Split(new[] { '.' }, 3);
            int major = 1, minor = 0, patch = 0;
            string meta = null;
            if ((parts.Length > 0) && !int.TryParse(parts[0], out major))
            {
                if (throwEx)
                {
                    throw new InvalidDataException($"Invalid major version {parts[0]}!");
                }

                result = false;
            }

            if ((parts.Length > 1) && !int.TryParse(parts[1], out minor))
            {
                if (throwEx)
                {
                    throw new InvalidDataException($"Invalid minor version {parts[1]}!");
                }

                result = false;
            }

            if (parts.Length > 2)
            {
                var i = parts[2].IndexOfAny(new[] { '-', '+', '.' });
                string spatch;
                if (i == -1)
                {
                    spatch = parts[2];
                }
                else
                {
                    spatch = parts[2].Substring(0, i);
                    meta = parts[2].Substring(i);
                }

                if (!int.TryParse(spatch, out patch))
                {
                    if (throwEx)
                    {
                        throw new InvalidDataException($"Invalid patch version {spatch}!");
                    }

                    result = false;
                }

                if ((meta != null) && meta.HasInvalidChars(ValidChars))
                {
                    if (throwEx)
                    {
                        throw new InvalidDataException($"Invalid meta data {meta}!");
                    }

                    meta = meta.ToLowerInvariant().GetValidChars(ValidChars);
                    result = false;
                }
            }

            version = new SemanticVersion(major, minor, patch, meta);
            return result;
        }


        /// <summary>Gets the classic version (calculates a build number based on the characters).</summary>
        /// <returns>the classic version.</returns>
        public Version GetClassicVersion()
        {
            if (Meta != null)
            {
                var mul = ValidChars.Length + 1;
                double value = 0;
                foreach (var c in Meta.ToLowerInvariant())
                {
                    value *= mul;
                    value += ValidChars.IndexOf(c);
                }

                if (value > 0)
                {
                    while (value > short.MaxValue)
                    {
                        value = Math.Sqrt(value);
                    }

                    var build = (int) Math.Round(value);
                    return new Version(Major, Minor, Patch > 0 ? Patch : 0, build);
                }
            }

            return Patch > -1 ? new Version(Major, Minor, Patch) : new Version(Major, Minor);
        }

        /// <summary>Gets the normalized version.</summary>
        /// <returns>the normalized version.</returns>
        public Version GetNormalizedVersion() => Patch > -1 ? new Version(Major, Minor, Patch) : new Version(Major, Minor);

        /// <summary>Gets an absolute value for this version.</summary>
        /// <returns>an absolute value.</returns>
        public decimal ToAbsolute()
        {
            decimal main = Major;
            main *= int.MaxValue;
            main += Minor;
            main *= int.MaxValue;
            main += Patch;
            decimal fraction = 0;
            decimal max = 1;
            foreach (var c in Meta)
            {
                fraction *= 256;
                fraction += c;
                max *= 256;
            }

            return main + (fraction / max);
        }

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
                if (Meta.IndexOfAny(new[] { '.', '-', '+' }) != 0)
                {
                    sb.Append('-');
                }

                sb.Append(Meta);
            }

            return sb.ToString();
        }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is SemanticVersion version)
            {
                Equals(version);
            }

            return base.Equals(obj);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => ToString().GetHashCode();
    }
}
