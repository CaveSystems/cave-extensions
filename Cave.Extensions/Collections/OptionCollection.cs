using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a option collection implementation.
    /// </summary>
    [DebuggerDisplay("Count={Count}")]
    public class OptionCollection : IEnumerable<Option>, IEquatable<OptionCollection>
    {
        /// <summary>Parses the specified string.</summary>
        /// <param name="text">The string to parse.</param>
        /// <returns></returns>
        public static OptionCollection Parse(string text)
        {
            return FromStrings(StringExtensions.SplitNewLine(text));
        }

        /// <summary>
        /// Obtains an Array of <see cref="Option"/>s from a <see cref="IDictionary"/>.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static OptionCollection FromDictionary(IDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            List<Option> opts = new List<Option>();
            foreach (DictionaryEntry entry in dictionary)
            {
                opts.Add(Option.FromDictionaryEntry(entry));
            }
            OptionCollection result = new OptionCollection(opts);
            return result;
        }

        /// <summary>Obtains an Array of <see cref="Option" />s from a specified string Array.</summary>
        /// <param name="lines">The strings to obtain Options from.</param>
        /// <param name="ignoreInvalid">if set to <c>true</c> [ignore invalid options].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">texts.</exception>
        public static OptionCollection FromStrings(string[] lines, bool ignoreInvalid = false)
        {
            if (lines == null)
            {
                throw new ArgumentNullException("texts");
            }

            List<Option> opts = new List<Option>();
            foreach (string line in lines)
            {
                if (ignoreInvalid && !Option.IsOption(line))
                {
                    continue;
                }

                opts.Add(Option.Parse(line));
            }
            OptionCollection result = new OptionCollection(opts);
            return result;
        }

        List<string, Option> items = new List<string, Option>();

        /// <summary>Initializes a new empty instance of the <see cref="OptionCollection"/> class.</summary>
        public OptionCollection()
        {
        }

        /// <summary>
        /// Creates a new <see cref="OptionCollection"/>.
        /// </summary>
        /// <param name="enumeration">The <see cref="IEnumerable"/> list of <see cref="Option"/>s.</param>
        public OptionCollection(IEnumerable<Option> enumeration)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException("enumeration");
            }

            foreach (Option option in enumeration)
            {
                items.Add(option.Name, option);
            }
        }

        /// <summary>
        /// Checks whether a specified option name is part of the collection.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <returns></returns>
        public bool Contains(string optionName)
        {
            if (optionName == null)
            {
                throw new ArgumentNullException("optionName");
            }

            return items.ContainsA(optionName);
        }

        /// <summary>
        /// Obtains the index of the first option with the specified name.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <returns>Returns the index of the first option or -1 if no option with the specified name can be found.</returns>
        int IndexOf(string optionName)
        {
            if (optionName == null)
            {
                throw new ArgumentNullException("optionName");
            }

            if (Option.GetPrefix(optionName) != null)
            {
                throw new ArgumentException("Do not prefix the optionname with an option prefix!");
            }

            return items.IndexOfA(optionName);
        }

        /// <summary>
        /// Obtains the index of the first option with the specified name.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="start">Start index to begin search at.</param>
        /// <returns>Returns the index of the first option or -1 if no option with the specified name can be found.</returns>
        int IndexOf(string optionName, int start)
        {
            if (optionName == null)
            {
                throw new ArgumentNullException("optionName");
            }

            if (Option.GetPrefix(optionName) != null)
            {
                throw new ArgumentException("Do not prefix the optionname with an option prefix!");
            }

            return items.IndexOfA(optionName, start);
        }

        /// <summary>
        /// Obtains all option names.
        /// </summary>
        public string[] Names => items.ItemsA;

        /// <summary>
        /// Allows direct access to the first<see cref="Option"/> with the specified name.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Option this[string optionName]
        {
            get
            {
                int index = IndexOf(optionName);
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(optionName));
                }

                return this[index];
            }
        }

        /// <summary>
        /// Allows direct access to the first<see cref="Option"/> with the specified name.
        /// </summary>
        /// <param name="optionIndex">Index of the option.</param>
        /// <returns></returns>
        Option this[int optionIndex] => items.GetB(optionIndex);

        /// <summary>
        /// Obtains a string containing all options.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (Option option in this)
            {
                string optionString = option.ToString();
                if (result.Length > 0)
                {
                    result.Append(" ");
                }

                bool containsSpace = optionString.IndexOf(' ') > -1;
                if (containsSpace)
                {
                    result.Append('"');
                }

                result.Append(optionString);
                if (containsSpace)
                {
                    result.Append('"');
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all items.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Option> GetEnumerator()
        {
            return ((IEnumerable<Option>)items.ItemsB).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all items.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.ItemsB.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the collection contains a specified element by using the default equality comparer.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Option item)
        {
            return items.ContainsB(item);
        }

        /// <summary>
        /// Copies all elements of the collection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Option[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Obtains the number of items present.
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Obtains all options of the collection as one dimensional array.
        /// </summary>
        /// <returns></returns>
        public Option[] ToArray()
        {
            Option[] result = new Option[Count];
            CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(OptionCollection other)
        {
            if (other is null)
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].Equals(other.items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object. (Inherited from Object.)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as OptionCollection);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return items.GetHashCode();
        }
    }
}
