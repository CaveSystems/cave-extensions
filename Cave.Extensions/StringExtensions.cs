using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cave;

/// <summary>Gets string functions.</summary>
public static partial class StringExtensions
{
    #region Private Enums

    enum Case { Default = 0, Upper, Digit };

    #endregion Private Enums

    #region Public Fields

    /// <summary>Gets the default date time string used when formatting date time variables for display.</summary>
    public const string DisplayDateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff";

    /// <summary>Gets the default date time string used when formatting date time variables for display.</summary>
    public const string DisplayDateTimeWithTimeZoneFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff K";

    /// <summary>Gets the default date time string used when formatting date time variables for file names.</summary>
    public const string FileNameDateTimeFormat = "yyyy'-'MM'-'dd' 'HHmmss";

    /// <summary>Gets the default date time string used when formatting date time variables for interop.</summary>
    public const string InteropDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";

    /// <summary>Gets the default date time string used when formatting date time variables for interop.</summary>
    public const string InteropDateTimeFormatWithoutTimeZone = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff";

    /// <summary>Gets the default date string used when formatting date time variables for interop.</summary>
    public const string ShortDateFormat = "yyyy'-'MM'-'dd";

    /// <summary>Gets the default time string used when formatting date time variables for interop.</summary>
    public const string ShortTimeFormat = "HH':'mm':'ss'.'fff";

    #endregion Public Fields

    #region Public Methods

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="character">The character to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterFirst(this string? value, char character)
    {
        var result = string.Empty;
        if (value is not null)
        {
            var i = value.IndexOf(character);
            if (i >= 0) result = value[(i + 1)..];
        }
        return result;
    }

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="characters">The characters to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterFirst(this string? value, char[] characters)
    {
        var result = string.Empty;
        if (value is not null)
        {
            var i = value.IndexOfAny(characters);
            if (i >= 0) result = value[(i + 1)..];
        }
        return result;
    }

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="pattern">The character to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterFirst(this string? value, string pattern)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var result = string.Empty;
        if (value is not null)
        {
            var i = value.IndexOf(pattern);
            if (i >= 0) result = value[(i + pattern.Length)..];
        }
        return result;
    }

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="character">The pattern to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterLast(this string? value, char character)
    {
        var result = string.Empty;
        if (value is not null)
        {
            var i = value.LastIndexOf(character);
            if (i >= 0) result = value[(i + 1)..];
        }
        return result;
    }

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="characters">The pattern to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterLast(this string? value, char[] characters)
    {
        var result = string.Empty;
        if (value is not null)
        {
            var i = value.LastIndexOfAny(characters);
            if (i >= 0) result = value[(i + 1)..];
        }
        return result;
    }

    /// <summary>Returns the string after the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>Returns the part of the string after the pattern or an empty string if the pattern cannot be found.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string AfterLast(this string? value, string pattern)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        var result = string.Empty;
        if (value is not null)
        {
            var i = value.LastIndexOf(pattern);
            if (i >= 0) result = value[(i + pattern.Length)..];
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="character">The character to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeFirst(this string? value, char character)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.IndexOf(character);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="characters">The characters to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeFirst(this string? value, char[] characters)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.IndexOfAny(characters);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="pattern">The character to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeFirst(this string? value, string pattern)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.IndexOf(pattern);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="character">The character to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeLast(this string value, char character)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.LastIndexOf(character);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="characters">The characters to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeLast(this string value, char[] characters)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.LastIndexOfAny(characters);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Returns the string before the specified pattern.</summary>
    /// <param name="value">The string value.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>Returns the part of the string before the pattern or the whole string it the pattern is not present.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string BeforeLast(this string value, string pattern)
    {
        string result;
        if (value is null)
        {
            result = string.Empty;
        }
        else
        {
            var i = value.LastIndexOf(pattern);
            result = (i >= 0) ? value[..i] : value;
        }
        return result;
    }

    /// <summary>Boxes the specified text with the given character.</summary>
    /// <param name="text">The text.</param>
    /// <param name="c">The character to pre and append.</param>
    /// <returns>Returns a string starting and ending with the specified character.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Box(this string text, char c) => c + text + c;

    /// <summary>Boxes the specified text with the given string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="s">The string to pre and append.</param>
    /// <returns>Returns a string starting and ending with the specified string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Box(this string text, string s) => s + text + s;

    /// <summary>Boxes the specified text with the given string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <returns>Returns a string starting and ending with the specified string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Box(this string text, string start, string end) => start + text + end;

    /// <summary>Counts the unicode codepoints of the specified characters</summary>
    /// <param name="text">Csharp string</param>
    /// <returns>Returns the number of unicode codepoints.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int CountCodepoints(this string text)
    {
        var result = 0;
        int index;
        for (index = 0; index < text.Length;)
        {
            result++;
            index += char.IsHighSurrogate(text[index]) ? 2 : 1;
        }
        return result - (index - text.Length);
    }

    /// <summary>Counts the unicode codepoints of the specified characters</summary>
    /// <param name="chars">Csharp characters</param>
    /// <param name="usedChars">Optional: number of items used at the <paramref name="chars"/> array.</param>
    /// <returns>Returns the number of unicode codepoints.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int CountCodepoints(this char[] chars, int usedChars = -1)
    {
        if (usedChars < 0) usedChars = chars.Length;
        var result = 0;
        int index;
        for (index = 0; index < usedChars;)
        {
            result++;
            index += char.IsHighSurrogate(chars[index]) ? 2 : 1;
        }
        return result - (index - usedChars);
    }

    /// <summary>Tries to detect the used newline chars in the specified string.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Retruns the detected new line string (CR, LF, CRLF).</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string? DetectNewLine(this string text)
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

        return text.IndexOf('\r') > -1 ? "\r" : null;
    }

    /// <summary>Escapes all characters at the specified string below ascii 32 and above ascii 127.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Returns an escaped ascii 7 bit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
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
                case '"': _ = sb.Append('\\'); _ = sb.Append(c); continue;
                case '\b': _ = sb.Append("\\b"); continue;
                case '\t': _ = sb.Append("\\t"); continue;
                case '\n': _ = sb.Append("\\n"); continue;
                case '\f': _ = sb.Append("\\f"); continue;
                case '\r': _ = sb.Append("\\r"); continue;
                default: break;
            }

            if (c is < ' ' or > (char)127)
            {
                _ = sb.Append($"\\u{(int)c:x4}");
                continue;
            }

            _ = sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>Escapes all characters at the specified string below ascii 32.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Returns an escaped utf8 string.</returns>
    [MethodImpl((MethodImplOptions)256)]
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
                case '"': _ = sb.Append('\\'); _ = sb.Append(c); continue;
                case '\b': _ = sb.Append("\\b"); continue;
                case '\t': _ = sb.Append("\\t"); continue;
                case '\n': _ = sb.Append("\\n"); continue;
                case '\f': _ = sb.Append("\\f"); continue;
                case '\r': _ = sb.Append("\\r"); continue;
                default: break;
            }

            if (c < ' ')
            {
                _ = sb.Append($"\\u{(int)c:x4}");
                continue;
            }

            _ = sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>Enforces a specific string length (appends spaces and cuts to length).</summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ForceLength(this string text, int maxLength) => ForceLength(text, maxLength, string.Empty, " ");

    /// <summary>Enforces a specific string length.</summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="prefix">The prefix to add.</param>
    /// <param name="suffix">The suffix to add.</param>
    /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ForceLength(this string text, int maxLength, string prefix, string suffix)
    {
        text ??= string.Empty;
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
            text = text[..maxLength];
        }

        return text;
    }

    /// <summary>Forces the maximum length.</summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ForceMaxLength(this string text, int maxLength)
    {
        text ??= string.Empty;
        return text.Length > maxLength ? text[..maxLength] : text;
    }

    /// <summary>Forces the maximum length.</summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="endReplacer">The end replacer. (String appended to the end when cutting the text. Sample: "..").</param>
    /// <returns>Returns a string with a length smaller than or equal to maxLength.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ForceMaxLength(this string text, int maxLength, string endReplacer)
    {
        if (endReplacer == null)
        {
            throw new ArgumentNullException(nameof(endReplacer));
        }
        text ??= string.Empty;
        return text.Length > maxLength ? text[..(maxLength - endReplacer.Length)] + endReplacer : text;
    }

    /// <summary>Gets a fail save version of string.Format not supporting extended format options (simply replacing {index} with the arguments.</summary>
    /// <param name="text">The format string.</param>
    /// <param name="args">The parameters.</param>
    /// <returns>The formatted string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Format(this string text, params object[] args)
    {
        if (text == null)
        {
            return string.Empty;
        }
        args ??= [];
        var result = text;
        for (var i = 0; i < args.Length; i++)
        {
            var argument = args[i] == null ? "<null>" : args[i].ToString();
            result = result.Replace("{" + i + "}", argument);
        }

        return result;
    }

    /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatBinarySize(this float size, IFormatProvider? formatProvider = null)
    {
        formatProvider ??= CultureInfo.InvariantCulture;

        var negative = size < 0;
        IecUnit unit = 0;
        while (size >= 1024)
        {
            size /= 1024;
            unit++;
        }

        var result = size.ToString("0.000", formatProvider);
        if (result.Length > 5)
        {
            result = result[..5];
        }

        return (negative ? "-" : string.Empty) + result + " " + unit;
    }

    /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
    /// <param name="value">Value to format.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The formatted string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatBinarySize(this double value, IFormatProvider? formatProvider = null) => FormatBinarySize((float)value, formatProvider);

    /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
    /// <param name="value">Value to format.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The formatted string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatBinarySize(this decimal value, IFormatProvider? formatProvider = null) => FormatBinarySize((float)value, formatProvider);

    /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
    /// <param name="value">Value to format.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The formatted string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatBinarySize(this ulong value, IFormatProvider? formatProvider = null) => FormatBinarySize((float)value, formatProvider);

    /// <summary>Formats a value with IEC values (factor 1024) to a human readable string (kiB, MiB, GiB, ...)</summary>
    /// <param name="value">Value to format.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The formatted string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatBinarySize(this long value, IFormatProvider? formatProvider = null) => FormatBinarySize((float)value, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    /// <param name="seconds">Seconds to format.</param>
    /// <param name="formatProvider">Culture used to format the double value.</param>
    /// <returns>Returns a string like: 10.23ns, 1.345ms, 102.3s, 10.2h, ...</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSeconds(this double seconds, IFormatProvider? formatProvider = null) => FormatSeconds(seconds, null, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    /// <param name="seconds">Seconds to format.</param>
    /// <param name="format">Numberformat</param>
    /// <param name="formatProvider">Culture used to format the double value.</param>
    /// <returns>Returns a string like: 10.23ns, 1.345ms, 102.3s, 10.2h, ...</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSeconds(this double seconds, string? format, IFormatProvider? formatProvider = null)
    {
        formatProvider ??= CultureInfo.InvariantCulture;

        if (seconds < 0)
        {
            return "-" + FormatSeconds(-seconds, format, formatProvider);
        }

        if (seconds == 0)
        {
            return "0.00s";
        }

        if (seconds >= 0.1)
        {
            return FormatTime(TimeSpan.FromTicks((long)(seconds * TimeSpan.TicksPerSecond)), format, formatProvider);
        }

        var part = seconds;
        for (var fraction = SiFraction.m; fraction <= SiFraction.y; fraction++)
        {
            part *= 1000.0;
            if (part > 9.99)
            {
                return part.ToString(format ?? "0.0", formatProvider) + fraction + "s";
            }

            if (part > 0.999)
            {
                return part.ToString(format ?? "0.00", formatProvider) + fraction + "s";
            }
        }

        return seconds.ToString(format, formatProvider) + "s";
    }

    /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSize(this float size, IFormatProvider? formatProvider = null)
    {
        formatProvider ??= CultureInfo.CurrentCulture;

        if (size < 0)
        {
            return "-" + FormatSize(-size, formatProvider);
        }

        var calc = size;
        SiUnit unit = 0;
        while (calc >= 1000)
        {
            calc /= 1000;
            unit++;
        }

        var result = Math.Truncate(calc) == calc ? calc.ToString(formatProvider) : calc.ToString("0.000", formatProvider);
        if (result.Length > 5)
        {
            result = result[..5];
        }

        return result + (unit == 0 ? string.Empty : " " + unit);
    }

    /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSize(this ulong size, IFormatProvider? formatProvider = null) => FormatSize((float)size, formatProvider);

    /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSize(this long size, IFormatProvider? formatProvider = null) => FormatSize((float)size, formatProvider);

    /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="culture">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSize(this decimal size, IFormatProvider? culture = null) => FormatSize((float)size, culture);

    /// <summary>Formats a value with SI units (factor 1000) to a human readable string (k, M, G, ...)</summary>
    /// <param name="size">The size.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a string with significant 4 digits and a unit string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatSize(this double size, IFormatProvider? formatProvider = null) => FormatSize((float)size, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    public static string FormatTicks(this long ticks, IFormatProvider? formatProvider = null) => FormatTime(new TimeSpan(ticks), null, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    public static string FormatTicks(this long ticks, string format, IFormatProvider? formatProvider = null) => FormatTime(new TimeSpan(ticks), format, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    /// <param name="timeSpan">TimeSpan to format.</param>
    /// <param name="formatProvider">Culture used to format the double value.</param>
    /// <returns>Returns a string like: 10.23µs, 1.345ms, 102.3s, 10.2h, ...</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatTime(this TimeSpan timeSpan, IFormatProvider? formatProvider = null) => FormatTime(timeSpan, null, formatProvider);

    /// <summary>Formats a time span to a short one unit value (1.20h, 15.3ms, ...)</summary>
    /// <param name="timeSpan">TimeSpan to format.</param>
    /// <param name="format">Numberformat</param>
    /// <param name="formatProvider">Culture used to format the double value.</param>
    /// <returns>Returns a string like: 10.23µs, 1.345ms, 102.3s, 10.2h, ...</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatTime(this TimeSpan timeSpan, string? format, IFormatProvider? formatProvider = null)
    {
        string Print(double value, string unit) => value.ToString(format ?? (Math.Round(value, 2) > 9.99 ? "0.0" : "0.00"), formatProvider) + unit;

        formatProvider ??= CultureInfo.InvariantCulture;

        if (timeSpan < TimeSpan.Zero)
        {
            return "-" + FormatTime(-timeSpan, format, formatProvider);
        }

        if (timeSpan == TimeSpan.Zero)
        {
            return "0s";
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerMillisecond / 1000)
        {
            var nano = timeSpan.Ticks / (double)(TimeSpan.TicksPerMillisecond / (1000d * 1000));
            return Print(nano, "ns");
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerMillisecond)
        {
            var micro = timeSpan.Ticks / (double)(TimeSpan.TicksPerMillisecond / 1000);
            return Print(micro, "us");
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerSecond)
        {
            var milli = timeSpan.TotalMilliseconds;
            return Print(milli, "ms");
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerMinute)
        {
            var sec = timeSpan.TotalSeconds;
            return Print(sec, "s");
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerHour)
        {
            var min = timeSpan.TotalMinutes;
            return Print(min, "min");
        }

        if (timeSpan.Ticks < TimeSpan.TicksPerDay)
        {
            var h = timeSpan.TotalHours;
            return Print(h, "h");
        }

        var d = timeSpan.TotalDays;
        return Print(d, "d");
    }

    /// <summary>Formats the specified timespan to [[d.]HH:]MM:SS.F.</summary>
    /// <param name="timeSpan">The time span.</param>
    /// <param name="millisecondDigits">The number of millisecond digits.</param>
    /// <returns>The formatted string.</returns>
    /// <exception cref="NotSupportedException">Only 0-3 millisecond digits are supported!.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static string FormatTimeSpan(this TimeSpan timeSpan, int millisecondDigits)
    {
        var result = new StringBuilder();

        var ticks = timeSpan.Ticks;
        if (timeSpan.Ticks > TimeSpan.TicksPerDay)
        {
            _ = result.Append($"{ticks / TimeSpan.TicksPerDay}:");
            ticks %= TimeSpan.TicksPerDay;
        }

        if (ticks > TimeSpan.TicksPerHour)
        {
            _ = result.Append($"{ticks / TimeSpan.TicksPerHour:00}:");
        }

        _ = result.Append($"{timeSpan.Minutes:00}:");
        var seconds = timeSpan.Seconds;
        switch (millisecondDigits)
        {
            case 0:
                if (timeSpan.Milliseconds > 0)
                {
                    seconds++;
                }

                _ = result.Append($"{seconds:00}");
                break;

            case 1:
                _ = result.Append($"{seconds:00}.{timeSpan.Milliseconds / 100:0}");
                break;

            case 2:
                _ = result.Append($"{seconds:00}.{timeSpan.Milliseconds / 100:00}");
                break;

            case 3:
                _ = result.Append($"{seconds:00}.{timeSpan.Milliseconds / 100:000}");
                break;

            default:
                throw new NotSupportedException("Only 0-3 millisecond digits are supported!");
        }

        return result.ToString();
    }

    /// <summary>Retrieves only invalidated chars from a string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <returns>Returns a new string containing only the invalid chars.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetInvalidChars(this string text, string validChars)
    {
        if ((text == null) || string.IsNullOrEmpty(validChars))
        {
            return string.Empty;
        }

        var result = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (validChars.IndexOf(c) < 0)
            {
                _ = result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>Gets a part of a string.</summary>
    /// <param name="data">Data to parse.</param>
    /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark).</param>
    /// <param name="startMark">StartMark to check/search for.</param>
    /// <param name="endMark">EndMark to search for.</param>
    /// <param name="throwException">if set to <c>true</c> [throw exception if string cannot be found].</param>
    /// <returns>The substring.</returns>
    /// <exception cref="System.ArgumentNullException">data.</exception>
    /// <exception cref="ArgumentException">StartMark not found! or EndMark not found!.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!.</exception>
    [MethodImpl((MethodImplOptions)256)]
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
                return string.Empty;
            }

            throw new ArgumentException("StartMark not found!");
        }

        if (data[start] != startMark)
        {
            if (!throwException)
            {
                return string.Empty;
            }

            throw new ArgumentOutOfRangeException(nameof(startMark), "StartMark does not match!");
        }

        var end = data.IndexOf(endMark, start + 1);
        if (end <= start)
        {
            if (!throwException)
            {
                return string.Empty;
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
    /// <exception cref="System.ArgumentNullException">data or startMark or endMark.</exception>
    /// <exception cref="ArgumentException">StartMark not found! or EndMark not found!.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">StartMark does not match!.</exception>
    [MethodImpl((MethodImplOptions)256)]
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
            start = data.IndexOf(startMark, StringComparison.Ordinal);
        }

        if (start < 0)
        {
            if (!throwException)
            {
                return string.Empty;
            }

            throw new ArgumentException("StartMark not found!");
        }

        if (!data[start..].StartsWith(startMark, StringComparison.Ordinal))
        {
            if (!throwException)
            {
                return string.Empty;
            }

            throw new ArgumentOutOfRangeException(nameof(startMark), "StartMark does not match!");
        }

        start += startMark.Length;
        var end = data.IndexOf(endMark, start + 1, StringComparison.Ordinal);
        if (end <= start)
        {
            if (!throwException)
            {
                return string.Empty;
            }

            throw new ArgumentException("EndMark not found!");
        }

        return data[start..end];
    }

    /// <summary>Retrieves only validated chars from a string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <returns>Returns a new string containing only the valid chars.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetValidChars(this string text, string validChars)
    {
        if ((text == null) || string.IsNullOrEmpty(validChars))
        {
            return string.Empty;
        }

        var result = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (validChars.IndexOf(c) > -1)
            {
                _ = result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>Gets whether the specified string contains invalid chars or not.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <returns>Returns true if the text contains invalid chars.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool HasInvalidChars(this string text, string validChars)
    {
        if ((text == null) || string.IsNullOrEmpty(validChars))
        {
            return false;
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

    /// <summary>Gets whether the specified string contains valid chars or not.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <returns>Returns true if the text contains valid chars.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool HasValidChars(this string text, string validChars)
    {
        if ((text == null) || string.IsNullOrEmpty(validChars))
        {
            return false;
        }

        foreach (var c in text)
        {
            if (validChars.IndexOf(c) > -1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets the index of the first invalid char or -1 if all chars are valid.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <returns>Returns the index or -1.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int IndexOfInvalidChar(this string text, string validChars)
    {
        if (text != null)
        {
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
        }
        return -1;
    }

    /// <summary>Gets the index of the first invalid char or -1 if all chars are valid.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The string with the valid chars.</param>
    /// <param name="start">The start index.</param>
    /// <returns>Returns the index or -1.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int IndexOfInvalidChar(this string text, string validChars, int start)
    {
        if (text != null)
        {
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
        }
        return -1;
    }

    /// <summary>Checks whether a specified text is enclosed by some markers.</summary>
    /// <param name="text">The text to check.</param>
    /// <param name="marker">The marker.</param>
    /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool IsBoxed(this string text, char marker) => !string.IsNullOrEmpty(text) && (text[0] == marker) && (text[^1] == marker);

    /// <summary>Checks whether a specified text is enclosed by some markers.</summary>
    /// <param name="text">The text to check.</param>
    /// <param name="start">The start marker.</param>
    /// <param name="end">The end marker.</param>
    /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool IsBoxed(this string text, char start, char end) => !string.IsNullOrEmpty(text) && (text[0] == start) && (text[^1] == end);

    /// <summary>Checks whether a specified text is enclosed by some markers.</summary>
    /// <param name="text">The text to check.</param>
    /// <param name="marker">The start marker.</param>
    /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool IsBoxed(this string text, string marker)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        if (marker == null)
        {
            throw new ArgumentNullException(nameof(marker));
        }

        return text.StartsWith(marker, StringComparison.Ordinal) && text.EndsWith(marker, StringComparison.Ordinal);
    }

    /// <summary>Checks whether a specified text is enclosed by some markers.</summary>
    /// <param name="text">The text to check.</param>
    /// <param name="start">The start marker.</param>
    /// <param name="end">The end marker.</param>
    /// <returns>Returns true if the string is boxed with the start and end mark.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        return text.StartsWith(start, StringComparison.Ordinal) && text.EndsWith(end, StringComparison.Ordinal);
    }

    /// <summary>Joins a collection to a string.</summary>
    /// <param name="array">The string array.</param>
    /// <param name="separator">The seperator.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Join(this IEnumerable array, string separator, IFormatProvider? formatProvider = null)
    {
        if (array == null)
        {
            return string.Empty;
        }

        formatProvider ??= CultureInfo.CurrentCulture;

        if (separator == null)
        {
            throw new ArgumentNullException(nameof(separator));
        }

        var result = new StringBuilder();
        foreach (var obj in array)
        {
            if (result.Length != 0)
            {
                _ = result.Append(separator);
            }

            _ = result.Append(ToString(obj, formatProvider));
        }

        return result.ToString();
    }

    /// <summary>Joins a collection to a string.</summary>
    /// <param name="array">The string array.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Join(this IEnumerable array, char separator, IFormatProvider? formatProvider = null) => Join(array, separator.ToString(), formatProvider);

    /// <summary>Joins a collection to a string.</summary>
    /// <param name="array">The string array.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Join(this IEnumerable array, IFormatProvider? formatProvider = null)
    {
        formatProvider ??= CultureInfo.CurrentCulture;

        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        var result = new StringBuilder();
        foreach (var obj in array)
        {
            result.Append(ToString(obj, formatProvider));
        }

        return result.ToString();
    }

    /// <summary>Joins a collection to a string with newlines for all systems.</summary>
    /// <param name="texts">The string collection.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinNewLine(this string[] texts) => Join(texts, "\r\n", null);

    /// <summary>Joins a collection to a string with newlines for all systems.</summary>
    /// <param name="array">The string array.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinNewLine(this IEnumerable array) => Join(array, "\r\n", null);

    /// <summary>Parses a binary size string created by <see cref="FormatSize(double, IFormatProvider)"/> or <see cref="FormatBinarySize(double, IFormatProvider)"/>.</summary>
    /// <param name="value">The value string.</param>
    /// <returns>Parses a value formatted using <see cref="FormatBinarySize(long, IFormatProvider)"/>.</returns>
    /// <exception cref="ArgumentNullException">value.</exception>
    /// <exception cref="ArgumentException">Invalid format in binary size. Expected 'value unit'. Example '15 MB'. Got ''.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static double ParseBinarySize(this string value)
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
                if (parts[1] == (unit + "Bit"))
                {
                    return size * Math.Pow(1000, (int)unit);
                }
            }

            foreach (var unit in Enum.GetValues(typeof(IecUnit)))
            {
                if (parts[1] == (unit + "it"))
                {
                    return size * Math.Pow(1024, (int)unit);
                }
            }

            foreach (var unit in Enum.GetValues(typeof(SiUnit)))
            {
                if (parts[1] == (unit + "B"))
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

    /// <summary>Parses a DateTime.</summary>
    /// <param name="dateTimeString">String value to parse.</param>
    /// <returns>The parsed datetime.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static DateTime ParseDateTime(this string dateTimeString) => ParseDateTime(dateTimeString, null);

    /// <summary>Parses a DateTime.</summary>
    /// <param name="dateTimeString">String value to parse.</param>
    /// <param name="formatProvider">Format provider used to check for the full date time pattern.</param>
    /// <returns>The parsed datetime.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static DateTime ParseDateTime(this string dateTimeString, IFormatProvider? formatProvider)
    {
        DateTime result;
        if (formatProvider is CultureInfo culture)
        {
            if (DateTime.TryParseExact(dateTimeString, culture.DateTimeFormat.FullDateTimePattern, formatProvider, default, out result))
            {
                return result;
            }
        }

        if (DateTime.TryParse(dateTimeString, formatProvider, default, out result))
        {
            return result;
        }

        if (DateTimeParser.TryParseDateTime(dateTimeString, out result, out var _))
        {
            return result;
        }

        return DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture);
    }

    /// <summary>Converts a hex string to a byte array.</summary>
    /// <param name="hex">The string with hex values.</param>
    /// <returns>The byte array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static byte[] ParseHexString(this string hex)
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

    /// <summary>Parses a Point.ToString() result.</summary>
    /// <param name="point">String value to parse.</param>
    /// <returns>The parsed point.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static Point ParsePoint(this string point)
    {
        if (point == null)
        {
            throw new ArgumentNullException(nameof(point));
        }

        var data = Unbox(point.Trim(), "{", "}");
        var parts = data.Split(',');
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("X=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        var x = int.Parse(parts[0].Trim()[2..], CultureInfo.CurrentCulture);
        var y = int.Parse(parts[1].Trim()[2..], CultureInfo.CurrentCulture);
        return new(x, y);
    }

    /// <summary>Parses a Point.ToString() result.</summary>
    /// <param name="point">String value to parse.</param>
    /// <returns>The parsed point.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static PointF ParsePointF(this string point)
    {
        if (point == null)
        {
            throw new ArgumentNullException(nameof(point));
        }

        var data = Unbox(point.Trim(), "{", "}");
        var parts = data.Split([", "], StringSplitOptions.None);
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("X=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid point data '{point}'!", nameof(point));
        }

        var x = float.Parse(parts[0].Trim()[2..], CultureInfo.CurrentCulture);
        var y = float.Parse(parts[1].Trim()[2..], CultureInfo.CurrentCulture);
        return new(x, y);
    }

    /// <summary>Parses a Rectangle.ToString() result.</summary>
    /// <param name="rect">String value to parse.</param>
    /// <returns>The parsed rectangle.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static Rectangle ParseRectangle(this string rect)
    {
        var data = Unbox(rect, "{", "}");
        var parts = data.Split(',');
        if (parts.Length != 4)
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("X=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[2].Trim().ToUpperInvariant().StartsWith("WIDTH=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[3].Trim().ToUpperInvariant().StartsWith("HEIGHT=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        var x = int.Parse(parts[0].Trim()[2..], CultureInfo.CurrentCulture);
        var y = int.Parse(parts[1].Trim()[2..], CultureInfo.CurrentCulture);
        var w = int.Parse(parts[2].Trim()[6..], CultureInfo.CurrentCulture);
        var h = int.Parse(parts[3].Trim()[7..], CultureInfo.CurrentCulture);
        return new(x, y, w, h);
    }

    /// <summary>Parses a Rectangle.ToString() result.</summary>
    /// <param name="rect">String value to parse.</param>
    /// <returns>The parsed rectangle.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static RectangleF ParseRectangleF(this string rect)
    {
        var data = Unbox(rect, "{", "}");
        var parts = data.Split(',');
        if (parts.Length == 8)
        {
            parts = [parts[0] + "," + parts[1], parts[2] + "," + parts[3], parts[4] + "," + parts[5], parts[6] + "," + parts[7]];
        }
        if (parts.Length != 4)
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("X=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("Y=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[2].Trim().ToUpperInvariant().StartsWith("WIDTH=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        if (!parts[3].Trim().ToUpperInvariant().StartsWith("HEIGHT=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid rect data '{rect}'!", nameof(rect));
        }

        var x = float.Parse(parts[0].Trim()[2..], CultureInfo.CurrentCulture);
        var y = float.Parse(parts[1].Trim()[2..], CultureInfo.CurrentCulture);
        var w = float.Parse(parts[2].Trim()[6..], CultureInfo.CurrentCulture);
        var h = float.Parse(parts[3].Trim()[7..], CultureInfo.CurrentCulture);
        return new(x, y, w, h);
    }

    /// <summary>Parses a Size.ToString() result.</summary>
    /// <param name="size">String value to parse.</param>
    /// <returns>The parsed size.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static Size ParseSize(this string size)
    {
        if (size == null)
        {
            throw new ArgumentNullException(nameof(size));
        }

        var data = Unbox(size.Trim(), "{", "}");
        var parts = data.Split(',');
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("WIDTH=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("HEIGHT=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        var w = int.Parse(parts[0].Trim()[6..], CultureInfo.CurrentCulture);
        var h = int.Parse(parts[1].Trim()[7..], CultureInfo.CurrentCulture);
        return new(w, h);
    }

    /// <summary>Parses a Size.ToString() result.</summary>
    /// <param name="size">String value to parse.</param>
    /// <returns>The parsed size.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static SizeF ParseSizeF(this string size)
    {
        if (size == null)
        {
            throw new ArgumentNullException(nameof(size));
        }

        var data = Unbox(size.Trim(), "{", "}");
        var parts = data.Split([", "], StringSplitOptions.None);
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        if (!parts[0].Trim().ToUpperInvariant().StartsWith("WIDTH=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        if (!parts[1].Trim().ToUpperInvariant().StartsWith("HEIGHT=", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid size data '{size}'!", nameof(size));
        }

        var w = float.Parse(parts[0].Trim()[6..], CultureInfo.CurrentCulture);
        var h = float.Parse(parts[1].Trim()[7..], CultureInfo.CurrentCulture);
        return new(w, h);
    }

    /// <summary>Converts a string to the specified target type using the <see cref="TypeExtension.ConvertValue(Type, object, IFormatProvider)"/> method.</summary>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <param name="value">String value to convert.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>Returns a new value instance.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static T? ParseValue<T>(this string value, IFormatProvider? formatProvider = null) => (T?)TypeExtension.ConvertValue(typeof(T), value, formatProvider);

    /// <summary>Randomizes the character casing.</summary>
    /// <param name="value">The string.</param>
    /// <returns>Returns a new string with random case.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string RandomCase(this string value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        var rnd = new Random(Environment.TickCount);
        var result = new char[value.Length];
        for (var i = 0; i < value.Length; i++)
        {
            result[i] = (rnd.Next() % 1) == 0 ? char.ToUpperInvariant(value[i]) : char.ToLowerInvariant(value[i]);
        }

        return new(result);
    }

    /// <summary>Removes any newline markings.</summary>
    /// <param name="text">The text.</param>
    /// <returns>Returns a string without any newline characters.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string RemoveNewLine(this string text)
    {
        if (text == null)
        {
            return string.Empty;
        }

        var result = new StringBuilder(text.Length);
        var newLineChars = new[]
        {
            '\r',
            '\n'
        };
        var pos = 0;
        var index = text.IndexOfAny(newLineChars);
        while (index > -1)
        {
            var size = index - pos;
            if (size > 0)
            {
                _ = result.Append(text.Substring(pos, size));
            }

            pos = index + 1;
            index = text.IndexOfAny(newLineChars, pos);
        }

        {
            var size = text.Length - pos;
            if (size > 0)
            {
                _ = result.Append(text.Substring(pos, size));
            }
        }
        return result.ToString();
    }

    /// <summary>A fast pattern replacement function for large strings.</summary>
    /// <param name="text">The text.</param>
    /// <param name="pattern">The pattern to find.</param>
    /// <param name="replacement">The replacement.</param>
    /// <returns>The replaced text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ReplaceCaseInsensitiveInvariant(this string text, string pattern, string replacement)
    {
        if (pattern == null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (replacement == null)
        {
            throw new ArgumentNullException(nameof(replacement));
        }

        if (text == null)
        {
            return string.Empty;
        }

        var result = text.ToUpperInvariant();
        pattern = pattern.ToUpperInvariant();

        // get the maximum change
        var maxChange = 0;
        if (pattern.Length < replacement.Length)
        {
            maxChange = (text.Length / pattern.Length) * (replacement.Length - pattern.Length);
        }

        var chars = new char[text.Length + maxChange];
        var count = 0;
        var start = 0;
        var index = result.IndexOf(pattern, StringComparison.Ordinal);
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
            index = result.IndexOf(pattern, start, StringComparison.Ordinal);
        }

        if (start == 0)
        {
            return text;
        }

        for (var i = start; i < text.Length; i++)
        {
            chars[count++] = text[i];
        }

        return new(chars, 0, count);
    }

    /// <summary>Retrieves all specified chars with a string.</summary>
    /// <param name="text">The text.</param>
    /// <param name="chars">The array of chars to retrieve.</param>
    /// <param name="replacer">The replacer string.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        replacer ??= string.Empty;

        var result = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (Array.IndexOf(chars, c) > -1)
            {
                _ = result.Append(replacer);
            }
            else
            {
                _ = result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>Retrieves only validated chars from a string and replaces all other occurances.</summary>
    /// <param name="text">The text.</param>
    /// <param name="chars">The array of chars to retrieve.</param>
    /// <param name="replacer">The replacer string.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        replacer ??= string.Empty;

        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (chars.IndexOf(c) > -1)
            {
                _ = sb.Append(replacer);
            }
            else
            {
                _ = sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>Retrieves only validated chars from a string and replaces all other occurances.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The array of chars to retrieve.</param>
    /// <param name="replacer">The replacer string.</param>
    /// <returns>Returns only valid characters.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        replacer ??= string.Empty;

        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (Array.IndexOf(validChars, c) > -1)
            {
                _ = sb.Append(c);
            }
            else
            {
                _ = sb.Append(replacer);
            }
        }

        return sb.ToString();
    }

    /// <summary>Retrieves only validated chars from a string and replaces all other occurances.</summary>
    /// <param name="text">The text.</param>
    /// <param name="validChars">The array of chars to retrieve.</param>
    /// <param name="replacer">The replacer string.</param>
    /// <returns>Returns a string containing only valid characters.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        replacer ??= string.Empty;

        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (validChars.IndexOf(c) > -1)
            {
                _ = sb.Append(c);
            }
            else
            {
                _ = sb.Append(replacer);
            }
        }

        return sb.ToString();
    }

    /// <summary>Replaces newline markings.</summary>
    /// <param name="text">the text.</param>
    /// <param name="newLine">The new newline markings.</param>
    /// <returns>Returns a new string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ReplaceNewLine(this string text, string newLine)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var strings = SplitNewLine(text);
        return string.Join(newLine, strings);
    }

    /// <summary>Replaces the specified part of a string by splitting, replacing and joining.</summary>
    /// <param name="text">The full text.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="index">The index of the part to replace.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns>The replaced string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ReplacePart(this string text, char separator, int index, string newValue)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var parts = text.Split(separator);
        parts[index] = newValue;
        return string.Join($"{separator}", parts);
    }

    /// <summary>Splits a string at the specified indices.</summary>
    /// <param name="text">The text.</param>
    /// <param name="indices">The indices.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitAt(this string text, IEnumerable<int> indices)
    {
        if (text == null)
        {
            return [];
        }

        if (indices == null)
        {
            throw new ArgumentNullException(nameof(indices));
        }

        var items = new List<string>();
        var start = 0;
        foreach (var i in indices)
        {
            items.Add(text[start..i]);
            start = i;
        }

        if (start < text.Length)
        {
            items.Add(text[start..]);
        }

        return [.. items];
    }

    /// <summary>Splits a string at the specified indices.</summary>
    /// <param name="text">The text.</param>
    /// <param name="indices">The indices.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitAt(this string text, params int[] indices) => SplitAt(text, (IEnumerable<int>)indices);

    /// <summary>Splits a string at the specified separators and allows to keep the separators in the list.</summary>
    /// <param name="text">The text.</param>
    /// <param name="separators">The arrays of chars used to seperate the text.</param>
    /// <returns>The array of seperated strings.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitKeepSeparators(this string text, params char[] separators)
    {
        if (string.IsNullOrEmpty(text))
        {
            return [];
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
            result.Add(text[last..]);
        }

        return [.. result];
    }

    /// <summary>Splits a string at the specified separators and allows to keep the separators in the list.</summary>
    /// <param name="text">The text.</param>
    /// <param name="isSeparator">A function to determine whether the char is a seperator.</param>
    /// <returns>The array of seperated strings.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitKeepSeparators(this string text, Func<char, bool> isSeparator)
    {
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        var result = new List<string>();
        var start = 0;
        for (var i = 1; i < text.Length; i++)
        {
            if (isSeparator(text[i]))
            {
                var count = i - start - 1;
                var part = text.Substring(start, count);
                if (part.Length > 0)
                {
                    result.Add(part);
                }
                result.Add(text[i].ToString());
                start = i + 1;
            }
        }

        var remainder = text[start..];
        if (remainder.Length > 0)
        {
            result.Add(remainder);
        }
        return [.. result];
    }

    /// <summary>Splits a string at platform independent newline markings (CR, LF, CRLF, #0).</summary>
    /// <param name="text">The text.</param>
    /// <param name="textSplitOptions">The options.</param>
    /// <returns>Returns a new array of strings.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitNewLine(this string text, StringSplitOptions textSplitOptions)
    {
        if (text == null)
        {
            return [];
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
                    result.Add(text[start..indexNull]);
                    start = indexNull + 1;
                    continue;
                }

                // NL<0<CR
                result.Add(text[start..indexNL]);
                start = indexNL + 1;
                continue;
            }

            // CRLF ?
            if (indexCR == (indexNL - 1))
            {
                // CRLF
                result.Add(text[start..indexCR]);
                start = indexCR + 2;
                continue;
            }

            // CR<NL
            if (indexCR < indexNL)
            {
                result.Add(text[start..indexCR]);
                start = indexCR + 1;
                continue;
            }

            // NL?
            if (indexNL < int.MaxValue)
            {
                result.Add(text[start..indexNL]);
                start = indexNL + 1;
                continue;
            }

            break;
        }

        if (start < text.Length)
        {
            result.Add(text[start..]);
        }

        if (textSplitOptions == StringSplitOptions.RemoveEmptyEntries)
        {
            _ = result.RemoveAll(string.IsNullOrEmpty);
        }

        return [.. result];
    }

    /// <summary>
    /// Splits a string at platform independent newline markings (CR, LF, CRLF, #0). Empty entries will be kept. (This equals <see cref="SplitNewLine(string,
    /// StringSplitOptions)"/> with <see cref="StringSplitOptions.None"/>).
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitNewLine(this string text) => SplitNewLine(text, StringSplitOptions.None);

    /// <summary>
    /// Splits a string at newline markings and after a specified length. Trys to split only at space and newline, but will split anywhere else if its not possible.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="maxLength">The maximum length of the new strings.</param>
    /// <returns>Returns a new array of strings.</returns>
    [MethodImpl((MethodImplOptions)256)]
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
                if ((currentText.Length + textPart.Length) <= maxLength)
                {
                    // textpart fits into this line
                    currentText += textPart;
                }
                else if (textPart.Length > maxLength)
                {
                    // textpart does not fit into this line and does not fit in an empty line
                    var partLength = maxLength - currentText.Length;
                    currentText += textPart[..partLength];
                    array.Add(currentText);
                    currentText = textPart[partLength..];
                    while (currentText.Length > maxLength)
                    {
                        array.Add(currentText[..maxLength]);
                        currentText = currentText[maxLength..];
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
                    if ((i + 1) < parts.Length)
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

        return [.. array];
    }

    /// <summary>
    /// Gets a substring from the end of the specified string. Positive values retrieve the number of characters from end. Negative values retrieve everything
    /// in front of the specified len - count.
    /// </summary>
    /// <param name="text">The string.</param>
    /// <param name="count">The number of characters at the end to be retrieved.</param>
    /// <returns>The substring.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string SubstringEnd(this string text, int count)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var len = text.Length;
        if ((count > len) || (count == 0) || (-count > len))
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Count needs to be in range -len..-1 or 1..len");
        }

        if (count > 0)
        {
            return text[(len - count)..];
        }

        // if count < 0
        return text[..(len + count)];
    }

    /// <summary>Converts a string to a bool.</summary>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool ToBool(this string value, bool defaultValue = false) => bool.TryParse(value, out var result) ? result : defaultValue;

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this double value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this float value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this int value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this uint value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this long value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a value to a hexadecimal string.</summary>
    /// <param name="value">The value.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>Returns the hexadecimal representation of the value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this ulong value, bool upperCase = false) => ToHexString(BitConverter.GetBytes(value), BitConverter.IsLittleEndian, upperCase);

    /// <summary>Converts a byte array to a hexadecimal string.</summary>
    /// <param name="data">The data.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>The converted string.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToHexString(this byte[] data, bool upperCase = false) => ToHexString(data, false, upperCase);

    /// <summary>Converts a byte array to a hexadecimal string.</summary>
    /// <param name="data">The data.</param>
    /// <param name="isLittleEndian">Defines whether the specified data has little endian byte order or not.</param>
    /// <param name="upperCase">if set to <c>true</c> [use upper case caracters].</param>
    /// <returns>The converted string.</returns>
    /// <exception cref="ArgumentNullException">data.</exception>
    [MethodImpl((MethodImplOptions)256)]
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
            _ = stringBuilder.Append(data[i].ToString(format, CultureInfo.InvariantCulture));
        }

        return stringBuilder.ToString();
    }

    /// <summary>Converts a string to an integer.</summary>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static int ToInt32(this string value, int defaultValue = 0) => int.TryParse(value, out var result) ? result : defaultValue;

    /// <summary>Converts a string to an integer.</summary>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static long ToInt64(this string value, long defaultValue = 0) => long.TryParse(value, out var result) ? result : defaultValue;

    /// <summary>Converts a string to an integer.</summary>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static ulong ToInt64(this string value, ulong defaultValue = 0) => ulong.TryParse(value, out var result) ? result : defaultValue;

    /// <summary>Returns the objects.ToString() result or "&lt;null&gt;".</summary>
    /// <param name="value">Value to format.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToString(object value, IFormatProvider? formatProvider)
    {
        if (value == null)
        {
            return "<null>";
        }

        formatProvider ??= CultureInfo.InvariantCulture;

        // special handling for roundtrip types
        if (value is double d)
        {
            return d.ToString("R", formatProvider);
        }

        if (value is float f)
        {
            return f.ToString("R", formatProvider);
        }

        if (value is DateTime dt)
        {
            if (formatProvider is CultureInfo culture)
            {
                if (culture.Calendar is not GregorianCalendar)
                {
                    throw new NotSupportedException($"Calendar {culture.Calendar} not supported!");
                }
                if (dt.Kind is DateTimeKind.Local or DateTimeKind.Unspecified)
                {
                    return dt.ToString(culture.DateTimeFormat.FullDateTimePattern, culture);
                }
                ;
            }
            return dt.ToString(InteropDateTimeFormat, formatProvider);
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, formatProvider);
        }

        return value is ICollection collection ? value + " {" + Join(collection, ",", formatProvider) + "}" : $"{value}";
    }

    /// <summary>Returns the objects.ToString() result or "&lt;null&gt;".</summary>
    /// <param name="value">Value to format.</param>
    /// <returns>The string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToString(object value) => ToString(value, null);

    /// <summary>Returns an array of strings using the element objects ToString() method with invariant culture.</summary>
    /// <param name="enumerable">The array ob objects.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] ToStringArray(this IEnumerable enumerable) => ToStringArray(enumerable, CultureInfo.InvariantCulture);

    /// <summary>Returns an array of strings using the element objects ToString() method.</summary>
    /// <param name="enumerable">The array ob objects.</param>
    /// <param name="cultureInfo">The culture to use during formatting.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
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

        return [.. result];
    }

    /// <summary>Converts a exception to a string array.</summary>
    /// <param name="exception">The <see cref="Exception"/>.</param>
    /// <param name="debug">Include debug information (stacktrace, data).</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] ToStrings(this Exception exception, bool debug = false)
    {
        // ignore AggregateException
        if (exception is AggregateException aggregateException)
        {
            return aggregateException.InnerExceptions.SelectMany(ex => ex.ToStrings()).ToArray();
        }

        if (exception == null)
        {
            return [];
        }

        var strings = new List<string>();
        if (debug)
        {
            strings.Add("Message:");
        }

        foreach (var s in SplitNewLine(exception.Message))
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
            if (!string.IsNullOrEmpty(exception.Source))
            {
                strings.Add("Source:");
                foreach (var s in SplitNewLine(exception.Source))
                {
                    if ((s.Trim().Length == 0) || !ASCII.IsClean(s))
                    {
                        continue;
                    }

                    strings.Add("  " + s);
                }
            }

            if (exception.Data.Count > 0)
            {
                strings.Add("Data:");
                foreach (var key in exception.Data.Keys)
                {
                    strings.Add($"  {key}: {exception.Data[key]}");
                }
            }

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                strings.Add("StackTrace:");
                foreach (var s in SplitNewLine(exception.StackTrace))
                {
                    if ((s.Trim().Length == 0) || !ASCII.IsClean(s))
                    {
                        continue;
                    }

                    strings.Add("  " + s);
                }
            }
        }

        if (exception.InnerException != null)
        {
            if (debug)
            {
                strings.Add("---");
            }

            strings.AddRange(ToStrings(exception.InnerException, debug));
        }

        if (exception is ReflectionTypeLoadException reflectionTypeLoadException)
        {
            foreach (var inner in reflectionTypeLoadException.LoaderExceptions)
            {
                if (inner is null) continue;

                if (debug)
                {
                    strings.Add("---");
                }

                strings.AddRange(ToStrings(inner, debug));
            }
        }

        return [.. strings];
    }

    /// <summary>Converts a exception to a simple text message.</summary>
    /// <param name="ex">The <see cref="Exception"/>.</param>
    /// <param name="debug">Include debug information (stacktrace, data).</param>
    /// <returns>The text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string ToText(this Exception ex, bool debug = false) => string.Join(Environment.NewLine, ToStrings(ex, debug));

    /// <summary>Converts a string to an integer.</summary>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>Returns the integer representation of the string if the parser succeeds or the default value.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static uint ToUInt32(this string value, uint defaultValue = 0) => uint.TryParse(value, out var result) ? result : defaultValue;

    /// <summary>Parses a DateTime (Supported formats: <see cref="InteropDateTimeFormat"/>, <see cref="DisplayDateTimeFormat"/>, default).</summary>
    /// <param name="dateTime">String value to parse.</param>
    /// <param name="result">The parsed datetime.</param>
    /// <returns>True if the value could be parsed.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static bool TryParseDateTime(string dateTime, out DateTime result)
        => DateTime.TryParseExact(dateTime, InteropDateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result)
         || DateTime.TryParseExact(dateTime, InteropDateTimeFormatWithoutTimeZone, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result)
         || DateTime.TryParseExact(dateTime, DisplayDateTimeWithTimeZoneFormat, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result)
         || DateTime.TryParseExact(dateTime, DisplayDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result)
         || DateTime.TryParse(dateTime, out result)
         || DateTimeParser.TryParseDateTime(dateTime, out result);

    /// <summary>Unboxes a string (removes strings from start end end).</summary>
    /// <param name="text">The string to be unboxed.</param>
    /// <param name="start">Start of box.</param>
    /// <param name="end">End of box.</param>
    /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
    /// <returns>Returns the content between the start and end marks.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string Unbox(this string text, string start, string end, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (start == null)
        {
            throw new ArgumentNullException(nameof(start));
        }

        if (end == null)
        {
            throw new ArgumentNullException(nameof(end));
        }

        if ((text.Length > start.Length) && text.StartsWith(start, StringComparison.Ordinal) && text.EndsWith(end, StringComparison.Ordinal))
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
    [MethodImpl((MethodImplOptions)256)]
    public static string Unbox(this string text, string border, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (border == null)
        {
            return text;
        }

        if ((text.Length > border.Length) && text.StartsWith(border, StringComparison.Ordinal) && text.EndsWith(border, StringComparison.Ordinal))
        {
            return text[border.Length..^border.Length];
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
    [MethodImpl((MethodImplOptions)256)]
    public static string Unbox(this string text, char border, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if ((text.Length > 1) && (text[0] == border) && (text[^1] == border))
        {
            return text[1..^1];
        }

        if (throwEx)
        {
            throw new FormatException($"Could not unbox {border} string {border}!");
        }

        return text;
    }

    /// <summary>Unboxes a string.</summary>
    /// <param name="text">The string to be unboxed.</param>
    /// <param name="start">Start of box.</param>
    /// <param name="end">End of box.</param>
    /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
    /// <returns>Returns the content between the start and end marks.</returns>
    /// <exception cref="ArgumentNullException">text.</exception>
    /// <exception cref="FormatException">Could not unbox {0} string {1}!.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static string Unbox(this string text, char start, char end, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if ((text.Length > 1) && (text[0] == start) && (text[^1] == end))
        {
            return text[1..^1];
        }

        if (throwEx)
        {
            throw new FormatException($"Could not unbox {start} string {end}!");
        }

        return text;
    }

    /// <summary>Unboxes a string (removes enclosing [], {} and ()).</summary>
    /// <param name="text">The string to be unboxed.</param>
    /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
    /// <returns>Returns the content between the start and end marks.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string UnboxBrackets(this string text, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (text.Length > 1)
        {
            if (text.StartsWith('(') && text.EndsWith(')'))
            {
                return text[1..^1];
            }

            if (text.StartsWith('[') && text.EndsWith(']'))
            {
                return text[1..^1];
            }

            if (text.StartsWith('{') && text.EndsWith('}'))
            {
                return text[1..^1];
            }
        }

        return !throwEx ? text : throw new FormatException($"Could not unbox {'"'} string {'"'}!");
    }

    /// <summary>Unboxes a string (removes enclosing "" and '').</summary>
    /// <param name="text">The string to be unboxed.</param>
    /// <param name="throwEx">Throw a FormatException on unboxing error.</param>
    /// <returns>Returns the content between the start and end marks.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string UnboxText(this string text, bool throwEx = true)
    {
        if (text == null)
        {
            return string.Empty;
        }

        if (text.Length > 1)
        {
            if (text.StartsWith('\'') && text.EndsWith('\''))
            {
                return text[1..^1];
            }

            if (text.StartsWith('\"') && text.EndsWith('\"'))
            {
                return text[1..^1];
            }
        }

        if (throwEx)
        {
            throw new FormatException($"Could not unbox {'"'} string {'"'}'!");
        }

        return text;
    }

    /// <summary>Unescapes the specified text and throws exceptions on invalid escape codes.</summary>
    /// <param name="text">The text (escaped ascii 7 bit string).</param>
    /// <returns>Returns the unescaped string.</returns>
    /// <exception cref="InvalidDataException">Invalid escape code.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static string Unescape(this string text) => Unescape(text, true);

    /// <summary>Unescapes the specified text.</summary>
    /// <param name="text">The text (escaped ascii 7 bit string).</param>
    /// <param name="throwOnInvalid">Throw exception on invalid escape codes.</param>
    /// <returns>Returns the unescaped string.</returns>
    /// <exception cref="InvalidDataException">Invalid escape code.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static string Unescape(this string text, bool throwOnInvalid)
    {
        if (text == null)
        {
            return string.Empty;
        }
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
                    case '"': _ = sb.Append('"'); continue;
                    case '\\': _ = sb.Append('\\'); continue;
                    case 'b': _ = sb.Append('\b'); continue;
                    case 't': _ = sb.Append('\t'); continue;
                    case 'n': _ = sb.Append('\n'); continue;
                    case 'f': _ = sb.Append('\f'); continue;
                    case 'r': _ = sb.Append('\r'); continue;
                    case 'u':
                        try
                        {
                            var code = text.Substring(i, 4);
                            _ = sb.Append((char)Convert.ToInt32(code, 16));
                            i += 4;
                        }
                        catch (Exception ex)
                        {
                            if (throwOnInvalid)
                            {
                                throw new InvalidDataException($"Invalid escape code at '{text.Substring(i - 2, Math.Min(6, text.Length - i))}'.", ex);
                            }

                            _ = sb.Append("\\u");
                        }
                        continue;
                    default:
                        if (throwOnInvalid)
                        {
                            throw new InvalidDataException("Invalid escape code.");
                        }
                        _ = sb.Append('\\');
                        _ = sb.Append(c2);
                        continue;
                }
            }

            _ = sb.Append(c);
        }

        return sb.ToString();
    }

    #endregion Public Methods
}
