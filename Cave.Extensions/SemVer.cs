using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cave;

/// <summary>Provides semantic version numbers: <see href="https://semver.org/"/></summary>
/// <seealso cref="IEquatable{T}"/>
/// <seealso cref="IComparable{SemanticVersion}"/>
public class SemVer : IEquatable<SemVer>, IComparable<SemVer>, IComparable
{
    #region Fields

    /// <summary>Gets the valid chars for the meta (pre-release and build) part.</summary>
    public const string ValidCharsMeta = ValidCharsMetaParts + "+";

    /// <summary>Gets the valid chars for meta parts</summary>
    public const string ValidCharsMetaParts = ASCII.Strings.Digits + ASCII.Strings.Letters + "-.";

    #endregion Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="SemVer"/> class.</summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch version number.</param>
    /// <param name="preRelease">The pre release part without leading '-'.</param>
    /// <param name="build">The build part without leading '+'</param>
    /// <exception cref="ArgumentOutOfRangeException">major or minor or meta.</exception>
    public SemVer(int major, int minor, int patch, string? preRelease = null, string? build = null)
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
        Patch = patch;
        PreRelease = preRelease;
        Build = build;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>converts a version to a semantic version.</summary>
    /// <param name="version">The version to convert.</param>
    /// <returns>Returns a new semantic version instance.</returns>
    public static implicit operator SemVer(Version version) => new(version.Major, version.Minor, version.Build, version.Revision > -1 ? $".{version.Revision}" : null);

    /// <summary>Implements the operator !=.</summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(SemVer version1, SemVer version2) => !Equals(version1, version2);

    /// <summary>Implements the operator &lt;.</summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator <(SemVer version1, SemVer version2)
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
    public static bool operator <=(SemVer version1, SemVer version2)
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

    /// <summary>Implements the operator ==.</summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(SemVer version1, SemVer version2) => Equals(version1, version2);

    /// <summary>Implements the operator &gt;.</summary>
    /// <param name="version1">The version1.</param>
    /// <param name="version2">The version2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator >(SemVer version1, SemVer version2)
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
    public static bool operator >=(SemVer version1, SemVer version2)
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

    /// <summary>Parses the specified value major.minor[.patch][-meta[.pre]].</summary>
    /// <param name="value">The value.</param>
    /// <returns>the semantic version.</returns>
    /// <exception cref="InvalidDataException">Error on parsing.</exception>
    public static SemVer Parse(string value)
    {
        if (!TryParse(value, true, out var result))
        {
            throw new InvalidOperationException($"Invalid data at version {value}!");
        }
        return result;
    }

    /// <summary>Parses the specified value major.minor[.patch][-meta[.pre]].</summary>
    /// <param name="text">The value.</param>
    /// <param name="throwEx">if set to <c>true</c> [throw exception on parser error].</param>
    /// <param name="version">The version.</param>
    /// <returns>Returns true if the version was parsed successfully, false otherwise.</returns>
    /// <exception cref="InvalidDataException">error on parsing.</exception>
    public static bool TryParse(string text, bool throwEx, out SemVer version)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        //get build part
        var split = text.Split(['+'], 2);
        var build = split.Length > 1 ? split[1] : null;
        var value = split[0];
        //get pre-release part
        split = value.Split(['-'], 2);
        var preRelease = split.Length > 1 ? split[1] : null;
        value = split[0];
        //get core version
#if NET20 || NET35
        Version coreVersion;
        bool result;
        try
        {
            coreVersion = new(value);
            result = true;
        }
        catch
        {
            result = false;
            coreVersion = new(0, 0);
            if (throwEx)
            {
                throw;
            }
        }
#else
        var result = Version.TryParse(value, out var coreVersion);
        if (!result)
        {
            if (throwEx)
            {
                throw new InvalidOperationException($"Invalid SemanticVersion.Core '{value}'!");
            }
        }
#endif
        version = new(coreVersion?.Major ?? 0, coreVersion?.Minor ?? 0, coreVersion?.Build ?? -1, preRelease, build);
        if (build?.HasInvalidChars(ValidCharsMetaParts) is true)
        {
            result = false;
            if (throwEx)
            {
                throw new InvalidOperationException($"Invalid SemanticVersion.Build '{build}'!");
            }
        }
        if (preRelease?.HasInvalidChars(ValidCharsMetaParts) is true)
        {
            result = false;
            if (throwEx)
            {
                throw new InvalidOperationException($"Invalid SemanticVersion.PreRelease '{preRelease}'!");
            }
        }
        return result;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows,
    /// or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(object? other) => other is SemVer version ? CompareTo(version) : 1;

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows,
    /// or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(SemVer? other)
    {
        if (other is null)
        {
            return -1;
        }

        var result = ValueComparer.Compare(Major, other.Major);
        if (result != 0)
        {
            return result;
        }

        var minor = Minor < 0 ? 0 : Minor;
        var otherMinor = other.Minor < 0 ? 0 : other.Minor;
        result = ValueComparer.Compare(minor, otherMinor);
        if (result != 0)
        {
            return result;
        }

        var patch = Patch < 0 ? 0 : Patch;
        var otherPatch = other.Patch < 0 ? 0 : other.Patch;
        result = ValueComparer.Compare(patch, otherPatch);
        if (result != 0)
        {
            return result;
        }

        result = MetaComparer.Compare(PreRelease ?? string.Empty, other.PreRelease ?? string.Empty);
        if (result != 0)
        {
            return result;
        }

        result = MetaComparer.Compare(Build ?? string.Empty, other.Build ?? string.Empty);
        return result;
    }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
    public bool Equals(SemVer? other) => other is not null && (CompareTo(other) == 0);

    /// <summary>Determines whether the specified <see cref="object"/>, is equal to this instance.</summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj switch
    {
        SemVer semVer => Equals(semVer),
        Version version => Equals(version),
        _ => false
    };

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => ToString().GetHashCode();

    /// <summary>Returns a <see cref="string"/> that represents this instance.</summary>
    /// <returns>A <see cref="string"/> that represents this instance.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        _ = sb.Append($"{Major}.{Minor}");
        if (Patch > -1)
        {
            _ = sb.Append($".{Patch}");
        }
        _ = sb.Append(Meta);
        return sb.ToString();
    }

    #endregion Public Methods

    #region Properties

    /// <summary>
    /// Gets or sets the comparer for the meta data (pre-release and build) part. This is a simple ordinal comparer, because the meta data is only used to
    /// determine precedence if the core version is equal and in this case the meta data is compared as strings.
    /// </summary>
    public static StringComparer MetaComparer { get; set; } = StringComparer.Ordinal;

    /// <summary>Provides a default comparer for integer values.</summary>
    public static Comparer<int> ValueComparer { get; set; } = Comparer<int>.Default;

    /// <summary>Gets the build version string.</summary>
    public string? Build { get; }

    /// <summary>Gets the core version. <![CDATA[<version core> ::= <major> "." <minor> "." <patch>]]></summary>
    public Version Core => Patch < 0 ? new(Major, Minor) : new Version(Major, Minor, Patch);

    /// <summary>Gets a value indicating whether the meta data contains only valid chars or not.</summary>
    public bool IsMetaValid =>
        (Meta is null || ((Meta.Count(c => c == '+') <= 1) && !Meta.HasInvalidChars(ValidCharsMeta)))
     && PreRelease?.StartsWith('-') is not true
     && Build?.StartsWith('+') is not true;

    /// <summary>Gets the major version number.</summary>
    public int Major { get; }

    /// <summary>Gets the meta data.</summary>
    public string? Meta
    {
        get
        {
            var s1 = string.IsNullOrEmpty(PreRelease) ? null : $"-{PreRelease}";
            var s2 = string.IsNullOrEmpty(Build) ? null : $"+{Build}";
            if ((s1 == null) && (s2 == null))
            {
                return null;
            }
            return $"{s1}{s2}";
        }
    }

    /// <summary>Gets the minor version number.</summary>
    public int Minor { get; }

    /// <summary>Gets the patch version number (this may be -1 if there is no patch version number set).</summary>
    public int Patch { get; }

    /// <summary>Gets the pre release version string.</summary>
    public string? PreRelease { get; }

    /// <summary>Returns the version without the <see cref="Build"/> part.</summary>
    public SemVer WithoutBuild => new(Major, Minor, Patch, PreRelease);

    #endregion Properties
}
