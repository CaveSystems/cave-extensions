using System;
using System.IO;
using System.Net;
using System.Text;

namespace Cave;

/// <summary>
/// Gets a connection string parser. The following string will be parsed:
/// [protocol://][user[:password]@]server[:port][/path/to/somewhere].
/// </summary>
public struct ConnectionString : IEquatable<ConnectionString>
{
    #region static operators

    /// <summary>Checks two ConnectionString instances for inequality.</summary>
    /// <param name="a">The first connection string.</param>
    /// <param name="b">The second connection string.</param>
    /// <returns>True if the connection strings are not equal.</returns>
    public static bool operator ==(ConnectionString a, ConnectionString b) => a.Equals(b);

    /// <summary>Checks two ConnectionString instances for equality.</summary>
    /// <param name="a">The first connection string.</param>
    /// <param name="b">The second connection string.</param>
    /// <returns>True if the connection strings are equal.</returns>
    public static bool operator !=(ConnectionString a, ConnectionString b) => !a.Equals(b);

    /// <summary>converts a string to a connection string using <see cref="Parse(string)" />.</summary>
    /// <param name="connectionString">The String.</param>
    /// <returns>The connection string.</returns>
    public static implicit operator ConnectionString(string connectionString) => connectionString == null ? default : Parse(connectionString);

    /// <summary>converts a connection string to a string including credentials.</summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>The string representation.</returns>
    public static implicit operator string(ConnectionString connectionString) => connectionString.ToString(ConnectionStringPart.All);

    #endregion

    #region static parsers

    /// <summary>Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere].</summary>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>Returns a new ConnectionString instance.</returns>
    public static ConnectionString Parse(string connectionString) => Parse(connectionString, null, null, null, null, null, null, null);

    /// <summary>
    /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere[?option=value[
    /// &amp;option=value]]].
    /// </summary>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="defaultProtocol">The protocol.</param>
    /// <param name="defaultUserName">The user name.</param>
    /// <param name="defaultPassword">The password.</param>
    /// <param name="defaultServer">The server.</param>
    /// <param name="defaultPort">The port.</param>
    /// <param name="defaultPath">The path.</param>
    /// <param name="defaultOptions">The options.</param>
    /// <returns>Returns a new ConnectionString instance.</returns>
    public static ConnectionString Parse(string connectionString, string defaultProtocol, string defaultUserName, string defaultPassword,
        string defaultServer, ushort? defaultPort, string defaultPath, string defaultOptions)
    {
        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        connectionString = connectionString.Replace('\\', '/');
        var port = defaultPort != null ? (ushort)defaultPort : (ushort)0;
        var protocol = defaultProtocol;
        var username = defaultUserName;
        var password = defaultPassword;
        var server = defaultServer;
        var path = defaultPath;
        var options = defaultOptions;

        // get options
        var optionsIndex = connectionString.LastIndexOf('?');
        if (optionsIndex > -1)
        {
            options = connectionString[(optionsIndex + 1)..];
            connectionString = connectionString[..optionsIndex];
        }

        // get protocol
        var parts = connectionString.Trim().Split(new[] { "://" }, 2, StringSplitOptions.None);
        if (parts.Length > 1)
        {
            protocol = parts[0];
        }

        // get username & password, server & port & path parts
        parts = parts[^1].Split(new[] { '@' }, 2);

        // get server, port and path part
        if (parts[^1].Trim().Length > 0)
        {
            server = parts[^1].Trim();

            // get path (if any) and remove it from server string
            var pathIndex = server.IndexOfAny(new[] { '/' });
            if ((pathIndex == 2) && ((protocol == null) || (protocol.ToUpperInvariant() == "FILE")))
            {
                path = server;
                server = null;
            }
            else if (pathIndex > -1)
            {
                path = server[(protocol == null ? pathIndex : pathIndex + 1)..];
                server = server[..pathIndex];
            }
        }

        // get server and port
        if (server != null)
        {
            var portIndex = server.IndexOf(':');
            if ((portIndex == 1) && (server.Length == 2))
            {
                path = server;
                server = null;
            }
            else if (portIndex > -1)
            {
                var portString = server[(portIndex + 1)..];
                if (ushort.TryParse(portString, out port))
                {
                    server = server[..portIndex];
                }
            }
        }

        // get user and password
        if (parts.Length > 1)
        {
            var usernamePassword = parts[0];
            for (var i = 1; i < (parts.Length - 1); i++)
            {
                usernamePassword += "@" + parts[i];
            }

            parts = usernamePassword.Split(new[] { ':' }, 2);
            username = parts[0];
            if (parts.Length > 1)
            {
                password = parts[1];
            }
        }

        return new(protocol, username, password, server, port, path, options);
    }

    /// <summary>Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere].</summary>
    /// <param name="connectionString">the string.</param>
    /// <param name="result">The parsed result.</param>
    /// <returns>Returns true on success, false otherwise.</returns>
    public static bool TryParse(string connectionString, out ConnectionString result)
    {
        // TODO: Implement clean TryParse function
        try
        {
            result = Parse(connectionString);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    #endregion

    /// <summary>Initializes a new instance of the <see cref="ConnectionString" /> struct.</summary>
    /// <param name="protocol">Protocol or null.</param>
    /// <param name="userName">Username or null.</param>
    /// <param name="password">Password.</param>
    /// <param name="server">Server or null.</param>
    /// <param name="port">Port or null.</param>
    /// <param name="location">Path or null.</param>
    /// <param name="options">list of options separated by an '&amp;' sign.</param>
    public ConnectionString(string protocol, string userName, string password, string server, ushort port = 0, string location = null,
        string options = null)
    {
        if (string.IsNullOrEmpty(protocol))
        {
            protocol = null;
        }

        if (string.IsNullOrEmpty(server))
        {
            server = null;
        }

        if (string.IsNullOrEmpty(userName))
        {
            userName = null;
        }

        if (string.IsNullOrEmpty(password))
        {
            password = null;
        }

        location = string.IsNullOrEmpty(location) ? null : location.Replace('\\', '/');
        Protocol = protocol;
        Server = server;
        Port = port;
        UserName = userName;
        Password = password;
        Location = location;
        Options = options;
    }

    /// <summary>Gets or sets the protocol.</summary>
    public ConnectionType ConnectionType
    {
        get => (Protocol != null) && Protocol.TryParse(out ConnectionType connectionType) ? connectionType : ConnectionType.Unknown;
        set => Protocol = value.ToString();
    }

    /// <summary>Gets or sets the protocol.</summary>
    public string Protocol;

    /// <summary>Gets or sets the username.</summary>
    public string UserName;

    /// <summary>Gets or sets the password.</summary>
    public string Password;

    /// <summary>Gets or sets the server address or name.</summary>
    public string Server;

    /// <summary>Gets or sets the port of the <see cref="ConnectionString" />.</summary>
    /// <param name="defaultPort">The default port.</param>
    /// <returns>The port.</returns>
    public int GetPort(int defaultPort) => Port <= 0 ? defaultPort : Port;

    /// <summary>Gets or sets the port.</summary>
    public ushort Port;

    /// <summary>Gets or sets the path.</summary>
    public string Location;

    /// <summary>Gets or sets the options.</summary>
    public string Options;

    /// <summary>Changes the path and returns a new ConnectionString.</summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>The connection string.</returns>
    public ConnectionString ChangePath(string relativePath)
    {
        var copy = this;
        copy.Location = Path.IsPathRooted(relativePath) || (Location == null) ? relativePath : Path.Combine(Location, relativePath);
        return copy;
    }

    /// <summary>Gets <see cref="NetworkCredential" />s.</summary>
    /// <returns>The network credentials.</returns>
    public NetworkCredential GetCredentials() => new(UserName, Password);

    /// <summary>
    /// Gets an <see cref="Uri" /> for the <see cref="ConnectionString" />. Only <see cref="ConnectionString" />s with the Protocol file,
    /// ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri" />!.
    /// </summary>
    /// <param name="items">The connection string parts.</param>
    /// <returns>The uri.</returns>
    public Uri ToUri(ConnectionStringPart items) => new(ToString(items));

    /// <summary>
    /// Gets an <see cref="Uri" /> for the <see cref="ConnectionString" />. Only <see cref="ConnectionString" />s with the Protocol file,
    /// ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri" />!.
    /// </summary>
    /// <returns>the uri.</returns>
    public Uri ToUri() => new(ToString());

    /// <summary>Gets a connection string with (if known) or without the password.</summary>
    /// <param name="items">The connection string parts.</param>
    /// <returns>Returns a new string.</returns>
    public string ToString(ConnectionStringPart items)
    {
        var result = new StringBuilder();

        // protocol
        if (((items & ConnectionStringPart.Protocol) != 0) && !string.IsNullOrEmpty(Protocol))
        {
            _ = result.Append(Protocol);
            _ = result.Append("://");
        }

        // username and password
        var username = ((items & ConnectionStringPart.UserName) != 0) && !string.IsNullOrEmpty(UserName);
        var password = ((items & ConnectionStringPart.Password) != 0) && !string.IsNullOrEmpty(Password);
        if (username || password)
        {
            if (UserName == null)
            {
                _ = result.Append(string.Empty);
            }
            else
            {
                _ = result.Append(UserName);
            }

            if (password)
            {
                _ = result.Append(':');
                _ = result.Append(Password);
            }

            _ = result.Append('@');
        }

        // server
        {
            if (((items & ConnectionStringPart.Server) != 0) && (Server != null))
            {
                _ = result.Append(Server);
                if (Port > 0)
                {
                    _ = result.Append($":{Port}");
                }
            }
        }

        // path
        if (((items & ConnectionStringPart.Path) != 0) && !string.IsNullOrEmpty(Location))
        {
            if (result.Length > 0)
            {
                _ = result.Append('/');
            }

            _ = result.Append(Location);
        }

        if (((items & ConnectionStringPart.Options) != 0) && !string.IsNullOrEmpty(Options))
        {
            _ = result.Append('?');
            _ = result.Append(Options);
        }

        return result.ToString();
    }

    /// <summary>
    /// Gets the connection string with credentials (username and password). If you want to strip some parts of the connection string use
    /// <see cref="ToString(ConnectionStringPart)" />.
    /// </summary>
    /// <returns>Returns a new string.</returns>
    public override string ToString() => ToString(ConnectionStringPart.All);

    /// <summary>Compares the ConnectionString to another <see cref="ConnectionString" /> or (connection) <see cref="string" /> .</summary>
    /// <param name="obj">The connection string.</param>
    /// <returns>True if the connection strings are equal.</returns>
    public override bool Equals(object obj) => obj is ConnectionString conStr && Equals(conStr);

    /// <summary>Gets hash code for the connection string.</summary>
    /// <returns>Returns a new hash code.</returns>
    public override int GetHashCode() => ToString(ConnectionStringPart.All).GetHashCode();

    #region IEquatable<ConnectionString> Member

    /// <summary>Compares the ConnectionString to another <see cref="ConnectionString" /> or (connection) <see cref="string" />.</summary>
    /// <param name="other">The object to compare with.</param>
    /// <returns>Returns true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(ConnectionString other) =>
        (other.Password == Password) &&
        (other.Location == Location) &&
        (other.Port == Port) &&
        (other.Protocol == Protocol) &&
        (other.Server == Server) &&
        (other.Options == Options) &&
        (other.UserName == UserName);

    #endregion
}
