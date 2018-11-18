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
using System.Collections.Generic;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Backport of enum extensions
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Gets an array containing all single flags set. (flag1, flag4, ..)
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum[] GetFlags<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            List<TEnum> flags = new List<TEnum>();
            long val = Convert.ToInt64(value);
            for (int i = 0; i < 63; i++)
            {
                long check = 1L << i;
                if (0 != (val & check))
                {
                    if (TryParse(check.ToString(), out TEnum flag))
                    {
                        flags.Add(flag);
                    }
                }
            }
            return flags.ToArray();
        }

        /// <summary>
        /// Gets a string for all single flags set. ("flag1, flag4, ..")
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetString<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            StringBuilder sb = new StringBuilder();
            long val = Convert.ToInt64(value);
            for (int i = 0; i < 63; i++)
            {
                long check = 1L << i;
                if (0 != (val & check))
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }

                    if (TryParse(check.ToString(), out TEnum flag))
                    {
                        sb.Append(flag.ToString());
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>Parses the specified string for a valid enum value.</summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value to be returned if no valid value was found.</param>
        /// <returns></returns>
        public static TEnum Parse<TEnum>(this string value, TEnum defaultValue = default(TEnum)) where TEnum : struct, IConvertible
        {
            if (!value.TryParse(out TEnum result))
            {
                result = defaultValue;
            }

            return result;
        }

#if NET40 || NET45 || NET46 || NET47 || NETSTANDARD20 || NETCOREAPP20
		/// <summary>Tries to parse the specified string.</summary>
		/// <typeparam name="TEnum">The type of the enum.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="result">The result.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static bool TryParse<TEnum>(this string value, out TEnum result) where TEnum : struct, IConvertible
        {
            return Enum.TryParse(value, true, out result);
        }
#elif NET35 || NET20 || NETCOREAPP13 || NETSTANDARD13
        /// <summary>Tries the parse.</summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool TryParse<TEnum>(this string value, out TEnum result) where TEnum : struct, IConvertible
        {
            Type t = typeof(TEnum);
            if (value == null)
            {
                result = new TEnum();
                return false;
            }
            try
            {
                result = (TEnum)Enum.Parse(t, value, true);
                return true;
            }
            catch (Exception ex)
            {
                result = new TEnum();
#if (!NETCOREAPP13 && !NETSTANDARD13)
                System.Diagnostics.Trace.TraceError("Error during Enum.TryParse() backport.\n{0}", ex);
#endif
                return false;
            }
        }

		/// <summary>
		/// Determines whether one or more bit fields are set at the enum.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool HasFlag(this Enum value, IConvertible flag)
		{
			ulong test = Convert.ToUInt64(flag);
			return (test == (Convert.ToUInt64(value) & test));
		}
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
    }
}
