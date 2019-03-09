using System;
using System.Collections.Generic;
using System.Text;

namespace Cave
{
    /// <summary>
    /// Backport of enum extensions.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Gets an array containing all single flags set. (flag1, flag4, ..)
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum[] GetFlags<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            var flags = new List<TEnum>();
            var val = Convert.ToInt64(value);
            for (var i = 0; i < 63; i++)
            {
                var check = 1L << i;
                if ((val & check) != 0)
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
        /// Gets a string for all single flags set. ("flag1, flag4, ..").
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetString<TEnum>(this TEnum value)
            where TEnum : struct, IConvertible
        {
            var sb = new StringBuilder();
            var val = Convert.ToInt64(value);
            for (var i = 0; i < 63; i++)
            {
                var check = 1L << i;
                if ((val & check) != 0)
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
        public static TEnum Parse<TEnum>(this string value, TEnum defaultValue = default(TEnum))
            where TEnum : struct, IConvertible
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
        public static bool TryParse<TEnum>(this string value, out TEnum result)
            where TEnum : struct, IConvertible
        {
            return Enum.TryParse(value, true, out result);
        }
#elif NET35 || NET20 || NETCOREAPP13 || NETSTANDARD13
        /// <summary>Tries the parse.</summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static bool TryParse<TEnum>(this string value, out TEnum result)
            where TEnum : struct, IConvertible
        {
            Type t = typeof(TEnum);
            if (value == null)
            {
                result = default(TEnum);
                return false;
            }
            try
            {
                result = (TEnum)Enum.Parse(t, value, true);
                return true;
            }
            catch
            {
                result = default(TEnum);
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
            var test = Convert.ToUInt64(flag);
            return test == (Convert.ToUInt64(value) & test);
        }
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
    }
}
