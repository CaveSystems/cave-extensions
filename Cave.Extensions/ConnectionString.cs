#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;
using System.Net;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides a connection string parser.
    /// The following string will be parsed:
    /// [protocol://][user[:password]@]server[:port][/path/to/somewhere]
    /// </summary>
    public struct ConnectionString : IEquatable<ConnectionString>
    {
        #region static operators

        /// <summary>
        /// Checks two ConnectionString instances for inequality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(ConnectionString a, ConnectionString b)
        {
            if (ReferenceEquals(a, null)) return ReferenceEquals(b, null);
            return a.Equals(b);
        }

        /// <summary>
        /// Checks two ConnectionString instances for equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(ConnectionString a, ConnectionString b)
        {
            if (ReferenceEquals(a, null)) return !ReferenceEquals(b, null);
            return !a.Equals(b);
        }

        /// <summary>
        /// converts a string to a connection string using <see cref="Parse(string)"/>
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static implicit operator ConnectionString(string connectionString)
        {
            if (connectionString == null) return new ConnectionString();
            return Parse(connectionString);
        }

        /// <summary>
        /// converts a connection string to a string including credentials
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
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere]
        /// </summary>
        public static ConnectionString Parse(string connectionString)
        {
            return Parse(connectionString, null, null, null, null, null, null, null);
        }

        /// <summary>
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere[?option=value[&amp;option=value]]]
        /// </summary>
        public static ConnectionString Parse(string connectionString, string defaultProtocol, string defaultUserName, string defaultPassword, string defaultServer, ushort? defaultPort, string defaultPath, string defaultOptions)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            ushort l_Port = (defaultPort != null) ? ((ushort)defaultPort) : (ushort)0;
            string l_Protocol = defaultProtocol;
            string l_Username = defaultUserName;
            string l_Password = defaultPassword;
            string l_Server = defaultServer;
            string path = defaultPath;
            string options = defaultOptions;

            //get options
            int optionsIndex = connectionString.LastIndexOf('?');
            if (optionsIndex > -1)
            {
                options = connectionString.Substring(optionsIndex + 1);
                connectionString = connectionString.Substring(0, optionsIndex);
            }

            //get protocol
            string[] parts = connectionString.Trim().Split(new string[] { "://" }, 2, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                l_Protocol = parts[0];
            }

            //get username & password, server & port & path parts
            parts = parts[parts.Length - 1].Split(new char[] { '@' }, 2);

            //get server, port and path part
            l_Server = parts[parts.Length - 1];

            //get path (if any) and remove it from server string
            int pathIndex = l_Server.IndexOfAny(new char[] { '/', '\\' });
            if ((pathIndex == 2) && (l_Protocol.ToUpperInvariant() == "FILE"))
            {
                path = l_Server;
                l_Server = null;
            }
            else if (pathIndex > -1)
            {
                path = l_Server.Substring(pathIndex + 1);
                l_Server = l_Server.Substring(0, pathIndex);
            }

            //get server and port
            if (l_Server != null)
            {
                int l_PortIndex = l_Server.IndexOf(':');
                if ((l_PortIndex == 1) && (l_Server.Length == 2))
                {
                    path = l_Server;
                    l_Server = null;
                }
                else if (l_PortIndex > -1)
                {
                    string l_PortString = l_Server.Substring(l_PortIndex + 1);
                    if (ushort.TryParse(l_PortString, out l_Port))
                    {
                        l_Server = l_Server.Substring(0, l_PortIndex);
                    }
                }
            }

            //get user and password
            if (parts.Length > 1)
            {
                string l_UsernamePassword = parts[0];
                for (int i = 1; i < parts.Length - 1; i++)
                {
                    l_UsernamePassword += "@" + parts[i];
                }
                parts = l_UsernamePassword.Split(new char[] { ':' }, 2);
                l_Username = parts[0];
                if (parts.Length > 1)
                {
                    l_Password = parts[1];
                }
            }

            if ((l_Port < 0) || (l_Port > 65535)) l_Port = 0;

            return new ConnectionString(l_Protocol, l_Username, l_Password, l_Server, l_Port, path, options);
        }

        /// <summary>
        /// Parses a specified connection string of the form [protocol://][user[:password]@]server[:port][/path/to/somewhere]
        /// </summary>
        public static bool TryParse(string connectionString, out ConnectionString result)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            ushort l_Port = 0;
            string l_Protocol = null;
            string l_Username = null;
            string l_Password = null;
            string l_Server = null;
            string path = null;
            string options = null;

            //get protocol
            string[] parts = connectionString.Split(new string[] { "://" }, 2, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                l_Protocol = parts[0];
            }
            else
            {
                result = null;
                return false;
            }

            //get username & password, server & port & path parts
            parts = parts[parts.Length - 1].Split(new char[] { '@' }, 2);

            //get server, port and path part
            l_Server = parts[parts.Length - 1];

            //get path (if any) and remove it from server string
            int pathIndex = l_Server.IndexOf('/');
            if (pathIndex > -1)
            {
                path = l_Server.Substring(pathIndex + 1);
                l_Server = l_Server.Substring(0, pathIndex);
            }

            //get server and port
            int l_PortIndex = l_Server.IndexOf(':');
            if ((l_PortIndex == 1) && (l_Server[l_PortIndex + 1] == '\\'))
            {
                path = l_Server;
                l_Server = null;
            }
            else if (l_PortIndex > -1)
            {
                string l_PortString = l_Server.Substring(l_PortIndex + 1);
                if (ushort.TryParse(l_PortString, out l_Port))
                {
                    l_Server = l_Server.Substring(0, l_PortIndex);
                }
            }

            //get user and password
            if (parts.Length > 1)
            {
                string l_UsernamePassword = parts[0];
                for (int i = 1; i < parts.Length - 1; i++)
                {
                    l_UsernamePassword += "@" + parts[i];
                }
                parts = l_UsernamePassword.Split(new char[] { ':' }, 2);
                l_Username = parts[0];
                if (parts.Length > 1)
                {
                    l_Password = parts[1];
                }
            }

            //get options
            if (path != null)
            {
                int index = path.IndexOf('?');
                if (index > -1)
                {
                    options = path.Substring(index + 1);
                    path = options.Substring(0, index);
                }
            }
            if ((l_Port < 0) || (l_Port > 65535)) l_Port = 0;

            result = new ConnectionString(l_Protocol, l_Username, l_Password, l_Server, l_Port, path, options);
            return true;
        }
        #endregion

        /// <summary>
        /// Provides a connection string with the specified data
        /// </summary>
        /// <param name="protocol">Protocol or null</param>
        /// <param name="userName">Username or null</param>
        /// <param name="password">Password</param>
        /// <param name="server">Server or null</param>
        /// <param name="port">Port or null</param>
        /// <param name="location">Path or null</param>
        /// <param name="options">list of options separated by an '&amp;' sign</param>
        public ConnectionString(string protocol, string userName, string password, string server, ushort port = 0, string location = null, string options = null)
        {
            if (string.IsNullOrEmpty(protocol)) protocol = null;
            if (string.IsNullOrEmpty(server)) server = null;
            if (string.IsNullOrEmpty(userName)) userName = null;
            if (string.IsNullOrEmpty(password)) password = null;
            if (string.IsNullOrEmpty(location)) location = null;

            Protocol = protocol;
            Server = server;
            Port = port;
            UserName = userName;
            Password = password;
            Location = location;
            Options = options;
        }

        /// <summary>
        /// Obtains the protocol
        /// </summary>
        public ConnectionType ConnectionType
        {
            get
            {
                if (Protocol != null)
                {
                    ConnectionType connectionType;
                    Protocol.TryParse(out connectionType);
                    return connectionType;
                }
                return ConnectionType.Unknown;
            }
            set
            {
                Protocol = value.ToString();
            }
        }

        /// <summary>Obtains the protocol</summary>
        public string Protocol;

        /// <summary>
        /// Obtains the username
        /// </summary>
        public string UserName;

        /// <summary>
        /// Obtains the password
        /// </summary>
        public string Password;

        /// <summary>
        /// Obtains the server address or name
        /// </summary>
        public string Server;

        /// <summary>
        /// Obtains the port of the <see cref="ConnectionString"/>
        /// </summary>
        /// <param name="defaultPort"></param>
        /// <returns></returns>
        public int GetPort(int defaultPort)
        {
            if (Port <= 0) return defaultPort;
            return Port;
        }

        /// <summary>
        /// Obtains the port
        /// </summary>
        public ushort Port;

        /// <summary>
        /// Obtains the path
        /// </summary>
        public string Location;

        /// <summary>
        /// Obtains the options
        /// </summary>
        public string Options;

        /// <summary>
        /// Changes the path and returns a new ConnectionString
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
        /// Obtains <see cref="NetworkCredential"/>s
        /// </summary>
        /// <returns></returns>
        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(UserName, Password);
        }

        /// <summary>
        /// Obtains an <see cref="Uri"/> for the <see cref="ConnectionString"/>. Only <see cref="ConnectionString"/>s with the Protocol
        /// file, ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri"/>!
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Uri ToUri(ConnectionStringPart items)
        {
            return new Uri(ToString(items));
        }

        /// <summary>
        /// Obtains an <see cref="Uri"/> for the <see cref="ConnectionString"/>. Only <see cref="ConnectionString"/>s with the Protocol
        /// file, ftp, http, https, mailto, news and nntp may be converted to an <see cref="Uri"/>!
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return new Uri(ToString());
        }

        /// <summary>
        /// Provides a connection string with (if known) or without the password
        /// </summary>
        public string ToString(ConnectionStringPart items)
        {
            StringBuilder result = new StringBuilder();
            //protocol
            if (((items & ConnectionStringPart.Protocol) != 0) && !string.IsNullOrEmpty(Protocol))
            {
                result.Append(Protocol);
                result.Append("://");
            }
            //username and password
            bool username = (((items & ConnectionStringPart.UserName) != 0) && !string.IsNullOrEmpty(UserName));
            bool password = (((items & ConnectionStringPart.Password) != 0) && !string.IsNullOrEmpty(Password));
            if (username || password)
            {
                if (UserName == null)
                {
                    result.Append("");
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
            //server
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
            //path
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
        /// use <see cref="ToString(ConnectionStringPart)"/>
        /// </summary>
        public override string ToString()
        {
            return ToString(ConnectionStringPart.All);
        }

        /// <summary>
        /// Compares the ConnectionString to another <see cref="ConnectionString"/> or (connection) <see cref="string"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ConnectionString)) return false;
            return Equals((ConnectionString)obj);
        }

        /// <summary>
        /// Obtains hash code for the connection string
        /// </summary>
        public override int GetHashCode()
        {
            return ToString(ConnectionStringPart.All).GetHashCode();
        }

        #region IEquatable<ConnectionString> Member

        /// <summary>
        /// Compares the ConnectionString to another <see cref="ConnectionString"/> or (connection) <see cref="string"/>
        /// </summary>
        /// <param name="other">The object to compare with</param>
        /// <returns></returns>
        public bool Equals(ConnectionString other)
        {
            if (ReferenceEquals(null, other)) return false;
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