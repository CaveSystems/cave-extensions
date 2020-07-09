#pragma warning disable CA1307

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Provides string functions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Provides the default date time string used when formatting date time variables for interop.
        /// </summary>
        public const string InterOpDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK ";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for display.
        /// </summary>
        public const string DisplayDateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for display.
        /// </summary>
        public const string DisplayDateTimeWithTimeZoneFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff K";

        /// <summary>
        /// Provides the default date string used when formatting date time variables for interop.
        /// </summary>
        public const string ShortDateFormat = "yyyy'-'MM'-'dd";

        /// <summary>
        /// Provides the default time string used when formatting date time variables for interop.
        /// </summary>
        public const string ShortTimeFormat = "HH':'mm':'ss'.'fff";

        /// <summary>
        /// Provides the default date time string used when formatting date time variables for file names.
        /// </summary>
        public const string FileNameDateTimeFormat = "yyyy'-'MM'-'dd' 'HHmmss";

        /// <summary>
        /// Joins a collection to a string with newlines for all systems.
        /// </summary>
        /// <param name="texts">The string collection.</param>
        /// <returns>Returns a new string.</returns>
        public static string JoinNewLine(this string[] texts)
        {
            return Join(texts, "\r\n");
        }

        /// <summary>
        /// Joins a collection to a string with newlines for all systems.
        /// </summary>
        /// <param name="array">The string array.</param>
        /// <returns>Returns a new string.</returns>
        public static string JoinNewLine(this IEnumerable array)
        {
            return Join(array, "\r\n");
        }

        /// <summary>
        /// Joins a collection to a string.
        /// </summary>
        /// <param name="array">The string array.</param>
        /// <param name="separator">The seperator.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>Returns a new string.</returns>
        public static string Join(this IEnumerable array, string separator, IFormatProvider cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (separator == null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            var result = new StringBuilder();
            foreach (var obj in array)
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
        /// Joins a collection to a string.
        /// </summary>
        /// <param name="array">The string array.</param>
        /// <param name="separator">The seperator.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>Returns a new string.</returns>
        public static string Join(this IEnumerable array, char separator, IFormatProvider cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var result = new StringBuilder();
            foreach (var obj in array)
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
        /// <param name="culture">The culture info.</param>
        /// <returns>The joned string.</returns>
        public static string JoinCamelCase(this string[] parts, CultureInfo culture = null)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            var result = new StringBuilder();
            foreach (var part in parts)
            {
                var t = part.Trim();
                if (t.Length < 1)
                {
                    continue;
                }

                result.Append(char.ToUpper(t[0], culture));
                if (t.Length > 1)
                {
                    result.Append(t.Substring(1).ToLower(culture));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts a exception to a simple text message.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/>.</param>
        /// <param name="debug">Include debug information (stacktrace, data).</param>
        /// <returns>The text.</returns>
        public static string ToText(this Exception ex, bool debug = false)
        {
            return string.Join(Environment.NewLine, ToStrings(ex, debug));
        }

        /// <summary>
        /// Converts a exception to a string array.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/>.</param>
        /// <param name="debug">Include debug information (stacktrace, data).</param>
        /// <returns>The string array.</returns>
        public static string[] ToStrings(this Exception ex, bool debug = false)
        {
            // ignore AggregateException
            if (ex is AggregateException)
            {
                return ToStrings(ex.InnerException, debug);
            }

            if (ex == null)
            {
                return new string[0];
            }

            var strings = new List<string>();

            if (debug)
            {
                strings.Add("Message:");
            }

            foreach (var s in SplitNewLine(ex.Message))
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
                    foreach (var s in SplitNewLine(ex.Source))
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
                    foreach (var key in ex.Data.Keys)
                    {
                        strings.Add($"  {key}: {ex.Data[key]}");
                    }
                }

                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    strings.Add("StackTrace:");
                    foreach (var s in SplitNewLine(ex.StackTrace))
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

            if (ex is ReflectionTypeLoadException reflectionTypeLoadException)
            {
                foreach (Exception inner in reflectionTypeLoadException.LoaderExceptions)
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
        /// <param name="text">The format string.</param>
        /// <param name="args">The parameters.</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this string text, params object[] args)
        {
            if (args == null)
            {
                args = new object[0];
            }

            var result = text ?? throw new ArgumentNullException(nameof(text));
            for (var i = 0; i < args.Length; i++)
            {
                var argument = (args[i] == null) ? "<null>" : args[i].ToString();
                result = result.Replace("{" + i + "}", argument);
            }
            return result;
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="timeSpan">TimeSpan to format.</param>
        /// <param name="culture">Culture used to format the double value.</param>
        /// <returns>Returns a string like: 10.23ns, 1.345ms, 102.3s, 10.2h, ...</returns>
        public static string FormatTime(this TimeSpan timeSpan, IFormatProvider culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

            if (timeSpan < TimeSpan.Zero)
            {
                return "-" + FormatTime(-timeSpan, culture);
            }

            if (timeSpan == TimeSpan.Zero)
            {
                return "0s";
            }

            if (timeSpan.Ticks < TimeSpan.TicksPerMillisecond)
            {
                var nano = timeSpan.Ticks / (double)(TimeSpan.TicksPerMillisecond / 1000);
                return (nano > 9.99) ? nano.ToString("0.0", culture) + "ns" : nano.ToString("0.00", culture) + "ns";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerSecond)
            {
                var msec = timeSpan.TotalMilliseconds;
                return (msec > 9.99) ? msec.ToString("0.0", culture) + "ms" : msec.ToString("0.00", culture) + "ms";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerMinute)
            {
                var sec = timeSpan.TotalSeconds;
                return (sec > 9.99) ? sec.ToString("0.0", culture) + "s" : sec.ToString("0.00", culture) + "s";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerHour)
            {
                var min = timeSpan.TotalMinutes;
                return (min > 9.99) ? min.ToString("0.0", culture) + "min" : min.ToString("0.00", culture) + "min";
            }
            if (timeSpan.Ticks < TimeSpan.TicksPerDay)
            {
                var h = timeSpan.TotalHours;
                return (h > 9.99) ? h.ToString("0.0", culture) + "h" : h.ToString("0.00", culture) + "h";
            }
            var d = timeSpan.TotalDays;
            if (d >= 36525)
            {
                return (d / 365.25).ToString("0", culture) + "a";
            }

            if (d >= 3652.5)
            {
                return (d / 365.25).ToString("0.0", culture) + "a";
            }

            if (d > 99.9)
            {
                return (d / 365.25).ToString("0.00", culture) + "a";
            }

            if (d > 9.99)
            {
                return d.ToString("0.0", culture) + "d";
            }

            return d.ToString("0.00", culture) + "d";
        }

        /// <summary>
        /// Formats a time span to a short one unit value (1.20h, 15.3ms, ...)
        /// </summary>
        /// <param name="seconds">Seconds to format.</param>
        /// <param name="culture">Culture used to format the double value.</param>
        /// <returns>Returns a string like: 10.23ns, 1.345ms, 102.3s, 10.2h, ...</returns>
        public static string FormatTime(this double seconds, IFormatProvider culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }

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
            var part = seconds;
            for (SiFraction fraction = SiFraction.m; fraction <= SiFraction.y; fraction++)
            {
                part *= 1000.0;
                if (part > 9.99)
                {
                    return part.ToString("0.0", culture) + fraction + "s";
                }

                if (part > 0.999)
                {
                    return part.ToString("0.00", culture) + fraction + "s";
                }
            }
            return seconds.ToString() + "s";
        }

        /// <summary>Formats the specified timespan to [HH:]MM:SS.F.</summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="millisecondDigits">The number of millisecond digits.</param>
        /// <returns>The formatted string.</returns>
        /// <exception cref="NotSupportedException">Only 0-3 millisecond digits are supported!.</exception>
        public static string FormatTimeSpan(this TimeSpan timeSpan, int millisecondDigits)
        {
            var result = new StringBuilder();
            if (timeSpan.Hours > 0)
            {
                result.Append((timeSpan.Ticks / TimeSpan.TicksPerHour).ToString("00"));
                result.Append(":");
            }
            result.Append(timeSpan.Minutes.ToString("00"));
            result.Append(":");
            var seconds = timeSpan.Seconds;

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
                    result.Append(timeSpan.Milliseconds.ToString("000"));
                    break;
                default:
                    throw new NotSupportedException("Only 0-3 millisecond digits are supported!");
            }
            return result.ToString();
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatSize(this float size, IFormatProvider culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            if (size < 0)
            {
                return "-" + FormatSize(-size);
            }
            var calc = size;
            SiUnit unit = 0;
            while (calc >= 1000)
            {
                calc /= 1000;
                unit++;
            }
            string result;
            if (Math.Truncate(calc) == calc)
            {
                result = calc.ToString(culture);
            }
            else
            {
                result = calc.ToString("0.000", culture);
            }
            if (result.Length > 5)
            {
                result = result.Substring(0, 5);
            }

            return result + ((unit == 0) ? string.Empty : " " + unit.ToString());
        }

        /// <summary>
        /// Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatSize(this ulong size, IFormatProvider culture = null)
        {
            return FormatSize((float)size, culture);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatSize(this long size, IFormatProvider culture = null)
        {
            return FormatSize((float)size, culture);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatSize(this decimal size, IFormatProvider culture = null)
        {
            return FormatSize((float)size, culture);
        }

        /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatSize(this double size, IFormatProvider culture = null)
        {
            return FormatSize((float)size, culture);
        }

        /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
        /// <param name="size">The size.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
        public static string FormatBinarySize(this float size, IFormatProvider culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.InvariantCulture;
            }
            var negative = size < 0;
            IecUnit unit = 0;
            while (size >= 1024)
            {
                size /= 1024;
                unit++;
            }
            var result = size.ToString("0.000", culture);
            if (result.Length > 5)
            {
                result = result.Substring(0, 5);
            }

            return (negative ? "-" : string.Empty) + result + " " + unit.ToString();
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatBinarySize(this double value, IFormatProvider culture = null)
        {
            return FormatBinarySize((float)value, culture);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatBinarySize(this decimal value, IFormatProvider culture = null)
        {
            return FormatBinarySize((float)value, culture);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatBinarySize(this ulong value, IFormatProvider culture = null)
        {
            return FormatBinarySize((float)value, culture);
        }

        /// <summary>
        /// Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatBinarySize(this long value, IFormatProvider culture = null)
        {
            return FormatBinarySize((float)value, culture);
        }

        /// <summary>
        /// Converts a string to the specified target type using the <see cref="TypeExtension.ConvertValue(Type, object, IFormatProvider)"/> method.
        /// </summary>
        /// <typeparam name="T">Type to convert to.</typeparam>
        /// <param name="value">String value to convert.</param>
        /// <param name="culture">An object that supplies culture-specific formatting information.</param>
        /// <returns>Returns a new value instance.</returns>
        public static T ParseValue<T>(this string value, IFormatProvider culture = null)
        {
            return (T)TypeExtension.ConvertValue(typeof(T), value, culture);
        }

        /// <summary>
        /// Returns the objects.ToString() result or "&lt;null&gt;".
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="format">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string.</returns>
        public static string ToString(object value, IFormatProvider format = null)
        {
            if (value == null)
            {
                return "<null>";
            }

            if (format == null)
            {
                format = CultureInfo.InvariantCulture;
            }
            else if (format is CultureInfo culture && !(culture.Calendar is GregorianCalendar))
            {
                throw new NotSupportedException($"Calendar {culture.Calendar} not supported!");
            }

            // special handling for roundtrip types
            if (value is double d)
            {
                return d.ToString("R", format);
            }
            if (value is float f)
            {
                return f.ToString("R", format);
            }
            if (value is DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Unspecified)
                {
                    throw new ArgumentOutOfRangeException("Please specify DateTime.Kind!");
                }
                return dt.ToString(InterOpDateTimeFormat, format);
            }

            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, format);
            }
            if (value is ICollection collection)
            {
                return value.ToString() + " {" + Join(collection, ",", format) + "}";
            }
            return value.ToString();
        }

        /// <summary>
        /// Returns the objects.ToString() result or "&lt;null&gt;".
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>The string.</returns>
        public static string ToString(object value) => ToString(value, null);

        /// <summary>
        /// Returns an array of strings using the element objects ToString() method with invariant culture.
        /// </summary>
        /// <param name="enumerable">The array ob objects.</param>
        /// <returns>The string array.</returns>
        public static string[] ToStringArray(this IEnumerable enumerable)
        {
            return ToStringArray(enumerable, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an array of strings using the element objects ToString() method.
        /// </summary>
        /// <param name="enumerable">The array ob objects.</param>
        /// <param name="cultureInfo">The culture to use during formatting.</param>
        /// <returns>The string array.</returns>
        public static string[] ToStringArray(this IEnumerable enumerable, CultureInfo cultureInfo)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var result = new List<string>();
            foreach (var obj in enumerable)
            {
                result.Add(ToString(obj, cultureInfo));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Parses a DateTime (Supported formats: <see cref="InterOpDateTimeFormat"/>, <see cref="DisplayDateTimeFormat"/>, default).
        /// </summary>
        /// <param name="dateTime">String value to parse.</param>
        /// <returns>The parsed datetime.</returns>
        public static DateTime ParseDateTime(string dateTime)
        {
            {
                if (DateTime.TryParseExact(dateTime, InterOpDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                {
                    return result;
                }
            }
            {
                if (DateTime.TryParseExact(dateTime, DisplayDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime result))
                {
                    return result;
                }
            }
            return DateTime.Parse(dateTime);
        }

        /// <summary>
        /// Parses a DateTime (Supported formats: <see cref="InterOpDateTimeFormat"/>, <see cref="DisplayDateTimeFormat"/>, default).
        /// </summary>
        /// <param name="dateTime">String value to parse.</param>
        /// <param name="result">The parsed datetime.</param>
        /// <returns>True if the value could be parsed.</returns>
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
        /// Parses a Point.ToString() result.
        /// </summary>
        /// <param name="point">String value to parse.</param>
        /// <returns>The parsed point.</returns>
        public static Point ParsePoint(string point)
        {
            var data = Unbox(point.Trim(), "{", "}", true);
            var parts = data.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("X="))
            {
                throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y="))
            {
                throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
            }

            var x = int.Parse(parts[0].Trim().Substring(2));
            var y = int.Parse(parts[1].Trim().Substring(2));
            return new Point(x, y);
        }

        /// <summary>
        /// Parses a Size.ToString() result.
        /// </summary>
        /// <param name="size">String value to parse.</param>
        /// <returns>The parsed size.</returns>
        public static Size ParseSize(string size)
        {
            var data = Unbox(size.Trim(), "{", "}", true);
            var parts = data.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("WIDTH="))
            {
                throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("HEIGHT="))
            {
                throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
            }

            var w = int.Parse(parts[0].Trim().Substring(6));
            var h = int.Parse(parts[1].Trim().Substring(7));
            return new Size(w, h);
        }

        /// <summary>
        /// Parses a Rectangle.ToString() result.
        /// </summary>
        /// <param name="rect">String value to parse.</param>
        /// <returns>The parsed rectangle.</returns>
        public static Rectangle ParseRectangle(string rect)
        {
            var data = Unbox(rect, "{", "}", true);
            var parts = data.Split(',');
            if (parts.Length != 4)
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            if (!parts[0].Trim().ToUpperInvariant().StartsWith("X="))
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y="))
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            if (!parts[2].Trim().ToUpperInvariant().StartsWith("WIDTH="))
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            if (!parts[3].Trim().ToUpperInvariant().StartsWith("HEIGHT="))
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            var x = int.Parse(parts[0].Trim().Substring(2));
            var y = int.Parse(parts[1].Trim().Substring(2));
            var w = int.Parse(parts[2].Trim().Substring(6));
            var h = int.Parse(parts[3].Trim().Substring(7));
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Parses a PointF.ToString() result.
        /// </summary>
        /// <param name="point">String value to parse.</param>
        /// <returns>The parsed float point.</returns>
        public static PointF ParsePointF(string point)
        {
            var data = Unbox(point, "{", "}", true);
            var parts = data.ToUpperInvariant().Split(new string[] { "X=", "Y=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
            }

            var x = float.Parse(parts[0].Trim(' ', ','));
            var y = float.Parse(parts[1].Trim(' ', ','));
            return new PointF(x, y);
        }

        /// <summary>
        /// Parses a SizeF.ToString() result.
        /// </summary>
        /// <param name="size">String value to parse.</param>
        /// <returns>The parsed float size.</returns>
        public static SizeF ParseSizeF(string size)
        {
            var data = Unbox(size, "{", "}", true);
            var parts = data.ToUpperInvariant().Split(new string[] { "WIDTH=", "HEIGHT=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
            }

            var w = float.Parse(parts[0].Trim(' ', ','));
            var h = float.Parse(parts[1].Trim(' ', ','));
            return new SizeF(w, h);
        }

        /// <summary>
        /// Parses a RectangleF.ToString() result.
        /// </summary>
        /// <param name="rect">String value to parse.</param>
        /// <returns>The parsed float rectangle.</returns>
        public static RectangleF ParseRectangleF(string rect)
        {
            var data = Unbox(rect, "{", "}", true);
            var parts = data.ToUpperInvariant().Split(new string[] { "X=", "Y=", "WIDTH=", "HEIGHT=" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4)
            {
                throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
            }

            var x = float.Parse(parts[0].Trim(' ', ','));
            var y = float.Parse(parts[1].Trim(' ', ','));
            var w = float.Parse(parts[2].Trim(' ', ','));
            var h = float.Parse(parts[3].Trim(' ', ','));
            return new RectangleF(x, y, w, h);
        }

        /// <summary>Gets a substring from the end of the specified string.
        /// Positive values retrieve the number of characters from end.
        /// Negative values retrieve everything in front of the specified len - count.</summary>
        /// <param name="text">The string.</param>
        /// <param name="count">The number of characters at the end to be retrieved.</param>
        /// <returns>The substring.</returns>
        public static string SubstringEnd(this string text, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var len = text.Length;
            if (count > len || count == 0 || -count > len)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count needs to be in range -len..-1 or 1..len");
            }

            if (count > 0)
            {
                return text.Substring(len - count);
            }
            else
            {
                // if count < 0
                return text.Substring(0, len + count);
            }
        }

        /// <summary>Gets a part of a string.</summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark).</param>
        /// <param name="startMark">StartMark to check/search for.</param>
        /// <param name="endMark">EndMark to search for.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception if string cannot be found].</param>
        /// <returns>The substring.</returns>
        /// <exception cref="System.ArgumentNullException">data.</exception>
        /// <exception cref="ArgumentException">
        /// StartMark not found!
        /// or
        /// EndMark not found!.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!.</exception>
        public static string GetString(this string data, int start, char startMark, char endMark, bool throwException = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
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
            var end = data.IndexOf(endMark, start + 1);
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

        /// <summary>Gets a part of a string.</summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark).</param>
        /// <param name="startMark">StartMark to check/search for.</param>
        /// <param name="endMark">EndMark to search for.</param>
        /// <param name="throwException">if set to <c>true</c> [throw exception if string cannot be found].</param>
        /// <returns>The substring.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// data
        /// or
        /// startMark
        /// or
        /// endMark.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// StartMark not found!
        /// or
        /// EndMark not found!.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!.</exception>
        public static string GetString(this string data, int start, string startMark, string endMark, bool throwException = true)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(startMark))
            {
                throw new ArgumentNullException(nameof(startMark));
            }

            if (string.IsNullOrEmpty(endMark))
            {
                throw new ArgumentNullException(nameof(endMark));
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
            var end = data.IndexOf(endMark, start + 1);
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
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this double value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this float value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this int value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this uint value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this long value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a value to a hexadecimal string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>Returns the hexadecimal representation of the value.</returns>
        public static string ToHexString(this ulong value, bool upperCase = false)
        {
            return ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a byte array to a hexadecimal string.</summary>
        /// <param name="data">The data.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>The converted string.</returns>
        /// <exception cref="ArgumentNullException">data.</exception>
        public static string ToHexString(this byte[] data, bool upperCase = false)
        {
            return ToHexString(data, BitConverter.IsLittleEndian, upperCase);
        }

        /// <summary>Converts a byte array to a hexadecimal string.</summary>
        /// <param name="data">The data.</param>
        /// <param name="isLittleEndian">Defines whether the specified data has little endian byte order or not.</param>
        /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
        /// <returns>The converted string.</returns>
        /// <exception cref="ArgumentNullException">data.</exception>
        public static string ToHexString(this byte[] data, bool isLittleEndian, bool upperCase = false)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (isLittleEndian)
            {
                Array.Reverse(data);
            }

            var stringBuilder = new StringBuilder(data.Length * 2);
            var format = upperCase ? "X2" : "x2";
            for (var i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString(format, CultureInfo.InvariantCulture));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="hex">The string with hex values.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ParseHexString(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            try
            {
                var data = new byte[hex.Length / 2];
                for (var i = 0; i < hex.Length; i += 2)
                {
                    data[i >> 1] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
                return data;
            }
            catch
            {
                throw new ArgumentException($"Invalid hex string {hex}");
            }
        }

        /// <summary>
        /// A fast pattern replacement function for large strings.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="pattern">The pattern to find.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The replaced text.</returns>
        public static string ReplaceCaseInsensitiveInvariant(this string text, string pattern, string replacement)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (replacement == null)
            {
                throw new ArgumentNullException(nameof(replacement));
            }

            var result = text.ToUpperInvariant();
            pattern = pattern.ToUpperInvariant();

            // get the maximum change
            var maxChange = 0;
            if (pattern.Length < replacement.Length)
            {
                maxChange = text.Length / pattern.Length * (replacement.Length - pattern.Length);
            }
            var chars = new char[text.Length + maxChange];

            var count = 0;
            var start = 0;
            var index = result.IndexOf(pattern);
            while (index != -1)
            {
                for (var i = start; i < index; i++)
                {
                    chars[count++] = text[i];
                }

                for (var i = 0; i < replacement.Length; i++)
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

            for (var i = start; i < text.Length; i++)
            {
                chars[count++] = text[i];
            }

            return new string(chars, 0, count);
        }

        /// <summary>
        /// Gets whether the specified string contains invalid chars or not.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The string with the valid chars.</param>
        /// <returns>Returns true if the text contains invalid chars.</returns>
        public static bool HasInvalidChars(this string text, string validChars)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return !string.IsNullOrEmpty(text);
            }

            foreach (var c in text)
            {
                if (validChars.IndexOf(c) < 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves only validated chars from a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The string with the valid chars.</param>
        /// <returns>Returns a new string with valid chars.</returns>
        public static string GetValidChars(this string text, string validChars)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return string.Empty;
            }

            var result = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                if (validChars.IndexOf(c) > -1)
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Gets the index of the first invalid char or -1 if all chars are valid.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The string with the valid chars.</param>
        /// <returns>Returns the index or -1.</returns>
        public static int IndexOfInvalidChar(this string text, string validChars)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return 0;
            }

            for (var i = 0; i < text.Length; i++)
            {
                if (validChars.IndexOf(text[i]) < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the first invalid char or -1 if all chars are valid.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The string with the valid chars.</param>
        /// <param name="start">The start index.</param>
        /// <returns>Returns the index or -1.</returns>
        public static int IndexOfInvalidChar(this string text, string validChars, int start)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrEmpty(validChars))
            {
                return 0;
            }

            for (var i = start; i < text.Length; i++)
            {
                if (validChars.IndexOf(text[i]) < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Retrieves all specified chars with a string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="chars">The array of chars to retrieve.</param>
        /// <param name="replacer">The replacer string.</param>
        /// <returns>Returns a new string.</returns>
        public static string ReplaceChars(this string text, char[] chars, string replacer)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (chars == null)
            {
                return text;
            }

            if (replacer == null)
            {
                replacer = string.Empty;
            }

            var result = new StringBuilder(text.Length);
            foreach (var c in text)
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
        /// Retrieves only validated chars from a string and replaces all other occurances.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="chars">The array of chars to retrieve.</param>
        /// <param name="replacer">The replacer string.</param>
        /// <returns>Returns a new string.</returns>
        public static string ReplaceChars(this string text, string chars, string replacer)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (chars == null)
            {
                return text;
            }

            if (replacer == null)
            {
                replacer = string.Empty;
            }

            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
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
        /// Retrieves only validated chars from a string and replaces all other occurances.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The array of chars to retrieve.</param>
        /// <param name="replacer">The replacer string.</param>
        /// <returns>Returns only valid characters.</returns>
        public static string ReplaceInvalidChars(this string text, char[] validChars, string replacer)
        {
            if ((validChars == null) || (validChars.Length == 0))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (replacer == null)
            {
                replacer = string.Empty;
            }

            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
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
        /// Retrieves only validated chars from a string and replaces all other occurances.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="validChars">The array of chars to retrieve.</param>
        /// <param name="replacer">The replacer string.</param>
        /// <returns>Returns a string containing only valid characters.</returns>
        public static string ReplaceInvalidChars(this string text, string validChars, string replacer)
        {
            if (string.IsNullOrEmpty(validChars))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (replacer == null)
            {
                replacer = string.Empty;
            }

            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
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
        /// Splits a string at the specified separators and allows to keep the separators in the list.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="separators">The arrays of chars used to seperate the text.</param>
        /// <returns>The array of seperated strings.</returns>
        public static string[] SplitKeepSeparators(this string text, params char[] separators)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }

            var result = new List<string>();
            var last = 0;
            var next = text.IndexOfAny(separators, 1);
            while (next > -1)
            {
                var len = next - last;
                if (len > 0)
                {
                    var part = text.Substring(last, len);
                    result.Add(part);
                }
                result.Add($"{text[next]}");
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
        /// Splits a string at platform independent newline markings (CR, LF, CRLF, #0).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="textSplitOptions">The options.</param>
        /// <returns>Returns a new array of strings.</returns>
        public static string[] SplitNewLine(this string text, StringSplitOptions textSplitOptions)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var result = new List<string>();
            var start = 0;
            var indexCR = -1;
            var indexNL = -1;
            var indexNull = -1;

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
                        // 0<NL|CR
                        result.Add(text.Substring(start, indexNull - start));
                        start = indexNull + 1;
                        continue;
                    }

                    // NL<0<CR
                    result.Add(text.Substring(start, indexNL - start));
                    start = indexNL + 1;
                    continue;
                }

                // CRLF ?
                if (indexCR == indexNL - 1)
                {
                    // CRLF
                    result.Add(text.Substring(start, indexCR - start));
                    start = indexCR + 2;
                    continue;
                }

                // CR<NL
                if (indexCR < indexNL)
                {
                    result.Add(text.Substring(start, indexCR - start));
                    start = indexCR + 1;
                    continue;
                }

                // NL?
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
        /// Empty entries will be kept. (This equals <see cref="SplitNewLine(string, StringSplitOptions)"/> with <see cref="StringSplitOptions.None"/>).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The string array.</returns>
        public static string[] SplitNewLine(this string text)
        {
            return SplitNewLine(text, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits a string at newline markings and after a specified length.
        /// Trys to split only at space and newline, but will split anywhere else if its not possible.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">The maximum length of the new strings.</param>
        /// <returns>Returns a new array of strings.</returns>
        public static string[] SplitNewLineAndLength(this string text, int maxLength)
        {
            var array = new List<string>();
            foreach (var str in SplitNewLine(text))
            {
                if (str.Length < maxLength)
                {
                    array.Add(str);
                    continue;
                }
                var currentText = string.Empty;
                var parts = str.Split(' ', '\t');
                for (var i = 0; i < parts.Length; i++)
                {
                    var textPart = parts[i];
                    if (currentText.Length + textPart.Length <= maxLength)
                    {
                        // textpart fits into this line
                        currentText += textPart;
                    }
                    else if (textPart.Length > maxLength)
                    {
                        // textpart does not fit into this line and does not fit in an empty line
                        var partLength = maxLength - currentText.Length;
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
                        // textpart fits in a line if its alone
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
                        currentText = string.Empty;
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
        /// <returns>The string array.</returns>
        public static string[] SplitCamelCase(this string text)
        {
            var splits = new List<int>();
            var isUpper = true;
            for (var current = 1; current < text.Length; current++)
            {
                var lastWasUpper = isUpper;
                isUpper = char.IsUpper(text[current]);

                // is not upper and last was upper, split before last
                if (isUpper && !lastWasUpper)
                {
                    if (current > 1)
                    {
                        splits.Add(current);
                    }
                }
            }
            return SplitAt(text, splits);
        }

        /// <summary>Splits a string at the specified indices.</summary>
        /// <param name="text">The text.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>The string array.</returns>
        public static string[] SplitAt(this string text, IEnumerable<int> indices)
        {
            var items = new List<string>();
            var start = 0;
            foreach (var i in indices)
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
        /// <returns>The string array.</returns>
        public static string[] SplitAt(this string text, params int[] indices)
        {
            return SplitAt(text, (IEnumerable<int>)indices);
        }

        /// <summary>Replaces the specified part of a string by splitting, replacing and joining.</summary>
        /// <param name="text">The full text.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="index">The index of the part to replace.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The replaced string.</returns>
        public static string ReplacePart(this string text, char separator, int index, string newValue)
        {
            var parts = text.Split(separator);
            parts[index] = newValue;
            return string.Join($"{separator}", parts);
        }

        /// <summary>
        /// Replaces newline markings.
        /// </summary>
        /// <param name="text">the text.</param>
        /// <param name="newLine">The new newline markings.</param>
        /// <returns>Returns a new string.</returns>
        public static string ReplaceNewLine(this string text, string newLine)
        {
            var strings = SplitNewLine(text);
            return string.Join(newLine, strings);
        }

        /// <summary>
        /// Removes any newline markings.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns a string without any newline characters.</returns>
        public static string RemoveNewLine(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var result = new StringBuilder(text.Length);
            var newLineChars = new char[] { '\r', '\n' };
            var pos = 0;
            var index = text.IndexOfAny(newLineChars);
            while (index > -1)
            {
                var size = index - pos;
                if (size > 0)
                {
                    result.Append(text.Substring(pos, size));
                }
                pos = index + 1;
                index = text.IndexOfAny(newLineChars, pos);
            }
            {
                var size = text.Length - pos;
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
        /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
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
        /// <param name="endReplacer">The end replacer. (String appended to the end when cutting the text. Sample: "..").</param>
        /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
        public static string ForceMaxLength(this string text, int maxLength, string endReplacer)
        {
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength - endReplacer.Length) + endReplacer;
            }
            return text;
        }

        /// <summary>
        /// Enforces a specific string length (appends spaces and cuts to length).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
        public static string ForceLength(this string text, int maxLength)
        {
            return ForceLength(text, maxLength, string.Empty, " ");
        }

        /// <summary>
        /// Enforces a specific string length.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="prefix">The prefix to add.</param>
        /// <param name="suffix">The suffix to add.</param>
        /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
        public static string ForceLength(this string text, int maxLength, string prefix, string suffix)
        {
            while (text.Length < maxLength)
            {
                if (prefix != null)
                {
                    text = prefix + text;
                    if (text.Length == maxLength)
                    {
                        break;
                    }
                }
                if (suffix != null)
                {
                    text += suffix;
                }
            }
            if (text.Length > maxLength)
            {
                text = text.Substring(0, maxLength);
            }

            return text;
        }

        /// <summary>
        /// Tries to detect the used newline chars in the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Retruns the detected new line string (CR, LF, CRLF).</returns>
        public static string DetectNewLine(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
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
        /// <returns>Returns a string starting and ending with the specified character.</returns>
        public static string Box(this string text, char c)
        {
            return c + text + c;
        }

        /// <summary>Boxes the specified text with the given string.</summary>
        /// <param name="text">The text.</param>
        /// <param name="s">The string to pre and append.</param>
        /// <returns>Returns a string starting and ending with the specified string.</returns>
        public static string Box(this string text, string s)
        {
            return s + text + s;
        }

        /// <summary>Boxes the specified text with the given string.</summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Returns a string starting and ending with the specified string.</returns>
        public static string Box(this string text, string start, string end)
        {
            return start + text + end;
        }

        /// <summary>Escapes all characters at the specified string below ascii 32 and above ascii 127.</summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns an escaped ascii 7 bit string.</returns>
        public static string Escape(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var c in text)
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
                if (c < ' ' || c > (char)127)
                {
                    sb.Append("\\u");
                    sb.Append(((int)c).ToString("x4"));
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>Escapes all characters at the specified string below ascii 32.</summary>
        /// <param name="text">The text.</param>
        /// <returns>Returns an escaped utf8 string.</returns>
        public static string EscapeUtf8(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var c in text)
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
        /// <param name="text">The text (escaped ascii 7 bit string).</param>
        /// <returns>Returns the unescaped string.</returns>
        /// <exception cref="InvalidDataException">Invalid escape code.</exception>
        public static string Unescape(this string text)
        {
            var sb = new StringBuilder();
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i++];
                if (c == '\\')
                {
                    var c2 = text[i++];
                    switch (c2)
                    {
                        case '"': sb.Append('"'); continue;
                        case '\\': sb.Append('\\'); continue;
                        case 'b': sb.Append('\b'); continue;
                        case 't': sb.Append('\t'); continue;
                        case 'n': sb.Append('\n'); continue;
                        case 'f': sb.Append('\f'); continue;
                        case 'r': sb.Append('\r'); continue;
                        case 'u': sb.Append((char)Convert.ToInt32(text.Substring(i, 4), 16)); i += 4; continue;
                        default: throw new InvalidDataException("Invalid escape code.");
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Unboxes a string (removes strings from start end end).
        /// </summary>
        /// <param name="text">The string to be unboxed.</param>
        /// <param name="start">Start of box.</param>
        /// <param name="end">End of box.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
        /// <returns>Returns the content between the start and end marks.</returns>
        public static string Unbox(this string text, string start, string end, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            if (text.Length > start.Length && text.StartsWith(start) && text.EndsWith(end))
            {
                return text.Substring(start.Length, text.Length - start.Length - end.Length);
            }

            if (throwEx)
            {
                throw new FormatException($"Could not unbox {start} string {end}!");
            }

            return text;
        }

        /// <summary>Unboxes a string.</summary>
        /// <param name="text">The string to be unboxed.</param>
        /// <param name="border">The border.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
        /// <returns>Returns the content between the start and end marks.</returns>
        /// <exception cref="ArgumentNullException">text.</exception>
        /// <exception cref="FormatException">Could not unbox string!.</exception>
        public static string Unbox(this string text, string border, bool throwEx = true)
        {
            if (border == null)
            {
                return text;
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length > border.Length && text.StartsWith(border) && text.EndsWith(border))
            {
                return text.Substring(border.Length, text.Length - border.Length - border.Length);
            }

            if (throwEx)
            {
                throw new FormatException($"Could not unbox {border} string {border}!");
            }

            return text;
        }

        /// <summary>Unboxes a string.</summary>
        /// <param name="text">The string to be unboxed.</param>
        /// <param name="border">The border.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
        /// <returns>Returns the content between the start and end marks.</returns>
        /// <exception cref="ArgumentNullException">text.</exception>
        /// <exception cref="FormatException">Could not unbox {0} string {0}!.</exception>
        public static string Unbox(this string text, char border, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length > 1 && text[0] == border && text[text.Length - 1] == border)
            {
                return text.Substring(1, text.Length - 2);
            }
            if (throwEx)
            {
                throw new FormatException($"Could not unbox {border} string {border}!");
            }

            return text;
        }

        /// <summary>
        /// Unboxes a string (removes enclosing "" and '').
        /// </summary>
        /// <param name="text">The string to be unboxed.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
        /// <returns>Returns the content between the start and end marks.</returns>
        public static string UnboxText(this string text, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
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
                throw new FormatException($"Could not unbox {'"'} string {'"'}'!");
            }

            return text;
        }

        /// <summary>
        /// Unboxes a string (removes enclosing [], {} and ()).
        /// </summary>
        /// <param name="text">The string to be unboxed.</param>
        /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
        /// <returns>Returns the content between the start and end marks.</returns>
        public static string UnboxBrackets(this string text, bool throwEx = true)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
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
                throw new FormatException($"Could not unbox {'"'} string {'"'}!");
            }

            return text;
        }

        /// <summary>Parses a binary size string created by <see cref="FormatSize(double, IFormatProvider)"/> or <see cref="FormatBinarySize(double, IFormatProvider)"/>.</summary>
        /// <param name="value">The value string.</param>
        /// <returns>Parses a value formatted using <see cref="FormatBinarySize(long, IFormatProvider)"/>.</returns>
        /// <exception cref="ArgumentNullException">value.</exception>
        /// <exception cref="ArgumentException">Invalid format in binary size. Expected 'value unit'. Example '15 MB'. Got ''.</exception>
        public static double ParseBinarySize(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var parts = value.Split(' ');
            var error = parts.Length != 2;
            error &= double.TryParse(parts[0], out var size);
            if (!error)
            {
                if (parts[1] == "B")
                {
                    return size;
                }

                foreach (var unit in Enum.GetValues(typeof(SiUnit)))
                {
                    if (parts[1] == unit.ToString() + "Bit")
                    {
                        return size * Math.Pow(1000, (int)unit);
                    }
                }

                foreach (var unit in Enum.GetValues(typeof(IecUnit)))
                {
                    if (parts[1] == unit.ToString() + "it")
                    {
                        return size * Math.Pow(1024, (int)unit);
                    }
                }

                foreach (var unit in Enum.GetValues(typeof(SiUnit)))
                {
                    if (parts[1] == unit.ToString() + "B")
                    {
                        return size * Math.Pow(1000, (int)unit);
                    }
                }

                foreach (var unit in Enum.GetValues(typeof(IecUnit)))
                {
                    if (parts[1] == unit.ToString())
                    {
                        return size * Math.Pow(1024, (int)unit);
                    }
                }
            }
            throw new ArgumentException($"Invalid format in binary size. Expected '<value> <unit>'. Example '15 MB'. Got '{value}'.");
        }

        /// <summary>Randomizes the character casing.</summary>
        /// <param name="value">The string.</param>
        /// <returns>Returns a new string with random case.</returns>
        public static string RandomCase(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var rnd = new Random(Environment.TickCount);
            var result = new char[value.Length];
            for (var i = 0; i < value.Length; i++)
            {
                if ((rnd.Next() % 1) == 0)
                {
                    result[i] = char.ToUpperInvariant(value[i]);
                }
                else
                {
                    result[i] = char.ToLowerInvariant(value[i]);
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
            var i = value?.IndexOf(character) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? string.Empty : value.Substring(i + 1);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The character to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterFirst(this string value, string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            var i = value?.IndexOf(pattern) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? string.Empty : value.Substring(i + pattern.Length);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The pattern to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeFirst(this string value, char character)
        {
            var i = value?.IndexOf(character) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? value : value.Substring(0, i);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The character to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeFirst(this string value, string pattern)
        {
            var i = value?.IndexOf(pattern) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? value : value.Substring(0, i);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The pattern to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterLast(this string value, char character)
        {
            var i = value?.LastIndexOf(character) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? string.Empty : value.Substring(i + 1);
        }

        /// <summary>Returns the string after the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
        public static string AfterLast(this string value, string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            var i = value?.LastIndexOf(pattern) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? string.Empty : value.Substring(i + pattern.Length);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="character">The character to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeLast(this string value, char character)
        {
            var i = value?.LastIndexOf(character) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? value : value.Substring(0, i);
        }

        /// <summary>Returns the string before the specified pattern.</summary>
        /// <param name="value">The string value.</param>
        /// <param name="pattern">The pattern to search for.</param>
        /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
        public static string BeforeLast(this string value, string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }
            var i = value?.LastIndexOf(pattern) ?? throw new ArgumentNullException(nameof(value));
            return i < 0 ? value : value.Substring(0, i);
        }

        /// <summary>Converts a string to a bool.</summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
        public static bool ToBool(this string value, bool defaultValue = false)
        {
            if (bool.TryParse(value, out var result))
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
            if (int.TryParse(value, out var result))
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
            if (uint.TryParse(value, out var result))
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
            if (long.TryParse(value, out var result))
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
            if (ulong.TryParse(value, out var result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Checks whether a specified text is enclosed by some markers.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="start">The start marker.</param>
        /// <param name="end">The end marker.</param>
        /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
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
        /// <param name="text">The text to check.</param>
        /// <param name="start">The start marker.</param>
        /// <param name="end">The end marker.</param>
        /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
        public static bool IsBoxed(this string text, string start, string end)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            return text.StartsWith(start) && text.EndsWith(end);
        }
    }
}

#pragma warning restore CA1307
