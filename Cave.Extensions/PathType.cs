using System;

namespace Cave;

/// <summary>Gets available path types.</summary>
[Flags]
public enum PathType
{
    /// <summary>Unknown, relative or invalid path.</summary>
    None,

    /// <summary>Absolute rooted path.</summary>
    Absolute = 1 << 0,

    /// <summary>Uniform Naming Convention e.g. //server or ip/share/path</summary>
    Unc = 1 << 2,

    /// <summary>Full qualified connection string e.g. protocol://[user[:password]@]server/share and path</summary>
    ConnectionString = 1 << 3
}
