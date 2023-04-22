namespace Cave;

/// <summary>Gets all supported connection types.</summary>
public enum ConnectionType
{
    /// <summary>Unknown or undefined protocol</summary>
    Unknown = 0,

    /// <summary>Direct memory access</summary>
    MEMORY,

    /// <summary>Named pipe communication</summary>
    PIPE,

    /// <summary>Direct file access</summary>
    FILE,

    /// <summary>User Datagram Protocol</summary>
    UDP,

    /// <summary>Transmission Control Protocol</summary>
    TCP,

    /// <summary>Secure Transmission Control Protocol (using Secure Socket Layer)</summary>
    STCP,

    /// <summary>Hypertext Transfer Protocol</summary>
    HTTP,

    /// <summary>Secure Hypertext Transfer Protocol (using Secure Socket Layer)</summary>
    HTTPS,

    /// <summary>File Transfer Protocol</summary>
    FTP,

    /// <summary>Secure File Transfer Protocol (using Secure Socket Layer)</summary>
    FTPS,

    /// <summary>Syslog Message Access Protocol</summary>
    SYSMAP,

    /// <summary>Connection to mysql database</summary>
    MYSQL,

    /// <summary>Connection to sqlite database</summary>
    SQLITE,

    /// <summary>Connection to mssql database</summary>
    MSSQL,

    /// <summary>The microsoft alias for <see cref="MSSQL" /></summary>
    MICROSOFT = MSSQL,

    /// <summary>Connection to postgre sql database</summary>
    PGSQL,

    /// <summary>The postgresql alias for <see cref="PGSQL" /></summary>
    POSTGRESQL = PGSQL
}
