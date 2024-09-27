using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Cave;

/// <summary>Backport of enum extensions.</summary>
public static class EnumExtension
{
    /// <summary>Gets an array containing all single flags set. (flag1, flag4, ..)</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The flags as array.</returns>
    public static TEnum[] GetFlags<TEnum>(this TEnum value)
        where TEnum : struct, IConvertible
    {
        var flags = new List<TEnum>();
        var val = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        for (var i = 0; i < 63; i++)
        {
            var check = 1L << i;
            if ((val & check) != 0)
            {
                if (TryParse($"{check}", out TEnum flag))
                {
                    flags.Add(flag);
                }
            }
        }

        return [.. flags];
    }

    /// <summary>Gets a string for all single flags set. ("flag1, flag4, ..").</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The flags as string.</returns>
    public static string GetString<TEnum>(this TEnum value)
        where TEnum : struct, IConvertible
    {
        var sb = new StringBuilder();
        var val = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        for (var i = 0; i < 63; i++)
        {
            var check = 1L << i;
            if ((val & check) != 0)
            {
                if (sb.Length != 0)
                {
                    _ = sb.Append(", ");
                }

                if (TryParse($"{check}", out TEnum flag))
                {
                    _ = sb.Append(flag.ToString());
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>Parses the specified string for a valid enum value.</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="defaultValue">The default value to be returned if no valid value was found.</param>
    /// <returns>The enum value.</returns>
    public static TEnum Parse<TEnum>(this string value, TEnum defaultValue = default)
        where TEnum : struct, IConvertible
    {
        if (!value.TryParse(out TEnum result))
        {
            result = defaultValue;
        }

        return result;
    }

#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER

    /// <summary>Tries to parse the specified string.</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if the value could be parsed.</returns>
    public static bool TryParse<TEnum>(this string value, out TEnum result)
        where TEnum : struct, IConvertible =>
        Enum.TryParse(value, true, out result);

#elif NET20_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
    /// <summary>Tries the parse.</summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <returns>True if the value could be parsed.</returns>
    public static bool TryParse<TEnum>(this string value, out TEnum result)
        where TEnum : struct, IConvertible
    {
        var t = typeof(TEnum);
        if (value == null)
        {
            result = default;
            return false;
        }
        try
        {
            result = (TEnum)Enum.Parse(t, value, true);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>Determines whether one or more bit fields are set at the enum.</summary>
    /// <param name="value">The enum value.</param>
    /// <param name="flag">The flag.</param>
    /// <returns>True if the flag is set in the value.</returns>
    public static bool HasFlag(this Enum value, IConvertible flag)
    {
        var test = Convert.ToUInt64(flag, CultureInfo.InvariantCulture);
        return test == (Convert.ToUInt64(value, CultureInfo.InvariantCulture) & test);
    }
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
}
