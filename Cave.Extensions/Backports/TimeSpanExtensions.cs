#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20_OR_GREATER && !NET40_OR_GREATER

using System.Globalization;
using System.Text;

namespace System;

public static class TimeSpanExtensions
{
    static string FormatUser(TimeSpan time, string format)
    {
        StringBuilder sb = new();
        var negative = time < TimeSpan.Zero;
        if (negative) time = -time;
        var inQuote = false;
        var escaped = false;

        int TryReplace(int index)
        {
            var c = format[index];
            var end = index;
            while (end < format.Length && format[end] == c) end++;
            var count = end - index;
            switch (c)
            {
                case '-':
                    if (count != 1) throw new FormatException();
                    if (negative) sb.Append('-');
                    return count;

                case 'd':
                    if (count != 1) throw new FormatException();
                    sb.Append(((int)time.TotalDays).ToString("D1"));
                    return count;

                case 'h':
                    if (count is < 1 or > 2) throw new FormatException();
                    sb.Append(time.Hours.ToString($"D{count}"));
                    return count;

                case 'm':
                    if (count != 2) throw new FormatException();
                    sb.Append(time.Minutes.ToString("D2"));
                    return count;

                case 's':
                    if (count != 2) throw new FormatException();
                    sb.Append(time.Seconds.ToString("D2"));
                    return count;

                case 'F':
                case 'f':
                    if (count > 7) throw new FormatException();
                    var fraction = (time.Ticks % TimeSpan.TicksPerSecond).ToString("0000000");
                    if (c == 'F')
                    {
                        fraction = fraction.TrimEnd('0');
                    }
                    if (count < 7)
                    {
                        sb.Append(fraction.Substring(0, count));
                    }
                    else
                    {
                        count = 7;
                        sb.Append(fraction);
                    }
                    return count;
            }
            return 0;
        }

        int HandleCharsAt(int index)
        {
            if (escaped)
            {
                escaped = false;
                sb.Append(format[index]);
                return 0;
            }
            if (format[index] == '\'')
            {
                inQuote = !inQuote;
                return 0;
            }
            if (format[index] == '\\')
            {
                escaped = true;
                return 0;
            }
            var result = TryReplace(index);
            if (result > 0) return result;
            sb.Append(format[index]);
            return 0;
        }

        for (var i = 0; i < format.Length;)
        {
            var result = HandleCharsAt(i);
            if (result < 1) result = 1;
            i += result;
        }

        return sb.ToString();
    }

    static string FormatKnown(TimeSpan time, string format, IFormatProvider formatProvider)
    {
        var sb = new StringBuilder();
        var ticks = time.Ticks;
        var days = ticks / TimeSpan.TicksPerDay;
        ticks = Math.Abs(ticks);
        time = new TimeSpan(ticks);
        var fraction = ticks % TimeSpan.TicksPerSecond;

        switch (format)
        {
            case "c":
                //international format [-][d'.']hh':'mm':'ss['.'fffffff]
                if (days != 0)
                {
                    sb.Append(days);
                    sb.Append('.');
                }
                sb.Append($"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}");
                if (fraction != 0)
                {
                    sb.Append('.');
                    sb.Append(fraction.ToString("0000000"));
                }
                break;

            case "g":
                // short format [-][d':']h':'mm':'ss[.FFFFFFF]
                if (days != 0)
                {
                    sb.Append(days);
                    sb.Append(':');
                }
                sb.Append($"{time.Hours:0}:{time.Minutes:00}:{time.Seconds:00}");
                if (fraction != 0)
                {
                    sb.Append(formatProvider.GetFormat(typeof(NumberFormatInfo)) is NumberFormatInfo nfi ? nfi.NumberDecimalSeparator : ".");
                    sb.Append(fraction.ToString("0000000").TrimEnd('0'));
                }
                break;

            case "G":
                // culture dependent long format [-]d':'hh':'mm':'ss.fffffff
                sb.Append($"{days}:{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}");
                if (fraction != 0)
                {
                    sb.Append(formatProvider.GetFormat(typeof(NumberFormatInfo)) is NumberFormatInfo nfi ? nfi.NumberDecimalSeparator : ".");
                    sb.Append(fraction.ToString("0000000"));
                }
                break;

            default: throw new NotImplementedException();
        }
        return sb.ToString();
    }

    public static string ToString(this TimeSpan time, string format) => ToString(time, format, CultureInfo.CurrentCulture);

    public static string ToString(this TimeSpan time, string format, IFormatProvider formatProvider)
    {
        if (format == null || format.Length == 0) format = "c";

        // standard formats
        if (format.Length == 1)
        {
            var c = format[0];
            if (c is 't' or 'T') return FormatKnown(time, "c", formatProvider);
            return FormatKnown(time, format, formatProvider);
        }
        return FormatUser(time, format);
    }
}

#endif
