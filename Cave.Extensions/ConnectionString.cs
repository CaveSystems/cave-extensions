using System;
using System.Net;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides a connection string parser.
    /// The following string will be parsed:
    /// [protocol://][user[:password]@]server[:port][/path/to/somewhere].
    /// </summary>
    public struct ConnectionString : IEquatable<ConnectionString>
    {
        #region static operators

        /// <summary>
        /// Checks two ConnectionString instances for inequality.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ConnectionString a, ConnectionString b) => a.Equals(b);

        /// <summary>
        /// Checks two ConnectionString instances for equality.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ConnectionString a, ConnectionString b) => !a.Equals(b);

        /// <summary>
        /// converts a string to a connection string using <see cref="Parse(string)"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static implicit operator ConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                return default;
            }

            return Parse(connectionString);
        }

        /// <summary>
        /// converts a connection string to a string including credentials.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static implicit operator string(ConnectionString connectionString)
        {
            return connectionString.ToString(ConnectionStringPart.All);
        }

        #endregion

        #region static parsers

        /// <summary>
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere].
        /// </summary>
        /// <returns>Returns a new ConnectionString instance.</returns>
        public static ConnectionString Parse(string connectionString)
        {
            return Parse(connectionString, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere[?option=value[&amp;option=value]]].
        /// </summary>
        /// <returns>Returns a new ConnectionString instance.</returns>
        public static ConnectionString Parse(string connectionString, string defaultProtocol, string defaultUserName, string defaultPassword, string defaultServer, ushort? defaultPort, string defaultPath, string defaultOptions)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            var port = (defaultPort != null) ? ((ushort)defaultPort) : (ushort)0;
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
                options = connectionString.Substring(optionsIndex + 1);
                connectionString = connectionString.Substring(0, optionsIndex);
            }

            // get protocol
            var parts = connectionString.Trim().Split(new string[] { "://" }, 2, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                protocol = parts[0];
            }

            // get username & password, server & port & path parts
            parts = parts[parts.Length - 1].Split(new char[] { '@' }, 2);

            // get server, port and path part
            if (parts[parts.Length - 1].Trim().Length > 0)
            {
                server = parts[parts.Length - 1].Trim();
            }

            // get path (if any) and remove it from server string
            var pathIndex = server.IndexOfAny(new char[] { '/', '\\' });
            if ((pathIndex == 2) && (protocol.ToUpperInvariant() == "FILE"))
            {
                path = server;
                server = null;
            }
            else if (pathIndex > -1)
            {
                path = server.Substring(pathIndex + 1);
                server = server.Substring(0, pathIndex);
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
                    var portString = server.Substring(portIndex + 1);
                    if (ushort.TryParse(portString, out port))
                    {
                        server = server.Substring(0, portIndex);
                    }
                }
            }

            // get user and password
            if (parts.Length > 1)
            {
                var usernamePassword = parts[0];
                for (var i = 1; i < parts.Length - 1; i++)
                {
                    usernamePassword += "@" + parts[i];
                }
                parts = usernamePassword.Split(new char[] { ':' }, 2);
                username = parts[0];
                if (parts.Length > 1)
                {
                    password = parts[1];
                }
            }

            if ((port < 0) || (port > 65535))
            {
                port = 0;
            }

            return new ConnectionString(protocol, username, password, server, port, path, options);
        }

        /// <summary>
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere].
        /// </summary>
        /// <returns>Returns true on success, false otherwise.</returns>
        public static bool TryParse(string connectionString, out ConnectionString result)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            ushort port = 0;
            string protocol;
            string username = null;
            string password = null;
            string server = string.Empty;
            string path = null;
            string options = null;

            // get protocol
            var parts = connectionString.Split(new string[] { "://" }, 2, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                protocol = parts[0];
            }
            else
            {
                result = default;
                return false;
            }

            // get username & password, server & port & path parts
            parts = parts[parts.Length - 1].Split(new char[] { '@' }, 2);

            // get server, port and path part
            if (parts[parts.Length - 1].Trim().Length > 0)
            {
                server = parts[parts.Length - 1].Trim();
            }

            // get path (if any) and remove it from server string
            var pathIndex = server.IndexOf('/');
            if (pathIndex > -1)
            {
                path = server.Substring(pathIndex + 1);
                server = server.Substring(0, pathIndex);
            }

            // get server and port
            var portIndex = server.IndexOf(':');
            if ((portIndex == 1) && (server[portIndex + 1] == '\\'))
            {
                path = server;
                server = null;
            }
            else if (portIndex > -1)
            {
                var portString = server.Substring(portIndex + 1);
                if (ushort.TryParse(portString, out port))
                {
                    server = server.Substring(0, portIndex);
                }
            }

            // get user and password
            if (parts.Length > 1)
            {
                var usernamePassword = parts[0];
                for (var i = 1; i < parts.Length - 1; i++)
                {
                    usernamePassword += "@" + parts[i];
                }
                parts = usernamePassword.Split(new char[] { ':' }, 2);
                username = parts[0];
                if (parts.Length > 1)
                {
                    password = parts[1];
                }
            }

            // get options
            if (path != null)
            {
                var index = path.IndexOf('?');
                if (index > -1)
                {
                    options = path.Substring(index + 1);
                    path = options.Substring(0, index);
                }
            }
            if ((port < 0) || (port > 65535))
            {
                port = 0;
            }

            result = new ConnectionString(protocol, username, password, server, port, path, options);
            return true;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionString"/> struct.
        /// </summary>
        /// <param name="protocol">Protocol or null.</param>
        /// <param name="userName">Username or null.</param>
        /// <param name="password">Password.</param>
        /// <param name="server">Server or null.</param>
        /// <param name="port">Port or null.</param>
        /// <param name="location">Path or null.</param>
        /// <param name="options">list of options separated by an '&amp;' sign.</param>
        public ConnectionString(string protocol, string userName, string password, string server, ushort port = 0, string location = null, string options = null)
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

            if (string.IsNullOrEmpty(location))
            {
                location = null;
            }

            Protocol = protocol;
            Server = server;
            Port = port;
            UserName = userName;
            Password = password;
            Location = location;
            Options = options;
        }

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        public ConnectionType ConnectionType
        {
            get
            {
                if (Protocol != null)
                {
                    Protocol.TryParse(out ConnectionType connectionType);
                    return connectionType;
                }
                return ConnectionType.Unknown;
            }
            set => Protocol = value.ToString();
        }

        /// <summary>Gets or sets the protocol.</summary>
        public string Protocol;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password;

        /// <summary>
        /// Gets or sets the server address or name.
        /// </summary>
        public string Server;

        /// <summary>
        /// Gets or sets the port of the <see cref="ConnectionString"/>.
        /// </summary>
        /// <param name="defaultPort"></param>
        /// <returns></returns>
        public int GetPort(int defaultPort)
        {
            if (Port <= 0)
            {
                return defaultPort;
            }

            return Port;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public ushort Port;

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public string Location;

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public string Options;

        /// <summary>
        /// Changes the path and returns a new ConnectionString.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public ConnectionString ChangePath(string relativePath)
        {
            ConnectionString copy = this;
            if (System.IO.Path.IsPathRooted(relativePath) || (Location == null))
            {
                copy.Location = relativePath;
            }
            else
            {
                copy.Location = System.IO.Path.Combine(Location, relativePath);
            }
            return copy;
        }

        /// <summary>
        /// Obtains <see cref="NetworkCredential"/>s.
        /// </summary>
        /// <returns></returns>
        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        /// <summary>
        /// Obtains an <see cref="Uri"/> for the <see cref="ConnectionString"/>. Only <see cref="ConnectionString"/>s with the Protocol
        /// file, ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri"/>!.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Uri ToUri(ConnectionStringPart items)
        {
            return new Uri(ToString(items));
        }

        /// <summary>
        /// Obtains an <see cref="Uri"/> for the <see cref="ConnectionString"/>. Only <see cref="ConnectionString"/>s with the Protocol
        /// file, ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri"/>!.
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return new Uri(ToString());
        }

        /// <summary>
        /// Provides a connection string with (if known) or without the password.
        /// </summary>
        /// <returns>Returns a new string.</returns>
        public string ToString(ConnectionStringPart items)
        {
            var result = new StringBuilder();

            // protocol
            if (((items & ConnectionStringPart.Protocol) != 0) && !string.IsNullOrEmpty(Protocol))
            {
                result.Append(Protocol);
                result.Append("://");
            }

            // username and password
            var username = ((items & ConnectionStringPart.UserName) != 0) && !string.IsNullOrEmpty(UserName);
            var password = ((items & ConnectionStringPart.Password) != 0) && !string.IsNullOrEmpty(Password);
            if (username || password)
            {
                if (UserName == null)
                {
                    result.Append(string.Empty);
                }
                else
                {
                    result.Append(UserName);
                }
                if (password)
                {
                    result.Append(":");
                    result.Append(Password);
                }
                result.Append("@");
            }

            // server
            {
                if (((items & ConnectionStringPart.Server) != 0) && (Server != null))
                {
                    result.Append(Server);
                    if (Port > 0)
                    {
                        result.Append(":");
                        result.Append(Port.ToString());
                    }
                }
            }

            // path
            if (((items & ConnectionStringPart.Path) != 0) && !string.IsNullOrEmpty(Location))
            {
                result.Append("/");
                result.Append(Location);
            }
            if (((items & ConnectionStringPart.Options) != 0) && !string.IsNullOrEmpty(Options))
            {
                result.Append('?');
                result.Append(Options);
            }
            return result.ToString();
        }

        /// <summary>
        /// Obtains the connection string with credentials (username and password). If you want to strip some parts of the connection string
        /// use <see cref="ToString(ConnectionStringPart)"/>.
        /// </summary>
        /// <returns>Returns a new string.</returns>
        public override string ToString()
        {
            return ToString(ConnectionStringPart.All);
        }

        /// <summary>
        /// Compares the ConnectionString to another <see cref="ConnectionString"/> or (connection) <see cref="string"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ConnectionString))
            {
                return false;
            }

            return Equals((ConnectionString)obj);
        }

        /// <summary>
        /// Obtains hash code for the connection string.
        /// </summary>
        /// <returns>Returns a new hash code.</returns>
        public override int GetHashCode()
        {
            return ToString(ConnectionStringPart.All).GetHashCode();
        }

        #region IEquatable<ConnectionString> Member

        /// <summary>
        /// Compares the ConnectionString to another <see cref="ConnectionString"/> or (connection) <see cref="string"/>.
        /// </summary>
        /// <param name="other">The object to compare with.</param>
        /// <returns>Returns true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(ConnectionString other)
        {
            return
                (other.Password == Password) &&
                (other.Location == Location) &&
                (other.Port == Port) &&
                (other.Protocol == Protocol) &&
                (other.Server == Server) &&
                (other.Options == Options) &&
                (other.UserName == UserName);
        }

        #endregion
    }
}
