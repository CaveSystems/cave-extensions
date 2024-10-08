using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

#pragma warning disable CA1710

namespace Cave.Collections;

/// <summary>Gets a simple integer range.</summary>
/// <remarks>Creates a new full range with the specified minimum and maximum.</remarks>
/// <param name="min"></param>
/// <param name="max"></param>
public class RangeExpression(int min, int max) : IEquatable<RangeExpression>, IEnumerable<int>, IEnumerable
{
    #region Private Classes

    sealed class RangeEnumerator : IEnumerator<int>, IEnumerator
    {
        #region Private Fields

        readonly RangeExpression range;
        long current;

        #endregion Private Fields

        #region Public Constructors

        public RangeEnumerator(RangeExpression range)
        {
            this.range = range;
            Reset();
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Disposed { get; private set; }

        public int Current
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(nameof(RangeEnumerator));
                }
                if (current < range.Minimum)
                {
                    throw new InvalidOperationException("Invalid operation, use MoveNext() first!");
                }

                if (current > range.Maximum)
                {
                    throw new InvalidOperationException("Invalid operation, moved out of Range!");
                }

                return (int)current;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Dispose() => Disposed = true;

        #endregion Public Methods

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(RangeEnumerator));
            }
            if (current > range.Maximum)
            {
                throw new InvalidOperationException("Invalid operation, use Reset() first!");
            }

            while (!range.Contains((int)++current))
            {
                if (current > range.Maximum)
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(RangeEnumerator));
            }
            current = range.Minimum - 1L;
        }
    }

    #endregion Private Classes

    #region Private Fields

    readonly List<Counter> counters = new();

    readonly List<int> values = new();

    bool allValues;

    string allValuesString = "*";

    string? currentString;

    char rangeSeparator = '-';

    char repetitionSeparator = '/';

    char valueSeparator = ',';

    #endregion Private Fields

    #region Private Methods

    void ParseRange(string text, int minValue, int maxValue)
    {
        var parts = text.Split(new[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            ParseRangePart(part, minValue, maxValue);
        }
    }

    void ParseRangePart(string text, int minValue, int maxValue)
    {
        try
        {
            var start = minValue;
            var end = maxValue;

            if (text == AllValuesString)
            {
                allValues = true;
                return;
            }

            // repetition part .../x
            var parts = text.Split(RepetitionSeparator);
            var repetition = 1;
            switch (parts.Length)
            {
                default: throw new ArgumentException($"Invalid range {text}!");
                case 1: break;
                case 2:
                    repetition = int.Parse(parts[1], CultureInfo.InvariantCulture);
                    break;
            }

            // value part x[-y]/...
            parts = parts[0].Split(RangeSeparator);
            if (parts.Length == 0)
            {
                throw new ArgumentException($"Invalid range {text}!");
            }
            if ((parts.Length == 1) && (repetition == 1))
            {
                Add(int.Parse(parts[0]));
                return;
            }
            if (parts[0] != AllValuesString)
            {
                start = Math.Max(start, int.Parse(parts[0]));
            }
            if (parts.Length > 1)
            {
                end = Math.Max(start, int.Parse(parts[1]));
            }
            Add(Counter.Create(start, end, repetition));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid range string '{text}'.", nameof(text), ex);
        }
    }

    #endregion Private Methods

    #region Public Properties

    /// <summary>Gets or sets the all values string.</summary>
    public string AllValuesString
    {
        get => allValuesString;
        set
        {
            allValuesString = value;
            currentString = null;
        }
    }

    /// <summary>Gets the maximum of the <see cref="RangeExpression"/>.</summary>
    public int Maximum => max;

    /// <summary>Gets the minimum of the <see cref="RangeExpression"/>.</summary>
    public int Minimum => min;

    /// <summary>Gets or sets the range separator.</summary>
    public char RangeSeparator
    {
        get => rangeSeparator;
        set
        {
            rangeSeparator = value;
            currentString = null;
        }
    }

    /// <summary>Gets or sets the repetition separator.</summary>
    public char RepetitionSeparator
    {
        get => repetitionSeparator;
        set
        {
            repetitionSeparator = value;
            currentString = null;
        }
    }

    /// <summary>Gets or sets the value separator.</summary>
    public char ValueSeparator
    {
        get => valueSeparator;
        set
        {
            valueSeparator = value;
            currentString = null;
        }
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Implements the operator !=.</summary>
    /// <param name="range1">The range1.</param>
    /// <param name="range2">The range2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator !=(RangeExpression range1, RangeExpression range2) => range1 is null ? range2 is not null : range2 is null || (range1.AllValuesString != range2.AllValuesString);

    /// <summary>Adds two <see cref="RangeExpression"/> s.</summary>
    /// <param name="range1"></param>
    /// <param name="range2"></param>
    /// <returns></returns>
    public static RangeExpression operator +(RangeExpression range1, RangeExpression range2)
    {
        if (range1 is null)
        {
            throw new ArgumentNullException(nameof(range1));
        }

        if (range2 is null)
        {
            throw new ArgumentNullException(nameof(range2));
        }

        var result = new RangeExpression(Math.Min(range1.Minimum, range2.Minimum), Math.Max(range1.Maximum, range2.Maximum))
        {
            range1,
            range2
        };
        return result;
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="range1">The range1.</param>
    /// <param name="range2">The range2.</param>
    /// <returns>The result of the operator.</returns>
    public static bool operator ==(RangeExpression range1, RangeExpression range2)
    {
        if (range1 is null)
        {
            return range2 is null;
        }

        return range2 is not null && (range1.AllValuesString == range2.AllValuesString);
    }

    /// <summary>Parses a <see cref="RangeExpression"/> from a specified string.</summary>
    /// <param name="text">A <see cref="RangeExpression"/> string.</param>
    /// <param name="min">Minimum value of the <see cref="RangeExpression"/>.</param>
    /// <param name="max">Maximum value of the <see cref="RangeExpression"/>.</param>
    /// <returns></returns>
    public static RangeExpression Parse(string text, int min, int max)
    {
        var result = new RangeExpression(min, max);
        result.Parse(text);
        return result;
    }

    /// <summary>Adds a value to this <see cref="RangeExpression"/>.</summary>
    /// <param name="value"></param>
    public void Add(int value)
    {
        if (allValues)
        {
            return;
        }
        if (Contains(value) || (value < Minimum) || (value > Maximum))
        {
            return;
        }
        currentString = null;
        values.Add(value);
    }

    /// <summary>Adds a <see cref="Counter"/> to this <see cref="RangeExpression"/>.</summary>
    /// <param name="counter"></param>
    public void Add(Counter counter)
    {
        if (allValues)
        {
            return;
        }
        if ((counters.Count > 0) && Contains(counter))
        {
            return;
        }

        if (counter.Start < Minimum)
        {
            throw new ArgumentOutOfRangeException(nameof(counter), $"Counter {counter} undercuts minimum {Minimum}!");
        }
        if (counter.End > Maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(counter), $"Counter {counter} exceeds maximum {Maximum}!");
        }
        currentString = null;
        counters.Add(counter);
    }

    /// <summary>Adds a <see cref="RangeExpression"/> to this <see cref="RangeExpression"/>.</summary>
    /// <param name="range"></param>
    public void Add(RangeExpression range)
    {
        if (range is null)
        {
            throw new ArgumentNullException(nameof(range));
        }

        currentString = null;
        foreach (var c in range.counters)
        {
            if (!Contains(c))
            {
                counters.Add(c);
            }
        }
        foreach (var v in range.values)
        {
            if (!Contains(v))
            {
                values.Add(v);
            }
        }
    }

    /// <summary>Checks whether a specified value is part of the <see cref="RangeExpression"/> or not.</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(int value)
    {
        if (allValues)
        {
            return (value >= Minimum) && (value <= Maximum);
        }

        if (values.Contains(value))
        {
            return true;
        }

        foreach (var counter in counters)
        {
            if (counter.Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Checks whether a specified <see cref="Counter"/> is part of the <see cref="RangeExpression"/> or not.</summary>
    /// <param name="counter"></param>
    /// <returns></returns>
    public bool Contains(Counter counter)
    {
        if (counter is null)
        {
            throw new ArgumentNullException(nameof(counter));
        }

        if (allValues)
        {
            return (counter.Start >= Minimum) && (counter.End <= Maximum);
        }

        foreach (var c in counters)
        {
            if (counter.Contains(c))
            {
                return true;
            }
        }

        foreach (var value in counter)
        {
            if (Contains(value))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public bool Equals(RangeExpression? other) => string.Equals(ToString(), other?.ToString(), StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is RangeExpression range && Equals(range);

    /// <inheritdoc/>
    public override int GetHashCode() => ToString().GetHashCode();

    /// <summary>Parses a string and sets all properties of the <see cref="RangeExpression"/> to the parsed values.</summary>
    /// <param name="text"></param>
    public void Parse(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        counters.Clear();
        values.Clear();
        ParseRange(text, Minimum, Maximum);
    }

    /// <summary>Gets the counter properties as string.</summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (currentString != null)
        {
            return currentString;
        }

        if (counters.Count == 0)
        {
            return AllValuesString;
        }

        counters.Sort();
        var result = new StringBuilder();
        foreach (var counter in counters)
        {
            if (result.Length > 0)
            {
                _ = result.Append(ValueSeparator);
            }

            if (counter.Count <= 1)
            {
                _ = result.Append($"{counter.Start}");
                continue;
            }

            if (counter.Step == 1)
            {
                _ = result.Append($"{counter.Start}{RangeSeparator}{counter.End}");
                continue;
            }

            if (counter.Start == Minimum)
            {
                _ = result.Append(AllValuesString + RepetitionSeparator + counter.Step);
                continue;
            }

            if (counter.End == Maximum)
            {
                _ = result.Append($"{counter.Start}{RepetitionSeparator}{counter.Step}");
                continue;
            }

            _ = result.Append($"{counter.Start}{RangeSeparator}{counter.End}{RepetitionSeparator}{counter.Step}");
        }

        foreach (var value in values)
        {
            if (result.Length > 0)
            {
                _ = result.Append(ValueSeparator);
            }
            _ = result.Append(value);
        }

        currentString = $"{result}";
        return currentString;
    }

    /// <summary>Gets an <see cref="IEnumerator"/>.</summary>
    /// <returns></returns>
    public IEnumerator<int> GetEnumerator() => new RangeEnumerator(this);

    /// <summary>Gets an <see cref="IEnumerator"/>.</summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator() => new RangeEnumerator(this);

    #endregion Public Methods
}
