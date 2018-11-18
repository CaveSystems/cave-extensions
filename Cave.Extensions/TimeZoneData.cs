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
    /// Provides time zone data
    /// </summary>
    public sealed class TimeZoneData
    {
        /// <summary>
        /// Parses a TimeZoneData from a string with a time zone name and optional [+/- Offset]
        /// </summary>
        /// <param name="text">Zone[+/-Offset]</param>
        /// <returns></returns>
        public static TimeZoneData Parse(string text)
        {
            TimeZoneData result;
            DateTimeParser.ParseTimeZone(text, out result);
            return result;
        }

        /// <summary>
        /// Searches for a timezone by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static TimeZoneData Get(string name, TimeSpan offset)
        {
            foreach (TimeZoneData data in TimeZones.GetList())
            {
                if (data.Name == name)
                {
                    return new TimeZoneData(data.Description, data.Name, data.Offset + offset);
                }
            }
#if NET35 || NET40 || NET45 || NET46 || NET47 || NETSTANDARD20
			throw new TimeZoneNotFoundException();
#elif NET20 || NETSTANDARD13
            throw new ArgumentException("Timezone not found!");
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
        }

        /// <summary>
        /// Obtains the current date time for this time zone
        /// </summary>
        public DateTime Now { get { return DateTime.UtcNow + Offset; } }

        /// <summary>
        /// The description of the time zone
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The name of the time zone
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The time zones offset from UTC/GMT
        /// </summary>
        public TimeSpan Offset { get; }

        /// <summary>
        /// Creates a new time zone data with the specified informations
        /// </summary>
        /// <param name="description"></param>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        public TimeZoneData(string description, string name, TimeSpan offset)
        {
            Name = name;
            Description = description;
            Offset = offset;
        }

        /// <summary>
        /// Obtains the timezone as UTC+Offset string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Offset == TimeSpan.Zero)
            {
                return Name;
            }

            if (Offset > TimeSpan.Zero)
            {
                return string.Format("{0}+{1,2:00}:{2,2:00}", Name, Offset.Hours, Offset.Minutes);
            }
            else
            {
                return string.Format("{0}-{1,2:00}:{2,2:00}", Name, Offset.Hours, Offset.Minutes);
            }
        }

        /// <summary>
        /// Checks for equality with another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            return ToString() == obj.ToString();
        }

        /// <summary>
        /// Obtains the hashcode of this instance based on the offset from utc
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Offset.GetHashCode();
        }
    }
}
