using System;
using System.Collections;
using System.Text;

namespace Cave.Collections;

/// <summary>Gets string based option handling of the form "option=long value text".</summary>
public sealed class Option : IEquatable<Option>
{
    #region Public Fields

    /// <summary>Gets the name of the <see cref="Option"/>.</summary>
    public readonly string Name;

    /// <summary>Gets the used prefix (e.g. null, "", "-" or "--").</summary>
    public readonly string? Prefix;

    /// <summary>Obtains the separator.</summary>
    public readonly string Separator;

    /// <summary>Gets the value of the <see cref="Option"/>.</summary>
    public readonly string? Value;

    #endregion Public Fields

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="Option"/> class.</summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    public Option(string name, string value)
    {
        Name = name;
        Value = value;
        Prefix = null;
        Separator = "=";
    }

    /// <summary>Creates a new <see cref="Option"/> with the specified name and value string.</summary>
    /// <param name="prefix">The identifier of the <see cref="Option"/>.</param>
    /// <param name="name">The name of the <see cref="Option"/>.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="value">The value of the <see cref="Option"/>.</param>
    /// <exception cref="ArgumentNullException">Name.</exception>
    /// <exception cref="ArgumentException"></exception>
    public Option(string? prefix, string name, string separator, string? value)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (separator == null)
        {
            throw new ArgumentNullException(nameof(separator));
        }

        if (!string.IsNullOrEmpty(prefix) && name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Do not prefix the optionname with an option prefix!");
        }

        if (name.IndexOf(separator, StringComparison.OrdinalIgnoreCase) > -1)
        {
            throw new ArgumentException("Option name may not contain the separator!");
        }

        if (name.StartsWith("-", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Option name contains prefix!");
        }

        Prefix = prefix;
        Name = name;
        Separator = separator;
        Value = value?.UnboxText(false).Replace("''", "'");
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>Gets an <see cref="Option"/> from a <see cref="DictionaryEntry"/>.</summary>
    /// <param name="dictionaryEntry"></param>
    /// <returns></returns>
    public static Option FromDictionaryEntry(DictionaryEntry dictionaryEntry) => new(null, $"{dictionaryEntry.Key}", "=", $"{dictionaryEntry.Value}");

    /// <summary>Gets only the option prefix at the beginning null, "-", "--".</summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string? GetPrefix(string option)
    {
        if (string.IsNullOrEmpty(option))
        {
            return null;
        }

        var result = option.UnboxText(false);
        if (result.StartsWith("--", StringComparison.OrdinalIgnoreCase))
        {
            return "--";
        }

        return result[0] == '-' ? "-" : null;
    }

    /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="Option"/>.</summary>
    /// <param name="s">The string.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Option(string s) => Parse(s);

    /// <summary>
    /// Checks whether a string is an option string or not.
    /// <para>The following option types are detected: -name[=value] --name[=value] [...]name=[value] [...]name=["value"] [...]name=['value'].</para>
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static bool IsOption(string option) => IsOption(option, true);

    /// <summary>
    /// Checks whether a string is an option string or not.
    /// <para>The following option types are detected: -name[=value] --name[=value] [...]name=[value] [...]name=["value"] [...]name=['value'].</para>
    /// </summary>
    /// <param name="option"></param>
    /// <param name="allowMissingPrefix"></param>
    /// <returns></returns>
    public static bool IsOption(string option, bool allowMissingPrefix)
    {
        if (string.IsNullOrEmpty(option))
        {
            return false;
        }

        // default option
        if (option[0] == '-')
        {
            return true;
        }

        if (allowMissingPrefix)
        {
            // no, option without marker ? (identified by not beeing boxed and containing an equal sign)
            if ((option.IndexOf('=') > -1) && (option[0] != '"') && (option[0] != '\''))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Gets an <see cref="Option"/> from a specified string.</summary>
    /// <param name="option">The whole option string of the form "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;".</param>
    /// <returns></returns>
    public static Option Parse(string option) => Parse(option, "=");

    /// <summary>Obtains an <see cref="Option"/> from a specified string.</summary>
    /// <param name="option">The whole option string of the form "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;".</param>
    /// <param name="separator">The name value separator.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="option"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="option"/> contains invalid characters.</exception>
    public static Option Parse(string option, string separator)
    {
        if (option == null)
        {
            throw new ArgumentNullException(nameof(option));
        }

        if (option.IndexOfAny(['\r', '\n']) > -1)
        {
            throw new ArgumentOutOfRangeException(nameof(option));
        }

        var index = option.IndexOf(separator, StringComparison.OrdinalIgnoreCase);
        string? prefix;
        string name;
        var value = string.Empty;
        if (index < 0)
        {
            prefix = GetPrefix(option);
            name = option;
        }
        else
        {
            value = option[(index + 1)..];
            name = option[..index];
            prefix = GetPrefix(name);
        }

        name = prefix == null ? name.Trim() : name[prefix.Length..].Trim();
        return new(prefix, name, separator, value.Trim());
    }

    /// <summary>Removes the option prefix at the beginning (-, -- and /).</summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public static string RemovePrefix(string option)
    {
        option = option.UnboxText(false);
        var optionID = GetPrefix(option);
        return optionID == null ? option : option[optionID.Length..];
    }

    /// <summary>Checks another option for equality (the option prefix will be ignored).</summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Option? other) => other is not null && (other.Name == Name) && (other.Value == Value) && (other.Separator == Separator);

    /// <summary>Checks another option for equality (the option prefix will be ignored).</summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as Option);

    /// <summary>Gets a hash code based on the result of <see cref="ToString()"/>.</summary>
    /// <returns></returns>
    public override int GetHashCode() => ToString().GetHashCode();

    /// <summary>Gets a "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;" string for the option or "&lt;Name&gt;" if value is null or empty.</summary>
    /// <returns></returns>
    public override string ToString() => ToString(false);

    /// <summary>Gets a "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;" string.</summary>
    /// <param name="alwaysShowSeaparator">Always add separator even if value is null.</param>
    /// <returns></returns>
    public string ToString(bool alwaysShowSeaparator)
    {
        var result = new StringBuilder();
        if (Prefix != null)
        {
            _ = result.Append(Prefix);
        }

        _ = result.Append(Name);
        var value = Value;
        if (alwaysShowSeaparator || !string.IsNullOrEmpty(value))
        {
            _ = result.Append(Separator);
            if (value != null)
            {
                if (value.Contains(' '))
                {
                    _ = result.Append('\'');
                    _ = result.Append(value.Replace("'", "''"));
                    _ = result.Append('\'');
                }
                else
                {
                    _ = result.Append(value);
                }
            }
        }

        return result.ToString();
    }

    #endregion Public Methods
}
