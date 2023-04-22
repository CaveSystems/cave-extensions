namespace Cave;

/// <summary>Gets selection of <see cref="ConnectionString" /> parts.</summary>
public enum ConnectionStringPart
{
    /// <summary>All parts of the connection string</summary>
    All = 0xFF,

    /// <summary>Everything except username and password</summary>
    NoCredentials = All & ~(UserName | Password),

    /// <summary>none</summary>
    None = 0,

    /// <summary>Protocol</summary>
    Protocol = 0x1,

    /// <summary>Username</summary>
    UserName = 0x2,

    /// <summary>Password</summary>
    Password = 0x4,

    /// <summary>Server</summary>
    Server = 0x8,

    /// <summary>Path</summary>
    Path = 0x10,

    /// <summary>Options</summary>
    Options = 0x20
}
