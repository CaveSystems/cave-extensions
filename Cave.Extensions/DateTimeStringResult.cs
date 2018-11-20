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

namespace Cave.Text
{
    /// <summary>
    /// Provides a date time string parser result containing the <see cref="SubStringResult"/> for date and time.
    /// </summary>
    public struct DateTimeStringResult : IEquatable<DateTimeStringResult>
    {
        /// <summary>Implements the operator ==.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DateTimeStringResult value1, DateTimeStringResult value2)
        {
            if (ReferenceEquals(null, value1))
            {
                return ReferenceEquals(null, value2);
            }

            if (ReferenceEquals(null, value2))
            {
                return false;
            }

            return value1.Time == value2.Time && value1.Date == value2.Date;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DateTimeStringResult value1, DateTimeStringResult value2)
        {
            if (ReferenceEquals(null, value1))
            {
                return !ReferenceEquals(null, value2);
            }

            if (ReferenceEquals(null, value2))
            {
                return true;
            }

            return value1.Time != value2.Time || value1.Date != value2.Date;
        }

        /// <summary>
        /// Gets/sets Time SubStringResult
        /// </summary>
        public SubStringResult Time;

        /// <summary>
        /// Gets/sets Date SubStringResult
        /// </summary>
        public SubStringResult Date;

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DateTimeStringResult)
            {
                return base.Equals((DateTimeStringResult)obj);
            }

            return false;
        }

        /// <summary>Determines whether the specified <see cref="DateTimeStringResult" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="DateTimeStringResult" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="DateTimeStringResult" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(DateTimeStringResult other)
        {
            return other.Time == Time && other.Date == Date;
        }
    }
}
