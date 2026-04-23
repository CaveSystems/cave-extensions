using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cave.Collections.Generic;

/// <summary>Gets a option collection implementation.</summary>
[DebuggerDisplay("Count={Count}")]
public class OptionCollection : IEnumerable<Option>, IEquatable<OptionCollection>
{
    #region Private Fields

    readonly List<string, Option> items = new();

    #endregion Private Fields

    #region Private Indexers

    /// <summary>Allows direct access to the first <see cref="Option"/> with the specified name.</summary>
    /// <param name="optionIndex">Index of the option.</param>
    /// <returns>Returns the <see cref="Option"/> at the specified index.</returns>
    Option this[int optionIndex] => items.GetB(optionIndex);

    #endregion Private Indexers

    #region Public Constructors

    /// <summary>Initializes a new instance of the <see cref="OptionCollection"/> class.</summary>
    public OptionCollection() { }

    /// <summary>Initializes a new instance of the <see cref="OptionCollection"/> class with the specified enumeration of options.</summary>
    /// <param name="enumeration">The <see cref="IEnumerable"/> list of <see cref="Option"/> s.</param>
    public OptionCollection(IEnumerable<Option> enumeration)
    {
        if (enumeration == null)
        {
            throw new ArgumentNullException(nameof(enumeration));
        }

        foreach (var option in enumeration)
        {
            items.Add(option.Name, option);
        }
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>Gets an empty option collection.</summary>
    public static OptionCollection Empty { get; } = new();

    /// <summary>Gets a value indicating whether the list is readonly or not.</summary>
    public static bool IsReadOnly => true;

    /// <summary>Gets all option names.</summary>
    public IList<string> Names => items.ItemsA;

    /// <summary>Gets the number of items present.</summary>
    public int Count => items.Count;

    #endregion Public Properties

    #region Public Indexers

    /// <summary>Allows direct access to the first <see cref="Option"/> with the specified name.</summary>
    /// <param name="optionName">Name of the option.</param>
    /// <returns>Returns the <see cref="Option"/> with the specified name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the option cannot be found!</exception>
    public Option this[string optionName]
    {
        get
        {
            var index = IndexOf(optionName);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(optionName));
            }

            return this[index];
        }
    }

    #endregion Public Indexers

    #region Public Methods

    /// <summary>Gets an Array of <see cref="Option"/> s from a <see cref="IDictionary"/>.</summary>
    /// <param name="dictionary">The dictionary to convert.</param>
    /// <returns>Returns a new <see cref="OptionCollection"/> containing the result.</returns>
    public static OptionCollection FromDictionary(IDictionary dictionary)
    {
        if (dictionary == null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        var opts = new List<Option>();
        foreach (DictionaryEntry entry in dictionary)
        {
            opts.Add(Option.FromDictionaryEntry(entry));
        }

        var result = new OptionCollection(opts);
        return result;
    }

    /// <summary>Obtains an Array of <see cref="Option"/> s from a specified string Array.</summary>
    /// <param name="lines">The strings to obtain Options from.</param>
    /// <param name="ignoreInvalid">if set to <c>true</c> [ignore invalid options].</param>
    /// <returns>Returns a new <see cref="OptionCollection"/> containing the result.</returns>
    /// <exception cref="ArgumentNullException">texts.</exception>
    public static OptionCollection FromStrings(string[]? lines, bool ignoreInvalid = false)
    {
        if (lines == null)
        {
            throw new ArgumentNullException(nameof(lines));
        }

        var opts = new List<Option>();
        foreach (var line in lines)
        {
            if (ignoreInvalid && !Option.IsOption(line))
            {
                continue;
            }

            opts.Add(Option.Parse(line));
        }

        var result = new OptionCollection(opts);
        return result;
    }

    /// <summary>Parses the specified string.</summary>
    /// <param name="text">The string to parse.</param>
    /// <returns>Returns a new <see cref="OptionCollection"/> containing the parsed options.</returns>
    public static OptionCollection Parse(string text) => FromStrings(text.SplitNewLine());

    /// <summary>Checks whether a specified option name is part of the collection.</summary>
    /// <param name="optionName">Name of the option.</param>
    /// <returns>Returns true if the collection contains the specified option name; otherwise, false.</returns>
    public bool Contains(string optionName)
    {
        if (optionName == null)
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        return items.ContainsA(optionName);
    }

    /// <summary>Determines whether the collection contains a specified element by using the default equality comparer.</summary>
    /// <param name="item">The object to locate in the collection.</param>
    /// <returns>Returns true if the collection contains the specified element; otherwise, false.</returns>
    public bool Contains(Option item) => items.ContainsB(item);

    /// <summary>Copies all elements of the collection to an Array, starting at a particular Array index.</summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(Option[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="other">The object to compare with the current instance.</param>
    /// <returns>Returns true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(OptionCollection? other)
    {
        if (other is null)
        {
            return false;
        }

        if (Count != other.Count)
        {
            return false;
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (!items[i].Equals(other.items[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as OptionCollection);

    /// <inheritdoc/>
    public IEnumerator<Option> GetEnumerator() => items.ItemsB.GetEnumerator();

    /// <inheritdoc/>
    public override int GetHashCode() => items.GetHashCode();

    /// <summary>Gets the index of the first option with the specified name.</summary>
    /// <param name="optionName">Name of the option.</param>
    /// <returns>Returns the index of the first option or -1 if no option with the specified name can be found.</returns>
    public int IndexOf(string optionName)
    {
        if (optionName == null)
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        if (Option.GetPrefix(optionName) != null)
        {
            throw new ArgumentException("Do not prefix the optionname with an option prefix!");
        }

        return items.IndexOfA(optionName);
    }

    /// <summary>Gets the index of the first option with the specified name.</summary>
    /// <param name="optionName">Name of the option.</param>
    /// <param name="start">Start index to begin search at.</param>
    /// <returns>Returns the index of the first option or -1 if no option with the specified name can be found.</returns>
    public int IndexOf(string optionName, int start)
    {
        if (optionName == null)
        {
            throw new ArgumentNullException(nameof(optionName));
        }

        if (Option.GetPrefix(optionName) != null)
        {
            throw new ArgumentException("Do not prefix the optionname with an option prefix!");
        }

        return items.IndexOfA(optionName, start);
    }

    /// <summary>Gets all options of the collection as one dimensional array.</summary>
    /// <returns>An array containing all options in the collection.</returns>
    public Option[] ToArray()
    {
        var result = new Option[Count];
        CopyTo(result, 0);
        return result;
    }

    /// <summary>Gets a string containing all options.</summary>
    /// <returns>A string containing all options in the collection.</returns>
    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var option in this)
        {
            var optionString = option.ToString();
            if (result.Length > 0)
            {
                _ = result.Append(' ');
            }

            var containsSpace = optionString.IndexOf(' ') > -1;
            if (containsSpace)
            {
                _ = result.Append('"');
            }

            _ = result.Append(optionString);
            if (containsSpace)
            {
                _ = result.Append('"');
            }
        }

        return result.ToString();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => items.ItemsB.GetEnumerator();

    #endregion Public Methods
}
