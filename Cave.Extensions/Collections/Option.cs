using System;
using System.Collections;
using System.Text;

namespace Cave.Collections
{
    /// <summary>
    /// Provides string based option handling of the form "option=long value text".
    /// </summary>
    public sealed class Option : IEquatable<Option>
    {
        /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="Option"/>.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Option(string s)
        {
            return Parse(s);
        }

        #region static functions

        /// <summary>
        /// Removes the option prefix at the beginning (-, -- and /).
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string RemovePrefix(string option)
        {
            option = option.UnboxText(false);
            string optionID = GetPrefix(option);
            if (optionID == null)
            {
                return option;
            }

            return option.Substring(optionID.Length);
        }

        /// <summary>
        /// Obtains only the option prefix at the beginning null, "-", "--".
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string GetPrefix(string option)
        {
            if (string.IsNullOrEmpty(option))
            {
                return null;
            }

            string result = option.UnboxText(false);
            if (result.StartsWith("--"))
            {
                return "--";
            }

            if (result[0] == '-')
            {
                return "-";
            }

            return null;
        }

        /// <summary>
        /// Checks whether a string is an option string or not.
        /// <para>
        /// The following option types are detected:
        /// -name[=value] --name[=value] [...]name=[value] [...]name=["value"] [...]name=['value'].
        /// </para>
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool IsOption(string option)
        {
            return IsOption(option, true);
        }

        /// <summary>
        /// Checks whether a string is an option string or not.
        /// <para>
        /// The following option types are detected:
        /// -name[=value] --name[=value] [...]name=[value] [...]name=["value"] [...]name=['value'].
        /// </para>
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

        /// <summary>
        /// Obtains an <see cref="Option"/> from a specified string.
        /// </summary>
        /// <param name="option">The whole option string of the form "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;".</param>
        /// <returns></returns>
        public static Option Parse(string option)
        {
            return Parse(option, "=");
        }

        /// <summary>Obtains an <see cref="Option" /> from a specified string.</summary>
        /// <param name="option">The whole option string of the form "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;".</param>
        /// <param name="separator">The name value separator.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">OptionString.</exception>
        /// <exception cref="ArgumentOutOfRangeException">OptionString.</exception>
        public static Option Parse(string option, string separator)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }

            if (option.IndexOfAny(new char[] { '\r', '\n' }) > -1)
            {
                throw new ArgumentOutOfRangeException(nameof(option));
            }

            int index = option.IndexOf(separator);
            string prefix;
            string name;
            string value = string.Empty;
            if (index < 0)
            {
                prefix = GetPrefix(option);
                name = option;
            }
            else
            {
                value = option.Substring(index + 1);
                name = option.Substring(0, index);
                prefix = GetPrefix(name);
            }
            if (prefix == null)
            {
                name = name.Trim();
            }
            else
            {
                name = name.Substring(prefix.Length).Trim();
            }
            return new Option(prefix, name, separator, value.Trim());
        }

        /// <summary>
        /// Obtains an <see cref="Option"/> from a <see cref="DictionaryEntry"/>.
        /// </summary>
        /// <param name="dictionaryEntry"></param>
        /// <returns></returns>
        public static Option FromDictionaryEntry(DictionaryEntry dictionaryEntry)
        {
            return new Option(null, dictionaryEntry.Key.ToString(), "=", dictionaryEntry.Value.ToString());
        }
        #endregion

        #region constructor

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

        /// <summary>
        /// Creates a new <see cref="Option" /> with the specified name and value string.
        /// </summary>
        /// <param name="prefix">The identifier of the <see cref="Option" />.</param>
        /// <param name="name">The name of the <see cref="Option" />.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="value">The value of the <see cref="Option" />.</param>
        /// <exception cref="ArgumentNullException">Name.</exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        public Option(string prefix, string name, string separator, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (separator == null)
            {
                throw new ArgumentNullException("separator");
            }

            if (!string.IsNullOrEmpty(prefix) && name.StartsWith(prefix))
            {
                throw new ArgumentException("Do not prefix the optionname with an option prefix!");
            }

            if (name.IndexOf(separator) > -1)
            {
                throw new ArgumentException("Option name may not contain the separator!");
            }

            if (name.StartsWith("-"))
            {
                throw new ArgumentException("Option name contains prefix!");
            }

            Prefix = prefix;
            Name = name;
            Separator = separator;
            if (value != null)
            {
                Value = value.UnboxText(false).Replace("''", "'");
            }
        }
        #endregion

        #region public properties

        /// <summary>
        /// Obtains the used prefix (e.g. null, "", "-"  or "--").
        /// </summary>
        public readonly string Prefix;

        /// <summary>
        /// Obtains the name of the <see cref="Option"/>.
        /// </summary>
        public readonly string Name;

        /// <summary>Obtains the separator.</summary>
        public readonly string Separator;

        /// <summary>
        /// Obtains the value of the <see cref="Option"/>.
        /// </summary>
        public readonly string Value;
        #endregion

        #region public functions

        /// <summary>
        /// Provides a "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;" string.
        /// </summary>
        /// <param name="alwaysShowSeaparator">Always add separator even if value is null.</param>
        /// <returns></returns>
        public string ToString(bool alwaysShowSeaparator)
        {
            StringBuilder result = new StringBuilder();
            if (Prefix != null)
            {
                result.Append(Prefix);
            }
            result.Append(Name);
            string value = Value;
            if (alwaysShowSeaparator || !string.IsNullOrEmpty(value))
            {
                result.Append(Separator);
                if (value != null)
                {
                    if (value.IndexOf(' ') >= 0)
                    {
                        result.Append("'");
                        result.Append(value.Replace("'", "''"));
                        result.Append("'");
                    }
                    else
                    {
                        result.Append(value);
                    }
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Checks another option for equality (the option prefix will be ignored).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Option other)
        {
            if (other is null)
            {
                return false;
            }

            return (other.Name == Name) && (other.Value == Value) && (other.Separator == Separator);
        }
        #endregion

        #region overrides

        /// <summary>
        /// Provides a "&lt;Name&gt;&lt;Separator&gt;&lt;Value&gt;" string for the option or "&lt;Name&gt;" if value is null or empty.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Obtains a hash code based on the result of <see cref="ToString()"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Checks another option for equality (the option prefix will be ignored).
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Option);
        }
        #endregion
    }
}
