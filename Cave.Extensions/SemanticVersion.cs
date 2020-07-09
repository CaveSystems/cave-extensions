using System;
using System.IO;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides semantic version numbers.
    /// </summary>
    /// <seealso cref="IEquatable{SemanticVersion}" />
    /// <seealso cref="IComparable{SemanticVersion}" />
    public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>, IComparable
    {
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
        public static bool operator ==(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                return version2 is null;
            }

            if (version2 is null)
            {
                return false;
            }

            return version1.Equals(version2);
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="version1">The version1.</param>
        /// <param name="version2">The version2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SemanticVersion version1, SemanticVersion version2)
        {
            if (version1 is null)
            {
                throw new ArgumentNullException(nameof(version1));
            }

            if (version2 is null)
            {
                throw new ArgumentNullException(nameof(version2));
            }

            return !version1.Equals(version2);
        }

        /// <summary>Gets the valid chars.</summary>
        /// <value>The valid chars.</value>
        public const string ValidChars = "0123456789abcdefghijklmnopqrstuvwxyz.-+";

        /// <summary>Parses the specified value major.minor[.patch][-meta[.pre]].</summary>
        /// <param name="value">The value.</param>
        /// <returns>the semantic version.</returns>
        /// <exception cref="InvalidDataException">Error on parsing.</exception>
        public static SemanticVersion Parse(string value)
        {
            TryParse(value, true, out SemanticVersion result);
            return result;
        }

        /// <summary>Tries to parse the specified value major.minor[.patch][-meta[.pre]].</summary>
        /// <param name="value">The value.</param>
        /// <returns>the semantic version.</returns>
        public static SemanticVersion TryParse(string value)
        {
            TryParse(value, false, out SemanticVersion result);
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
            bool result = true;
            var parts = value.Split(new char[] { '.' }, 3);
            int major = 1, minor = 0, patch = 0;
            string meta = null;
            if (parts.Length > 0 && !int.TryParse(parts[0], out major))
            {
                if (throwEx)
                {
                    throw new InvalidDataException(string.Format("Invalid major version {0}!", parts[0]));
                }

                result = false;
            }
            if (parts.Length > 1 && !int.TryParse(parts[1], out minor))
            {
                if (throwEx)
                {
                    throw new InvalidDataException(string.Format("Invalid minor version {0}!", parts[1]));
                }

                result = false;
            }
            if (parts.Length > 2)
            {
                int i = parts[2].IndexOfAny(new char[] { '-', '+', '.' });
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
                        throw new InvalidDataException(string.Format("Invalid patch version {0}!", spatch));
                    }

                    result = false;
                }
                if (meta != null && meta.HasInvalidChars(ValidChars))
                {
                    if (throwEx)
                    {
                        throw new InvalidDataException(string.Format("Invalid meta data {0}!", meta));
                    }

                    meta = meta.ToLower().GetValidChars(ValidChars);
                    result = false;
                }
            }
            version = new SemanticVersion(major, minor, patch, meta);
            return result;
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

        /// <summary>Initializes a new instance of the <see cref="SemanticVersion"/> class.</summary>
        /// <param name="major">The major version number.</param>
        /// <param name="minor">The minor version number.</param>
        /// <param name="patch">The patch version number.</param>
        /// <param name="meta">The meta data to append.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// major
        /// or
        /// minor
        /// or
        /// meta.
        /// </exception>
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

            if (meta != null && meta.HasInvalidChars(ValidChars))
            {
                throw new ArgumentOutOfRangeException(nameof(meta));
            }

            Major = major;
            Minor = minor;
            Patch = patch.GetValueOrDefault(-1);
            Meta = meta;
        }

        /// <summary>Gets the classic version (calculates a build number based on the characters).</summary>
        /// <returns>the classic version.</returns>
        public Version GetClassicVersion()
        {
            if (Meta != null)
            {
                int mul = ValidChars.Length + 1;
                double value = 0;
                foreach (char c in Meta.ToLower())
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

                    int build = (int)Math.Round(value);
                    return new Version(Major, Minor, Patch > 0 ? Patch : 0, build);
                }
            }

            if (Patch > -1)
            {
                return new Version(Major, Minor, Patch);
            }

            return new Version(Major, Minor);
        }

        /// <summary>Gets the normalized version.</summary>
        /// <returns>the normalized version.</returns>
        public Version GetNormalizedVersion()
        {
            if (Patch > -1)
            {
                return new Version(Major, Minor, Patch);
            }

            return new Version(Major, Minor);
        }

        /// <summary>Gets an absolute value for this version.</summary>
        /// <returns>an absolute value.</returns>
        public decimal ToAbsolute()
        {
            StringBuilder sb = new StringBuilder();
            decimal main = Major;
            main *= int.MaxValue;
            main += Minor;
            main *= int.MaxValue;
            main += Patch;

            decimal fraction = 0;
            decimal max = 1;
            foreach (char c in Meta)
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
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Major}.{Minor}");
            if (Patch > -1)
            {
                sb.Append($".{Patch}");
            }

            if (Meta != null)
            {
                if (Meta.IndexOfAny(new char[] { '.', '-', '+' }) != 0)
                {
                    sb.Append('-');
                }

                sb.Append(Meta);
            }
            return sb.ToString();
        }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SemanticVersion version)
            {
                Equals(version);
            }

            return base.Equals(obj);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SemanticVersion other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Major == Major && other.Minor == Minor && other.Patch == Patch && other.Meta == Meta;
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes,
        /// follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(SemanticVersion other)
        {
            if (other is null)
            {
                return 1;
            }

            return ToAbsolute().CompareTo(other.ToAbsolute());
        }

        /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes,
        /// follows, or occurs in the same position in the sort order as the other object.</summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(object other)
        {
            if (other is SemanticVersion)
            {
                return CompareTo((SemanticVersion)other);
            }

            return 1;
        }
    }
}
