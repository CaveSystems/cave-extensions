using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cave;

/// <summary>Gets string functions.</summary>
public static partial class StringExtensions
{
    #region Private Fields

    const string ValidCharsCasing = ASCII.Strings.Letters + ASCII.Strings.Digits;

    #endregion Private Fields

    #region Public Methods

    /// <summary>Builds a camel case name split at invalid characters and upper case letters. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [Obsolete("Use GetLowerCamelCaseName or GetPascalCase instead!")]
    public static string GetCamelCaseName(this string text) => GetLowerCamelCaseName(text);

    /// <summary>Builds a camel case name split at invalid characters and upper case letters. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="validChars">Valid characters.</param>
    /// <param name="splitter">Character used to split parts.</param>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [Obsolete("Use GetLowerCamelCaseName or GetPascalCase instead!")]
    public static string GetCamelCaseName(this string text, string validChars, char splitter) => GetLowerCamelCaseName(text, validChars, splitter);

    /// <summary>Builds a kebab case name split at invalid characters and upper case letters. Example: this-is-a-kebab-case-name</summary>
    /// <param name="validChars">Valid characters.</param>
    /// <param name="splitter">Character used to split parts.</param>
    /// <param name="text">The text to use.</param>
    /// <returns>A kebab case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetKebabCaseName(this string text, string validChars, char splitter)
    {
        text = text.ReplaceInvalidChars(validChars, $"{splitter}");
        var parts = text.Split(splitter).SelectMany(s => s.SplitCasing());
        return parts.ToArray().JoinKebabCase();
    }

    /// <summary>Builds a camel case name split at invalid characters and upper case letters. Example: this-is-a-kebab-case-name</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetKebabCaseName(this string text) => GetKebabCaseName(text, ValidCharsCasing, '-');

    /// <summary>Builds a camel case name split at invalid characters and upper case letters. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="validChars">Valid characters.</param>
    /// <param name="splitter">Character used to split parts.</param>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetLowerCamelCaseName(this string text, string validChars, char splitter)
    {
        text = text.ReplaceInvalidChars(validChars, $"{splitter}");
        var parts = text.Split(splitter).SelectMany(s => s.SplitCasing());
        return parts.ToArray().JoinLowerCamelCase();
    }

    /// <summary>Builds a camel case name split at invalid characters and upper case letters. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetLowerCamelCaseName(this string text) => GetLowerCamelCaseName(text, ValidCharsCasing, '_');

    /// <summary>Builds a pascal case name split at invalid characters and upper case letters. Example: ThisIsAPascalCaseName</summary>
    /// <param name="validChars">Valid characters.</param>
    /// <param name="splitter">Character used to split parts.</param>
    /// <param name="text">The text to use.</param>
    /// <returns>A pascal case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetPascalCaseName(this string text, string validChars, char splitter)
    {
        text = text.ReplaceInvalidChars(validChars, $"{splitter}");
        var parts = text.Split(splitter).SelectMany(s => s.SplitCasing());
        return parts.ToArray().JoinPascalCase();
    }

    /// <summary>Builds a pascal case name split at invalid characters and upper case letters. Example: ThisIsAPascalCaseName</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>A pascal case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetPascalCaseName(this string text) => GetPascalCaseName(text, ValidCharsCasing, '_');

    /// <summary>Builds a snake case name split at invalid characters and upper case letters. Example: this_is_a_snake_case_name</summary>
    /// <param name="validChars">Valid characters.</param>
    /// <param name="splitter">Character used to split parts.</param>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetSnakeCaseName(this string text, string validChars, char splitter)
    {
        text ??= string.Empty;
        text = text.ReplaceInvalidChars(validChars, $"{splitter}");
        var parts = text.Split(splitter).SelectMany(s => s.SplitCasing());
        return parts.ToArray().JoinSnakeCase();
    }

    /// <summary>Builds a snake case name split at invalid characters and upper case letters. Example: this_is_a_snake_case_name</summary>
    /// <param name="text">The text to use.</param>
    /// <returns>A camel case version of text.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string GetSnakeCaseName(this string text) => GetSnakeCaseName(text, ValidCharsCasing, '_');

    /// <summary>Joins the strings with (upper) camel casing. Example: ThisIsACamelCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joined string.</returns>
    [Obsolete("Use JoinPascalCase() or JoinLowerCamelCase() instead!")]
    public static string JoinCamelCase(this string[] parts, CultureInfo culture = null) => JoinPascalCase(parts, culture);

    /// <summary>Joins the strings with (upper) camel casing. Example: ThisIsACamelCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joined string.</returns>
    [Obsolete("Use JoinPascalCase() or JoinLowerCamelCase() instead!")]
    public static string JoinCamelCase(this IEnumerable parts, CultureInfo culture = null) => JoinPascalCase(parts, culture);

    /// <summary>Joins the strings using kebab case. Example: this-is-a-kebab-case-name</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinKebabCase(this string[] parts, CultureInfo culture = null)
    {
        if ((parts == null) || (parts.Length == 0))
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part.Trim();
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append('-');
            }

            _ = result.Append(t.ToLower(culture));
        }

        return result.ToString();
    }

    /// <summary>Joins the strings using kebab case. Example: this-is-a-kebab-case-name</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinKebabCase(this IEnumerable parts, CultureInfo culture = null)
    {
        if (parts == null)
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part is IFormattable formattable ? formattable.ToString(null, culture) : $"{part}";
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append('_');
            }

            _ = result.Append(t.ToLower(culture));
        }

        return result.ToString();
    }

    /// <summary>Joins the strings with camel casing. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinLowerCamelCase(this string[] parts, CultureInfo culture = null)
    {
        if ((parts == null) || (parts.Length == 0))
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part.Trim();
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append(t[0].ToUpper(culture));
                if (t.Length > 1)
                {
                    _ = result.Append(t[1..].ToLower(culture));
                }
            }
            else
            {
                _ = result.Append(t.ToLower(culture));
            }
        }

        return result.ToString();
    }

    /// <summary>Joins the strings with camel casing. Example: thisIsALowerCamelCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinLowerCamelCase(this IEnumerable parts, CultureInfo culture = null)
    {
        if (parts == null)
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part is IFormattable formattable ? formattable.ToString(null, culture) : $"{part}";
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append(t[0].ToUpper(culture));
                if (t.Length > 1)
                {
                    _ = result.Append(t[1..].ToLower(culture));
                }
            }
            else
            {
                _ = result.Append(t.ToLower(culture));
            }
        }

        return result.ToString();
    }

    /// <summary>Joins the strings with pascal casing. Example: ThisIsAPascalCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joined string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinPascalCase(this string[] parts, CultureInfo culture = null)
    {
        if ((parts == null) || (parts.Length == 0))
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part.Trim();
            if (t.Length < 1)
            {
                continue;
            }

            _ = result.Append(t[0].ToUpper(culture));
            if (t.Length > 1)
            {
                _ = result.Append(t[1..].ToLower(culture));
            }
        }

        return result.ToString();
    }

    /// <summary>Joins the strings with pascal casing. Example: ThisIsAPascalCaseName</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinPascalCase(this IEnumerable parts, CultureInfo culture = null)
    {
        if (parts == null)
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part is IFormattable formattable ? formattable.ToString(null, culture) : $"{part}";
            if (t.Length < 1)
            {
                continue;
            }

            _ = result.Append(t[0].ToUpper(culture));
            if (t.Length > 1)
            {
                _ = result.Append(t[1..].ToLower(culture));
            }
        }

        return result.ToString();
    }

    /// <summary>Joins the strings using snake case.</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinSnakeCase(this string[] parts, CultureInfo culture = null)
    {
        if ((parts == null) || (parts.Length == 0))
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part.Trim();
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append('_');
            }

            _ = result.Append(t.ToLower(culture));
        }

        return result.ToString();
    }

    /// <summary>Joins the strings using snake case.</summary>
    /// <param name="parts">The parts.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The joned string.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string JoinSnakeCase(this IEnumerable parts, CultureInfo culture = null)
    {
        if (parts == null)
        {
            return string.Empty;
        }

        culture ??= CultureInfo.CurrentCulture;

        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var t = part is IFormattable formattable ? formattable.ToString(null, culture) : $"{part}";
            if (t.Length < 1)
            {
                continue;
            }

            if (result.Length > 0)
            {
                _ = result.Append('_');
            }

            _ = result.Append(t.ToLower(culture));
        }

        return result.ToString();
    }

    /// <summary>Splits a string at character casing.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The string array.</returns>
    [Obsolete("Use SplitCasing() instead!")]
    public static string[] SplitCamelCase(this string text) => SplitCasing(text);

    /// <summary>Splits a string at character casing.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The string array.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static string[] SplitCasing(this string text)
    {
        if (text == null)
        {
            return ArrayExtension.Empty<string>();
        }

        var splits = new List<int>();
        var casing = Case.Upper;
        for (var current = 1; current < text.Length; current++)
        {
            var lastCasing = casing;
            var c = text[current];
            casing = char.IsUpper(c) ? Case.Upper : char.IsDigit(c) ? Case.Digit : Case.Default;
            if (casing != lastCasing)
            {
                if (lastCasing == Case.Digit || casing != Case.Default)
                {
                    if (current > 1)
                    {
                        splits.Add(current);
                    }
                }
            }
        }

        return SplitAt(text, splits);
    }

    #endregion Public Methods
}
