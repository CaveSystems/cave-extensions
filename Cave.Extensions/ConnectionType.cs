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

namespace Cave
{
    /// <summary>Obtains all supported connection types</summary>
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
        POSTGRESQL = PGSQL,
    }
}
