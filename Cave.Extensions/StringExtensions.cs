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
#endregion Authors & Contributors

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides string functions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Provides the default date time string used when formatting date time variables for interop
        /// </summary>
        public const string InterOpDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK ";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for display
        /// </summary>
        public const string DisplayDateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for display
        /// </summary>
        public const string DisplayDateTimeWithTimeZoneFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff K";

        /// <summary>
        /// Provides the default date string used when formatting date time variables for interop
        /// </summary>
        public const string ShortDateFormat = "yyyy'-'MM'-'dd";

        /// <summary>
        /// Provides the default time string used when formatting date time variables for interop
        /// </summary>
        public const string ShortTimeFormat = "HH':'mm':'ss'.'fff";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for file names
        /// </summary>
        public const string FileNameDateTimeFormat = "yyyy'-'MM'-'dd' 'HHmmss";

        /// <summary>
        /// Joins a collection to a string with newlines for all systems
        /// </summary>
        public static string JoinNewLine(this string[] texts)
        {
            return Join(texts, "\r\n");
        }

        /// <summary>
        /// Joins a collection to a string with newlines for all systems
        /// </summary>
        public static string JoinNewLine(this IEnumerable array)
        {
            return Join(array, "\r\n");
        }

        /// <summary>
        /// Joins a collection to a string
        /// </summary>
        public static string Join(this IEnumerable array, string separator, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (separator == null)
            {
                throw new ArgumentNullException("separator");
            }

            StringBuilder result = new StringBuilder();
            foreach (object obj in array)
            {
                if (result.Length != 0)
                {
                    result.Append(separator);
                }

                result.Append(ToString(obj, cultureInfo));
            }
            return result.ToString();
        }

        /// <summary>
        /// Joins a collection to a string
        /// </summary>
        public static string Join(this IEnumerable array, char separator, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            StringBuilder result = new StringBuilder();
            foreach (object obj in array)
            {
                if (result.Length != 0)
                {
                    result.Append(separator);
                }

                result.Append(ToString(obj, cultureInfo));
            }
            return result.ToString();
        }

        /// <summary>Joins the camel case.</summary>
        /// <param name="parts">The parts.</param>
        /// <returns></returns>
        public static string JoinCamelCase(this string[] parts)
        {
            StringBuilder result = new StringBuilder();
            foreach (string part in parts)
            {
                string t = part.Trim();
                if (t.Length < 1)
                {
                    continue;
                }

                result.Append(char.ToUpper(t[0]));
                if (t.Length > 1)
                {
                    result.Append(t.Substring(1).ToLower());
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts a exception to a simple text message
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/></param>
        /// <param name="debug">Include debug information (stacktrace, data)</param>
        /// <returns></returns>
        public static string ToText(this Exception ex, bool debug = false)
        {
            return string.Join(Environment.NewLine, ToStrings(ex, debug));
        }

        /// <summary>
        /// Converts a exception to a string array
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/></param>
        /// <param name="debug">Include debug information (stacktrace, data)</param>
        /// <returns></returns>
        public static string[] ToStrings(this Exception ex, bool debug = false)
        {
            //ignore AggregateException
            if (ex is AggregateException)
            {
                return ToStrings(ex.InnerException, debug);
            }

            if (ex == null)
            {
                return new string[0];
            }

            List<string> strings = new List<string>();

            if (debug)
            {
                strings.Add("Message:");
            }

            foreach (string s in SplitNewLine(ex.Message))
            {
                if (s.Trim().Length == 0)
                {
                    continue;
                }

                if (debug)
                {
                    strings.Add("  " + s);
                }
                else
                {
                    strings.Add(s);
                }
            }

            if (debug)
            {
                if (!string.IsNullOrEmpty(ex.Source))
                {
                    strings.Add("Source:");
                    foreach (string s in SplitNewLine(ex.Source))
                    {
                        if (s.Trim().Length == 0 || !ASCII.IsClean(s))
                        {
                            continue;
                        }

                        strings.Add("  " + s);
                    }
                }
                if (ex.Data.Count > 0)
                {
                    strings.Add("Data:");
                    foreach (object key in ex.Data.Keys)
                    {
                        strings.Add(string.Format("  {0}: {1}", key, ex.Data[key]));
                    }
                }

                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    strings.Add("StackTrace:");
                    foreach (string s in SplitNewLine(ex.StackTrace))
                    {
                        if (s.Trim().Length == 0 || !ASCII.IsClean(s))
                        {
                            continue;
                        }

                        strings.Add("  " + s);
                    }
                }
            }

            if (ex.InnerException != null)
            {
                if (debug)
                {
                    strings.Add("---");
                }

                strings.AddRange(ToStrings(ex.InnerException, debug));
            }

            if (ex is ReflectionTypeLoadException)
            {
                foreach (Exception inner in ((ReflectionTypeLoadException)ex).LoaderExceptions)
                {
                    if (debug)
                    {
                        strings.Add("---");
                    }

                    strings.AddRange(ToStrings(inner, debug));
                }
            }

            return strings.ToArray();
        }

        /// <summary>
        /// Provides a fail save version of string.Format not supporting extended format options (simply replacing {index} with the arguments.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string text, params object[] args)
        {
            if (args == null)
            {
                args = new object[0];
            }

            string result = text;
            for (int i = 0; i < args.Length; i++)
            {
                string argument = (args[i] == null) ? "<null>" : args[i].ToString();
                result = result.Replace("{" + i + "}", argument);
            }
            return result;
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string FormatTime(this TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + FormatTime(-timeSpan);
            }

            if (timeSpan == TimeSpan.Zero)
            {
                return "0s";
            }

            if (timeSpan.Ticks < TimeSpan.TicksPerMillisecond)
            {
                double nano = (timeSpan.Ticks / (double)(TimeSpan.TicksPerMillisecond / 1000));
                return (nano > 9.99) ? nano.ToString("0.0") + "ns" : nano.ToString("0.00") + "ns";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerSecond)
            {
                double msec = timeSpan.TotalMilliseconds;
                return (msec > 9.99) ? msec.ToString("0.0") + "ms" : msec.ToString("0.00") + "ms";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerMinute)
            {
                double sec = timeSpan.TotalSeconds;
                return (sec > 9.99) ? sec.ToString("0.0") + "s" : sec.ToString("0.00") + "s";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerHour)
            {
                double min = timeSpan.TotalMinutes;
                return (min > 9.99) ? min.ToString("0.0") + "min" : min.ToString("0.00") + "min";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerDay)
            {
                double h = timeSpan.TotalHours;
                return (h > 9.99) ? h.ToString("0.0") + "h" : h.ToString("0.00") + "h";
            }
            double d = timeSpan.TotalDays;
            if (d >= 36525)
            {
                return (d / 365.25).ToString("0") + "a";
            }

            if (d >= 3652.5)
            {
                return (d / 365.25).ToString("0.0") + "a";
            }

            if (d > 99.9)
            {
                return (d / 365.25).ToString("0.00") + "a";
            }

            if (d > 9.99)
            {
                return d.ToString("0.0") + "d";
            }

            return d.ToString("0.00") + "d";
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTime(this double seconds)
        {
            if (seconds < 0)
            {
                return "-" + FormatTime(-seconds);
            }

            if (seconds == 0)
            {
                return "0.00";
            }

            if (seconds >= 0.1)
            {
                return FormatTime(TimeSpan.FromTicks((long)(seconds * TimeSpan.TicksPerSecond)));
            }
            double part = seconds;
            for (SI_Fractions i = SI_Fractions.m; i <= SI_Fractions.y; i++)
            {
                part *= 1000.0;
                if (part > 9.99)
                {
                    return part.ToString("0.0") + i + "s";
                }

                if (part > 0.999)
                {
                    return part.ToString("0.00") + i + "s";
                }
            }
            return seconds.ToString() + "s";
        }

        /// <summary>Formats the specified timespan to [HH:]MM:SS.F.</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="millisecondDigits">The number of millisecond digits.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only 0-3 millisecond digits are supported!</exception>
        public static string FormatTimeSpan(this TimeSpan timeSpan, int millisecondDigits)
        {
            StringBuilder result = new StringBuilder();
            if (timeSpan.Hours > 0)
            {
                result.Append(Math.Truncate(timeSpan.TotalHours));
                result.Append(":");
            }
            result.Append(timeSpan.Minutes.ToString("00"));
            result.Append(":");
            int seconds = timeSpan.Seconds;

            switch (millisecondDigits)
            {
                case 0:
                    if (timeSpan.Milliseconds > 0)
                    {
                        seconds++;
                    }

                    result.Append(seconds.ToString("00"));
                    break;
                case 1:
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append((timeSpan.Milliseconds / 100).ToString("0"));
                    break;
                case 2:
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append((timeSpan.Milliseconds / 10).ToString("00"));
                    break;
                case 3:
                    result.Append(seconds.ToString("00"));
                    result.Append(".");
                    result.Append((timeSpan.Milliseconds).ToString("000"));
                    break;
                default:
                    throw new NotSupportedException("Only 0-3 millisecond digits are supported!");
            }
            return result.ToString();
        }

        /// <summary>
        /// Provides common IEC units for binary values (byte)
        /// </summary>
        public enum IEC_Units : int
        {
            /// <summary>
            /// Byte
            /// </summary>
            B = 0,

            /// <summary>
            /// kilo Byte
            /// </summary>
            kiB,

            /// <summary>
            /// Mega Byte
            /// </summary>
            MiB,

            /// <summary>
            /// Giga Byte
            /// </summary>
            GiB,

            /// <summary>
            /// Tera Byte
            /// </summary>
            TiB,

            /// <summary>
            /// Peta Byte
            /// </summary>
            PiB,

            /// <summary>
            /// Exa Byte
            /// </summary>
            EiB,

            /// <summary>
            /// Zetta Byte
            /// </summary>
            ZiB,

            /// <summary>
            /// Yotta Byte
            /// </summary>
            YiB,
        }

        /// <summary>
        /// si unit fractions
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
        public enum SI_Fractions : int
        {
            /// <summary>
            /// Milli
            /// </summary>
            m = 1,

            /// <summary>
            /// Micro
            /// </summary>
            µ,

            /// <summary>
            /// Nano
            /// </summary>
            n,

            /// <summary>
            /// Pico
            /// </summary>
            p,

            /// <summary>
            /// Femto
            /// </summary>
            f,

            /// <summary>
            /// Atto
            /// </summary>
            a,

            /// <summary>
            /// Zepto
            /// </summary>
            z,

            /// <summary>
            /// Yocto
            /// </summary>
            y,
        }

        /// <summary>
        /// Provides the international system of units default units
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
        public enum SI_Units : int
        {
            /// <summary>
            /// kilo
            /// </summary>
            k = 1,

            /// <summary>
            /// Mega
            /// </summary>
            M,

            /// <summary>
            /// Giga
            /// </summary>
            G,

            /// <summary>
            /// Tera
            /// </summary>
            T,

            /// <summary>
            /// Peta
            /// </summary>
            P,

            /// <summary>
            /// Exa
            /// </summary>
            E,

            /// <summary>
            /// Zetta
            /// </summary>
            Z,

            /// <summary>
            /// Yota
            /// </summary>
            Y,
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatSize(this float size)
        {
            if (size < 0)
            {
                return "-" + FormatSize(-size);
            }
            float calc = size;
            SI_Units unit = 0;
            while (calc >= 1000)
            {
                calc /= 1000;
                unit++;
            }
            string result;
            if (Math.Truncate(calc) == calc)
            {
                result = calc.ToString();
            }
            else
            {
                result = calc.ToString("0.000");
            }
            if (result.Length > 5)
            {
                result = result.Substring(0, 5);
            }

            return result + ((unit == 0) ? "" : " " + unit.ToString());
        }

        /// <summary>
        /// Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatSize(this ulong size)
        {
            return FormatSize((float)size);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatSize(this long size)
        {
            return FormatSize((float)size);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatSize(this decimal size)
        {
            return FormatSize((float)size);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatSize(this double size)
        {
            return FormatSize((float)size);
        }

        /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string</returns>
        public static string FormatBinarySize(this float size)
        {
            bool negative = (size < 0);
            IEC_Units unit = 0;
            while (size >= 1024)
            {
                size /= 1024;
                unit++;
            }
            string result = size.ToString("0.000");
            if (result.Length > 5)
            {
                result = result.Substring(0, 5);
            }

            return (negative ? "-" : "") + result + " " + unit.ToString();
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatBinarySize(this double value)
        {
            return FormatBinarySize((float)value);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatBinarySize(this decimal value)
        {
            return FormatBinarySize((float)value);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatBinarySize(this ulong value)
        {
            return FormatBinarySize((float)value);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatBinarySize(this long value)
        {
            return FormatBinarySize((float)value);
        }

        /// <summary>
        /// Returns the objects.ToString() result or "&lt;null&gt;"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo">The culture to use during formatting</param>
        /// <returns></returns>
        public static string ToString(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "<null>";
            }

            if (value is IFormattable)
            {
                return ((IFormattable)value).ToString(null, cultureInfo);
            }
            if (value is ICollection)
            {
                return value.ToString() + " {" + Join((ICollection)value, ",", cultureInfo) + "}";
            }
            return value.ToString();
        }

        /// <summary>
        /// Returns the objects.ToString() result or "&lt;null&gt;"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(object value)
        {
            if (value == null)
            {
                return "<null>";
            }

            if (value is ICollection)
            {
                return value.ToString() + " {" + Join((ICollection)value, ",") + "}";
            }
            return value.ToString();
        }

        /// <summary>
        /// Returns an array of strings using the element objects ToString() method with invariant culture.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static string[] ToStringArray(this IEnumerable enumerable)
        {
            return ToStringArray(enumerable, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an array of strings using the element objects ToString() method.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="cultureInfo">The culture to use during formatting</param>
        /// <returns></returns>
        public static string[] ToStringArray(this IEnumerable enumerable, CultureInfo cultureInfo)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException("cultureInfo");
            }

            List<string> result = new List<string>();
            foreach (object obj in enumerable)
            {
                result.Add(ToString(obj, cultureInfo));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Parses a DateTime (Supported formats: <see cref="InterOpDateTimeFormat"/>, <see cref="DisplayDateTimeFormat"/>, default)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string dateTime)
        {
            { if (DateTime.TryParseExact(dateTime, InterOpDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                {
                    return result;
                }
            }
            { if (DateTime.TryParseExact(dateTime, DisplayDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                {
                    return result;
                }
            }
            return DateTime.Parse(dateTime);
        }

        /// <summary>
        /// Parses a DateTime (Supported formats: <see cref="InterOpDateTimeFormat"/>, <see cref="DisplayDateTimeFormat"/>, default)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseDateTime(string dateTime, out DateTime result)
        {
            if (DateTime.TryParseExact(dateTime, InterOpDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }

            if (DateTime.TryParseExact(dateTime, DisplayDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }

            return DateTime.TryParse(dateTime, out result);
        }

        /// <summary>
        /// Parses a Point.ToString() result
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Point ParsePoint(string point)
        {
            string data = Unbox(point.Trim(), "{", "}", true);
            string[] parts = data.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException(string.Format("Invalid point data '{0}'!", point), "point");
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("X="))
            {
                throw new ArgumentException(string.Format("Invalid point data '{0}'!", point), "point");
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y="))
            {
                throw new ArgumentException(string.Format("Invalid point data '{0}'!", point), "point");
            }

            int x = int.Parse(parts[0].Trim().Substring(2));
            int y = int.Parse(parts[1].Trim().Substring(2));
            return new Point(x, y);
        }

        /// <summary>
        /// Parses a Size.ToString() result
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Size ParseSize(string size)
        {
            string data = Unbox(size.Trim(), "{", "}", true);
            string[] parts = data.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException(string.Format("Invalid size data '{0}'!", size), "size");
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("WIDTH="))
            {
                throw new ArgumentException(string.Format("Invalid size data '{0}'!", size), "size");
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("HEIGHT="))
            {
                throw new ArgumentException(string.Format("Invalid size data '{0}'!", size), "size");
            }

            int w = int.Parse(parts[0].Trim().Substring(6));
            int h = int.Parse(parts[1].Trim().Substring(7));
            return new Size(w, h);
        }

        /// <summary>
        /// Parses a Rectangle.ToString() result
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle ParseRectangle(string rect)
        {
            string data = Unbox(rect, "{", "}", true);
            string[] parts = data.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("X="))
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y="))
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            if (!parts[2].Trim().ToUpperInvariant().StartsWith("WIDTH="))
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            if (!parts[3].Trim().ToUpperInvariant().StartsWith("HEIGHT="))
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            int x = int.Parse(parts[0].Trim().Substring(2));
            int y = int.Parse(parts[1].Trim().Substring(2));
            int w = int.Parse(parts[2].Trim().Substring(6));
            int h = int.Parse(parts[3].Trim().Substring(7));
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Parses a PointF.ToString() result
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static PointF ParsePointF(string point)
        {
            string data = Unbox(point, "{", "}", true);
            string[] parts = data.ToUpperInvariant().Split(new string[] { "X=", "Y=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException(string.Format("Invalid point data '{0}'!", point), "point");
            }

            float x = float.Parse(parts[0].Trim(' ', ','));
            float y = float.Parse(parts[1].Trim(' ', ','));
            return new PointF(x, y);
        }

        /// <summary>
        /// Parses a SizeF.ToString() result
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static SizeF ParseSizeF(string size)
        {
            string data = Unbox(size, "{", "}", true);
            string[] parts = data.ToUpperInvariant().Split(new string[] { "WIDTH=", "HEIGHT=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException(string.Format("Invalid size data '{0}'!", size), "size");
            }

            float w = float.Parse(parts[0].Trim(' ', ','));
            float h = float.Parse(parts[1].Trim(' ', ','));
            return new SizeF(w, h);
        }

        /// <summary>
        /// Parses a RectangleF.ToString() result
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RectangleF ParseRectangleF(string rect)
        {
            string data = Unbox(rect, "{", "}", true);
            string[] parts = data.ToUpperInvariant().Split(new string[] { "X=", "Y=", "WIDTH=", "HEIGHT=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                throw new ArgumentException(string.Format("Invalid rect data '{0}'!", rect), "rect");
            }

            float x = float.Parse(parts[0].Trim(' ', ','));
            float y = float.Parse(parts[1].Trim(' ', ','));
            float w = float.Parse(parts[2].Trim(' ', ','));
            float h = float.Parse(parts[3].Trim(' ', ','));
            return new RectangleF(x, y, w, h);
        }

        /// <summary>Gets a substring from the end of the specified string.</summary>
        /// <param name="text">The string.</param>
        /// <param name="count">The number of characters at the end to be retrieved.</param>
        /// <returns></returns>
        public static string SubstringEnd(this string text, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            int len = text.Length;
            count = Math.Min(count, len);
            if (count == 0)
            {
                return "";
            }

            return text.Substring(len - count);
        }

        /// <summary>Obtains a part of a string</summary>
        /// <param name="data">Data to parse</param>
        /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark)</param>
        /// <param name="startMark">StartMark to check/search for</param>
        /// <param name="endMark">EndMark to search for</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception if string cannot be found].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">data</exception>
        /// <exception cref="ArgumentException">
        /// StartMark not found!
        /// or
        /// EndMark not found!
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!</exception>
        public static string GetString(this string data, int start, char startMark, char endMark, bool throwException = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (start < 0)
            {
                start = data.IndexOf(startMark);
            }

            if (start < 0)
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentException("StartMark not found!");
            }
            if (data[start] != startMark)
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentOutOfRangeException(nameof(startMark), "StartMark does not match!");
            }
            int end = data.IndexOf(endMark, start + 1);
            if (end <= start)
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentException("EndMark not found!");
            }
            return data.Substring(start + 1, end - start - 1);
        }

        /// <summary>Obtains a part of a string</summary>
        /// <param name="data">Data to parse</param>
        /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark)</param>
        /// <param name="startMark">StartMark to check/search for</param>
        /// <param name="endMark">EndMark to search for</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception if string cannot be found].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// data
        /// or
        /// startMark
        /// or
        /// endMark
        /// </exception>
        /// <exception cref="ArgumentException">
        /// StartMark not found!
        /// or
        /// EndMark not found!
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!</exception>
        public static string GetString(this string data, int start, string startMark, string endMark, bool throwException = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (string.IsNullOrEmpty(startMark))
            {
                throw new ArgumentNullException("startMark");
            }

            if (string.IsNullOrEmpty(endMark))
            {
                throw new ArgumentNullException("endMark");
            }

            if (start < 0)
            {
                start = data.IndexOf(startMark);
            }

            if (start < 0)
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentException("StartMark not found!");
            }
            if (!data.Substring(start).StartsWith(startMark))
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentOutOfRangeException(nameof(startMark), "StartMark does not match!");
            }
            start += startMark.Length;
            int end = data.IndexOf(endMark, start + 1);
            if (end <= start)
            {
                if (!throwException)
                {
                    return null;
                }

                throw new ArgumentException("EndMark not found!");
            }
            return data.Substring(start, end - start);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this double value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this float value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this int value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this uint value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this long value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value</returns>
        public static string ToHexString(this ulong value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a byte array to a hexadecimal string.</summary>
        /// <param name="data">The data.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public static string ToHexString(this byte[] data, bool upperCase = false)
        {
            return ToHexString(data, BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a byte array to a hexadecimal string.</summary>
        /// <param name="data">The data.</param>
        /// <param name="isLittleEndian">Defines whether the specified data has little endian byte order or not.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public static string ToHexString(this byte[] data, bool isLittleEndian, bool upperCase = false)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (isLittleEndian)
            {
                Array.Reverse(data);
            }

            StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
            string format = upperCase ? "X2" : "x2";
            for(int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString(format));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a hex string to a byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ParseHexString(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException("hex");
            }

            try
            {
                byte[] data = new byte[hex.Length / 2];
                for (int i = 0; i < hex.Length; i += 2)
                {
                    data[i >> 1] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                return data;
            }
            catch { throw new ArgumentException(string.Format("Invalid hex string {0}", hex)); }
        }

        /// <summary>
        /// A fast pattern replacement function for large strings
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceCaseInsensitiveInvariant(this string text, string pattern, string replacement)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            if (replacement == null)
            {
                throw new ArgumentNullException("replacement");
            }

            string result = text.ToUpperInvariant();
            pattern = pattern.ToUpperInvariant();
            //get the maximum change
            int maxChange = 0;
            if (pattern.Length < replacement.Length)
            {
                maxChange = (text.Length / pattern.Length) * (replacement.Length - pattern.Length);
            }
            char[] chars = new char[text.Length + maxChange];

            int count = 0;
            int start = 0;
            int index = result.IndexOf(pattern);
            while (index != -1)
            {
                for (int i = start; i < index; i++)
                {
                    chars[count++] = text[i];
                }

                for (int i = 0; i < replacement.Length; i++)
                {
                    chars[count++] = replacement[i];
                }

                start = index + pattern.Length;
                index = result.IndexOf(pattern, start);
            }
            if (start == 0)
            {
                return text;
            }

            for (int i = start; i < text.Length; i++)
            {
                chars[count++] = text[i];
            }

            return new string(chars, 0, count);
        }

        /// <summary>
        /// Obtains whether the specified string contains invalid chars or not
        /// </summary>
        public static bool HasInvalidChars(this string source, string validChars)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return !string.IsNullOrEmpty(source);
            }

            foreach (char c in source)
            {
                if (validChars.IndexOf(c) < 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves only validated chars from a string
        /// </summary>
        public static string GetValidChars(this string source, string validChars)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return "";
            }

            StringBuilder result = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (validChars.IndexOf(c) > -1)
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Obtains the index of the first invalid char or -1 if all chars are valid.
        /// </summary>
        /// <param name="validChars"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int IndexOfInvalidChar(this string source, string validChars)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return 0;
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (validChars.IndexOf(source[i]) < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Obtains the index of the first invalid char or -1 if all chars are valid.
        /// </summary>
        /// <param name="validChars"></param>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int IndexOfInvalidChar(this string source, string validChars, int start)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return 0;
            }

            for (int i = start; i < source.Length; i++)
            {
                if (validChars.IndexOf(source[i]) < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Retrieves all specified chars with a string.
        /// </summary>
        public static string ReplaceChars(this string source, char[] chars, string replacer)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }

            if (chars == null)
            {
                return source;
            }

            if (replacer == null)
            {
                replacer = "";
            }

            StringBuilder result = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (Array.IndexOf(chars, c) > -1)
                {
                    result.Append(replacer);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Retrieves only validated chars from a string and replaces all other occurances
        /// </summary>
        public static string ReplaceChars(this string source, string chars, string replacer)
        {
            if (string.IsNullOrEmpty(source))
            {
                return "";
            }

            if (chars == null)
            {
                return source;
            }

            if (replacer == null)
            {
                replacer = "";
            }

            StringBuilder sb = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (chars.IndexOf(c) > -1)
                {
                    sb.Append(replacer);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves only validated chars from a string and replaces all other occurances
        /// </summary>
        public static string ReplaceInvalidChars(this string source, char[] validChars, string replacer)
        {
            if ((validChars == null) || (validChars.Length == 0))
            {
                return "";
            }

            if (string.IsNullOrEmpty(source))
            {
                return "";
            }

            if (replacer == null)
            {
                replacer = "";
            }

            StringBuilder sb = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (Array.IndexOf(validChars, c) > -1)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(replacer);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves only validated chars from a string and replaces all other occurances
        /// </summary>
        public static string ReplaceInvalidChars(this string source, string validChars, string replacer)
        {
            if (string.IsNullOrEmpty(validChars))
            {
                return "";
            }

            if (string.IsNullOrEmpty(source))
            {
                return "";
            }

            if (replacer == null)
            {
                replacer = "";
            }

            StringBuilder sb = new StringBuilder(source.Length);
            foreach (char c in source)
            {
                if (validChars.IndexOf(c) > -1)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(replacer);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Splits a string at the specified separators and allows to keep the separators in the list
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static string[] SplitKeepSeparators(this string text, params char[] separators)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }

            List<string> result = new List<string>();
            int last = 0;
            int next = text.IndexOfAny(separators, 1);
            while (next > -1)
            {
                int len = next - last;
                if (len > 0)
                {
                    string part = text.Substring(last, len);
                    result.Add(part);
                }
                result.Add(text[next].ToString());
                last = next + 1;
                next = text.IndexOfAny(separators, last);
            }
            if (last < text.Length)
            {
                result.Add(text.Substring(last));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Splits a string at platform independent newline markings (CR, LF, CRLF, #0)
        /// </summary>
        public static string[] SplitNewLine(this string text, StringSplitOptions textSplitOptions)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            List<string> result = new List<string>();
            int start = 0;
            int indexCR = -1;
            int indexNL = -1;
            int indexNull = -1;

            while (start < text.Length)
            {
                if (indexCR < int.MaxValue)
                {
                    indexCR = text.IndexOf('\r', start);
                    if (indexCR == -1)
                    {
                        indexCR = int.MaxValue;
                    }
                }
                if (indexNL < int.MaxValue)
                {
                    indexNL = text.IndexOf('\n', start);
                    if (indexNL == -1)
                    {
                        indexNL = int.MaxValue;
                    }
                }
                if (indexNull < int.MaxValue)
                {
                    indexNull = text.IndexOf('\0', start);
                    if (indexNull == -1)
                    {
                        indexNull = int.MaxValue;
                    }
                }

                if (indexNull < indexCR)
                {
                    if (indexNull < indexNL)
                    {
                        //0<NL|CR
                        result.Add(text.Substring(start, indexNull - start));
                        start = indexNull + 1;
                        continue;
                    }
                    //NL<0<CR
                    result.Add(text.Substring(start, indexNL - start));
                    start = indexNL + 1;
                    continue;
                }
                //CRLF ?
                if (indexCR == indexNL - 1)
                {
                    //CRLF
                    result.Add(text.Substring(start, indexCR - start));
                    start = indexCR + 2;
                    continue;
                }
                //CR<NL
                if (indexCR < indexNL)
                {
                    result.Add(text.Substring(start, indexCR - start));
                    start = indexCR + 1;
                    continue;
                }
                //NL?
                if (indexNL < int.MaxValue)
                {
                    result.Add(text.Substring(start, indexNL - start));
                    start = indexNL + 1;
                    continue;
                }
                break;
            }
            if (start < text.Length)
            {
                result.Add(text.Substring(start));
            }
            if (textSplitOptions == StringSplitOptions.RemoveEmptyEntries)
            {
                result.RemoveAll(s => string.IsNullOrEmpty(s));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Splits a string at platform independent newline markings (CR, LF, CRLF, #0).
        /// Empty entries will be kept. (This equals <see cref="SplitNewLine(string, StringSplitOptions)"/> with <see cref="StringSplitOptions.None"/>)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] SplitNewLine(this string text)
        {
            return SplitNewLine(text, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits a string at newline markings and after a specified length.
        /// Trys to split only at space and newline, but will split anywhere else if its not possible.
        /// </summary>
        public static string[] SplitNewLineAndLength(this string text, int maxLength)
        {
            List<string> array = new List<string>();
            foreach (string str in SplitNewLine(text))
            {
                if (str.Length < maxLength)
                {
                    array.Add(str);
                    continue;
                }
                string currentText = "";
                string[] parts = str.Split(' ', '\t');
                for (int i = 0; i < parts.Length; i++)
                {
                    string textPart = parts[i];
                    if (currentText.Length + textPart.Length <= maxLength)
                    {
                        //textpart fits into this line
                        currentText += textPart;
                    }
                    else if (textPart.Length > maxLength)
                    {
                        //textpart does not fit into this line and does not fit in an empty line
                        int partLength = maxLength - currentText.Length;
                        currentText += textPart.Substring(0, partLength);
                        array.Add(currentText);
                        currentText = textPart.Substring(partLength);
                        while (currentText.Length > maxLength)
                        {
                            array.Add(currentText.Substring(0, maxLength));
                            currentText = currentText.Substring(maxLength);
                        }
                    }
                    else
                    {
                        //textpart fits in a line if its alone
                        array.Add(currentText);
                        currentText = textPart;
                    }
                    if (currentText.Length < maxLength)
                    {
                        if (i + 1 < parts.Length)
                        {
                            currentText += " ";
                        }
                    }
                    if (currentText.Length >= maxLength)
                    {
                        array.Add(currentText);
                        currentText = "";
                    }
                }
                if (!string.IsNullOrEmpty(currentText))
                {
                    array.Add(currentText);
                }
            }
            return array.ToArray();
        }

        /// <summary>Splits a string at character casing.</summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string[] SplitCamelCase(this string text)
        {
            List<int> splits = new List<int>();
            bool isUpper = true;
            for (int current = 1; current < text.Length; current++)
            {
                bool lastWasUpper = isUpper;
                isUpper = char.IsUpper(text[current]);
                //is upper do nothing
                if (isUpper)
                {
                    continue;
                }
                //is not upper and last was upper, split before last
                if (lastWasUpper)
                {
                    if (current > 1)
                    {
                        splits.Add(current - 1);
                    }
                }
            }
            return SplitAt(text, splits);
        }

        /// <summary>Splits a string at the specified indices.</summary>
        /// <param name="text">The text.</param>
        /// <param name="indices">The indices.</param>
        /// <returns></returns>
        public static string[] SplitAt(this string text, IEnumerable<int> indices)
        {
            List<string> items = new List<string>();
            int start = 0;
            foreach (int i in indices)
            {
                items.Add(text.Substring(start, i - start));
                start = i;
            }
            if (start < text.Length)
            {
                items.Add(text.Substring(start));
            }

            return items.ToArray();
        }

        /// <summary>Splits a string at the specified indices.</summary>
        /// <param name="text">The text.</param>
        /// <param name="indices">The indices.</param>
        /// <returns></returns>
        public static string[] SplitAt(this string text, params int[] indices)
        {
            return SplitAt(text, (IEnumerable<int>)indices);
        }

        /// <summary>Replaces the specified part of a string by splitting, replacing and joining.</summary>
        /// <param name="text">The full text.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="index">The index of the part to replace.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public static string ReplacePart(this string text, char separator, int index, string newValue)
        {
            string[] parts = text.Split(separator);
            parts[index] = newValue;
            return string.Join(separator.ToString(), parts);
        }

        /// <summary>
        /// Replaces newline markings
        /// </summary>
        public static string ReplaceNewLine(this string text, string newLine)
        {
            string[] strings = SplitNewLine(text);
            return string.Join(newLine, strings);
        }

        /// <summary>
        /// Removes any newline markings
        /// </summary>
        public static string RemoveNewLine(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            StringBuilder result = new StringBuilder(text.Length);
            char[] newLineChars = new char[] { '\r', '\n' };
            int pos = 0;
            int index = text.IndexOfAny(newLineChars);
            while (index > -1)
            {
                int size = index - pos;
                if (size > 0)
                {
                    result.Append(text.Substring(pos, size));
                }
                pos = index + 1;
                index = text.IndexOfAny(newLineChars, pos);
            }
            {
                int size = text.Length - pos;
                if (size > 0)
                {
                    result.Append(text.Substring(pos, size));
                }
            }
            return result.ToString();
        }

        /// <summary>Forces the maximum length.</summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static string ForceMaxLength(this string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength);
            }

            return text;
        }

        /// <summary>Forces the maximum length.</summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="endReplacer">The end replacer. (String appended to the end when cutting the text. Sample: "..")</param>
        /// <returns></returns>
        public static string ForceMaxLength(this string text, int maxLength, string endReplacer)
        {
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength - endReplacer.Length) + endReplacer;
            }
            return text;
        }

        /// <summary>
        /// Enforces a specific string length (appends spaces and cuts to length)
        /// </summary>
        public static string ForceLength(this string text, int count)
        {
            return ForceLength(text, count, "", " ");
        }

        /// <summary>
        /// Enforces a specific string length
        /// </summary>
        public static string ForceLength(this string text, int count, string prefix, string suffix)
        {
            while (text.Length < count)
            {
                if (prefix != null)
                {
                    text = prefix + text;
                    if (text.Length == count)
                    {
                        break;
                    }
                }
                if (suffix != null)
                {
                    text = text + suffix;
                }
            }
            if (text.Length > count)
            {
                text = text.Substring(0, count);
            }

            return text;
        }

        /// <summary>
        /// Tries to detect the used newline chars in the specified string
        /// </summary>
        public static string DetectNewLine(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.IndexOf("\r\n") > -1)
            {
                return "\r\n";
            }

            if (text.IndexOf('\n') > -1)
            {
                return "\n";
            }

            if (text.IndexOf('\r') > -1)
            {
                return "\r";
            }

            return null;
        }

        /// <summary>Boxes the specified text with the given character.</summary>
        /// <param name="text">The text.</param>
        /// <param name="c">The character to pre and append.</param>
        /// <returns></returns>
        public static string Box(this string text, char c)
        {
            return c + text + c;
        }

        /// <summary>Boxes the specified text with the given string.</summary>
        /// <param name="text">The text.</param>
        /// <param name="s">The string to pre and append.</param>
        /// <returns></returns>
        public static string Box(this string text, string s)
        {
            return s + text + s;
        }

        /// <summary>Boxes the specified text with the given string.</summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        public static string Box(this string text, string start, string end)
        {
            return start + text + end;
        }

        /// <summary>Escapes the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Escape(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\':
                    case '"': sb.Append('\\'); sb.Append(c); continue;
                    case '\b': sb.Append("\\b"); continue;
                    case '\t': sb.Append("\\t"); continue;
                    case '\n': sb.Append("\\n"); continue;
                    case '\f': sb.Append("\\f"); continue;
                    case '\r': sb.Append("\\r"); continue;
                }
                if (c < ' ')
                {
                    sb.Append("\\u");
                    sb.Append(((int)c).ToString("x4"));
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>Unescapes the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static string Unescape(this string text)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i++];
                if (c == '\\')
                {
                    char c2 = text[i++];
                    switch (c2)
                    {
                        case '"': sb.Append('"'); continue;
                        case '\\': sb.Append('\\'); continue;
                        case 'b': sb.Append('\b'); continue;
                        case 't': sb.Append('\t'); continue;
                        case 'n': sb.Append('\n'); continue;
                        case 'f': sb.Append('\f'); continue;
                        case 'r': sb.Append('\r'); continue;
                        case 'u': sb.Append((char)int.Parse(text.Substring(i, 4))); i += 4; continue;
                        default: throw new InvalidDataException();
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Unboxes a string (removes strings from start end end)
        /// </summary>
        /// <param name="text">The string to be unboxed</param>
        /// <param name="start">Start of box</param>
        /// <param name="end">End of box</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error</param>
        /// <returns></returns>
        public static string Unbox(this string text, string start, string end, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (start == null)
            {
                throw new ArgumentNullException("start");
            }

            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            if (text.Length > start.Length && text.StartsWith(start) && text.EndsWith(end))
            {
                return text.Substring(start.Length, text.Length - start.Length - end.Length);
            }

            if (throwEx)
            {
                throw new FormatException(string.Format("Could not unbox {0} string {1}!", start, end));
            }

            return text;
        }

        /// <summary>Unboxes a string</summary>
        /// <param name="text">The string to be unboxed</param>
        /// <param name="border">The border.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        /// <exception cref="FormatException"></exception>
        public static string Unbox(this string text, string border, bool throwEx = true)
        {
            if (border == null)
            {
                return text;
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > border.Length && text.StartsWith(border) && text.EndsWith(border))
            {
                return text.Substring(border.Length, text.Length - border.Length - border.Length);
            }

            if (throwEx)
            {
                throw new FormatException(string.Format("Could not unbox {0} string {0}!", border));
            }

            return text;
        }

        /// <summary>Unboxes a string</summary>
        /// <param name="text">The string to be unboxed</param>
        /// <param name="border">The border.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        /// <exception cref="FormatException"></exception>
        public static string Unbox(this string text, char border, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 1 && text[0] == border && text[text.Length - 1] == border)
            {
                return text.Substring(1, text.Length - 2);
            }
            if (throwEx)
            {
                throw new FormatException(string.Format("Could not unbox {0} string {0}!", border));
            }

            return text;
        }

        /// <summary>
        /// Unboxes a string (removes enclosing "" and '')
        /// </summary>
        /// <param name="text">The string to be unboxed</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error</param>
        public static string UnboxText(this string text, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 1)
            {
                if (text.StartsWith("'") && text.EndsWith("'"))
                {
                    return text.Substring(1, text.Length - 2);
                }

                if (text.StartsWith("\"") && text.EndsWith("\""))
                {
                    return text.Substring(1, text.Length - 2);
                }
            }
            if (throwEx)
            {
                throw new FormatException(string.Format("Could not unbox {0} string {1}'!", '"', '"'));
            }

            return text;
        }

        /// <summary>
        /// Unboxes a string (removes enclosing [], {} and ())
        /// </summary>
        /// <param name="text">The string to be unboxed</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error</param>
        public static string UnboxBrackets(this string text, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length > 1)
            {
                if (text.StartsWith("(") && text.EndsWith(")"))
                {
                    return text.Substring(1, text.Length - 2);
                }

                if (text.StartsWith("[") && text.EndsWith("]"))
                {
                    return text.Substring(1, text.Length - 2);
                }

                if (text.StartsWith("{") && text.EndsWith("}"))
                {
                    return text.Substring(1, text.Length - 2);
                }
            }
            if (throwEx)
            {
                throw new FormatException(string.Format("Could not unbox {0} string {1}!", '"', '"'));
            }

            return text;
        }

        /// <summary>Parses a binary size string created by <see cref="FormatSize(double)"/> or <see cref="FormatBinarySize(double)"/>.</summary>
        /// <param name="value">The value string.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double ParseBinarySize(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string[] parts = value.Split(' ');
            bool error = (parts.Length != 2);
            error = error & double.TryParse(parts[0], out double size);
            if (!error)
            {
                if (parts[1] == "B")
                {
                    return size;
                }

                foreach (SI_Units u in Enum.GetValues(typeof(SI_Units)))
                {
                    if (parts[1] == u.ToString() + "B")
                    {
                        return size * Math.Pow(1000, (int)u);
                    }
                }
                foreach (IEC_Units u in Enum.GetValues(typeof(IEC_Units)))
                {
                    if (parts[1] == u.ToString() + "B")
                    {
                        return size * Math.Pow(1024, (int)u);
                    }
                }
            }
            throw new ArgumentException(string.Format("Invalid format in binary size. Expected '<value> <unit>'. Example '15 MB'. Got '{0}'.", value));
        }

        /// <summary>Randomizes the character casing</summary>
        /// <param name="value">The string.</param>
        /// <returns>Returns a new string with random case</returns>
        public static string RandomCase(this string value)
        {
            Random rnd = new Random(Environment.TickCount);
            char[] result = new char[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if ((rnd.Next() % 1) == 0)
                {
                    result[i] = char.ToUpper(value[i]);
                }
                else
                {
                    result[i] = char.ToLower(value[i]);
                }
            }
            return new string(result);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The character to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterFirst(this string value, char character)
        {
            int i = value.IndexOf(character);
            if (i < 0)
            {
                return "";
            }

            return value.Substring(i + 1);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The character to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterFirst(this string value, string pattern)
        {
            int i = value.IndexOf(pattern);
            if (i < 0)
            {
                return "";
            }

            return value.Substring(i + pattern.Length);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The pattern to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeFirst(this string value, char character)
        {
            int i = value.IndexOf(character);
            if (i < 0)
            {
                return value;
            }

            return value.Substring(0, i);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The character to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeFirst(this string value, string pattern)
        {
            int i = value.IndexOf(pattern);
            if (i < 0)
            {
                return value;
            }

            return value.Substring(0, i);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The pattern to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterLast(this string value, char character)
        {
            int i = value.LastIndexOf(character);
            if (i < 0)
            {
                return "";
            }

            return value.Substring(i + 1);
        }
        
        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterLast(this string value, string pattern)
        {
            int i = value.LastIndexOf(pattern);
            if (i < 0)
            {
                return "";
            }

            return value.Substring(i + pattern.Length);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The character to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeLast(this string value, char character)
        {
            int i = value.LastIndexOf(character);
            if (i < 0)
            {
                return value;
            }

            return value.Substring(0, i);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeLast(this string value, string pattern)
        {
            int i = value.LastIndexOf(pattern);
            if (i < 0)
            {
                return value;
            }

            return value.Substring(0, i);
        }

        /// <summary>Converts a string to a bool.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static bool ToBool(this string value, bool defaultValue = false)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>Converts a string to an integer.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static int ToInt32(this string value, int defaultValue = 0)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>Converts a string to an integer.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static uint ToUInt32(this string value, uint defaultValue = 0)
        {
            if (uint.TryParse(value, out uint result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>Converts a string to an integer.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static long ToInt64(this string value, long defaultValue = 0)
        {
            if (long.TryParse(value, out long result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>Converts a string to an integer.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static ulong ToInt64(this string value, ulong defaultValue = 0)
        {
            if (ulong.TryParse(value, out ulong result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Checks whether a specified text is enclosed by some markers.
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <param name="start">The start marker</param>
        /// <param name="end">The end marker</param>
        public static bool IsBoxed(this string text, char start, char end)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return (text[0] == start) && (text[text.Length - 1] == end);
        }

        /// <summary>
        /// Checks whether a specified text is enclosed by some markers.
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <param name="start">The start marker</param>
        /// <param name="end">The end marker</param>
        public static bool IsBoxed(this string text, string start, string end)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            return (text.StartsWith(start) && text.EndsWith(end));
        }
	}
}
