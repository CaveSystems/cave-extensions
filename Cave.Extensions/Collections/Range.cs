using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Cave.Collections
{
    /// <summary>Gets a simple integer range.</summary>
    [SuppressMessage("Naming", "CA1710")]
    public class Range : IEquatable<Range>, IEnumerable<int>, IEnumerable
    {
        #region static functionality

        /// <summary>Parses a <see cref="Range" /> from a specified string.</summary>
        /// <param name="text">A <see cref="Range" /> string.</param>
        /// <param name="min">Minimum value of the <see cref="Range" />.</param>
        /// <param name="max">Maximum value of the <see cref="Range" />.</param>
        /// <returns></returns>
        public static Range Parse(string text, int min, int max)
        {
            var result = new Range(min, max);
            result.Parse(text);
            return result;
        }

        #endregion

        #region operators

        /// <summary>Implements the operator ==.</summary>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Range range1, Range range2)
        {
            if (range1 is null)
            {
                return range2 is null;
            }

            if (range2 is null)
            {
                return false;
            }

            return range1.AllValuesString == range2.AllValuesString;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="range1">The range1.</param>
        /// <param name="range2">The range2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Range range1, Range range2)
        {
            if (range1 is null)
            {
                return !(range2 is null);
            }

            if (range2 is null)
            {
                return true;
            }

            return range1.AllValuesString != range2.AllValuesString;
        }

        /// <summary>Adds two <see cref="Range" />s.</summary>
        /// <param name="range1"></param>
        /// <param name="range2"></param>
        /// <returns></returns>
        public static Range operator +(Range range1, Range range2)
        {
            if (range1 == null)
            {
                throw new ArgumentNullException(nameof(range1));
            }

            if (range2 == null)
            {
                throw new ArgumentNullException(nameof(range2));
            }

            var result = new Range(Math.Min(range1.Minimum, range2.Minimum), Math.Max(range1.Maximum, range2.Maximum)) { range1, range2 };
            return result;
        }

        #endregion

        #region private functionality

        readonly List<Counter> counters = new List<Counter>();
        string currentString;
        string allValuesString = "*";
        char rangeSeparator = '-';
        char valueSeparator = ',';
        char repetitionSeparator = '/';

        Counter ParseRangePart(string text, int minValue, int maxValue)
        {
            try
            {
                if (text.IndexOf(RangeSeparator) > -1)
                {
                    var parts = text.Split(RangeSeparator);
                    if (parts.Length != 2)
                    {
                        throw new ArgumentException("Expected 'start-end'!", nameof(text));
                    }

                    var start = int.Parse(parts[0], CultureInfo.InvariantCulture);
                    if (start < minValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(minValue));
                    }

                    return Counter.Create(start, int.Parse(parts[1], CultureInfo.InvariantCulture));
                }

                if (text.IndexOf(RepetitionSeparator) > -1)
                {
                    var parts = text.Split(RepetitionSeparator);
                    if (parts.Length != 2)
                    {
                        throw new ArgumentException("Expected 'start/repetition'!", nameof(text));
                    }

                    if (parts[0] == AllValuesString)
                    {
                        return Counter.Create(minValue, maxValue, int.Parse(parts[1],CultureInfo.InvariantCulture));
                    }

                    var start = int.Parse(parts[0], CultureInfo.InvariantCulture);
                    if (start < minValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(minValue));
                    }

                    return Counter.Create(start, maxValue, int.Parse(parts[1], CultureInfo.InvariantCulture));
                }

                if (text == AllValuesString)
                {
                    return null;
                }

                {
                    var start = int.Parse(text, CultureInfo.InvariantCulture);
                    if (start < minValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(minValue));
                    }

                    return new Counter(start, 1);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid range string '{text}'.", nameof(text), ex);
            }
        }

        Counter[] ParseRange(string text, int minValue, int maxValue)
        {
            var allValueRangePart = false;
            var result = new List<Counter>();
            var parts = text.Split(new[] { ValueSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var counter = ParseRangePart(part, minValue, maxValue);
                if (counter == null)
                {
                    allValueRangePart = true;
                }
                else
                {
                    if (allValueRangePart)
                    {
                        throw new ArgumentException("You may not add an all-value-range and a normal range!");
                    }

                    result.Add(counter);
                }
            }

            return result.ToArray();
        }

        #endregion

        #region public functionality

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

        /// <summary>Gets the minimum of the <see cref="Range" />.</summary>
        public int Minimum { get; }

        /// <summary>Gets the maximum of the <see cref="Range" />.</summary>
        public int Maximum { get; }

        /// <summary>Creates a new full range with the specified minimum and maximum.</summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public Range(int min, int max)
        {
            Minimum = min;
            Maximum = max;
        }

        /// <summary>Parses a string and sets all properties of the <see cref="Range" /> to the parsed values.</summary>
        /// <param name="text"></param>
        public void Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            this.counters.Clear();
            var counters = ParseRange(text, Minimum, Maximum);
            foreach (var counter in counters)
            {
                Add(counter);
            }
        }

        /// <summary>Adds a value to this <see cref="Range" />.</summary>
        /// <param name="value"></param>
        public void Add(int value)
        {
            if (Contains(value))
            {
                return;
            }

            counters.Add(new Counter(value, 1));
        }

        /// <summary>Adds a <see cref="Counter" /> to this <see cref="Range" />.</summary>
        /// <param name="counter"></param>
        public void Add(Counter counter)
        {
            if ((counters.Count > 0) && Contains(counter))
            {
                return;
            }

            currentString = null;
            counters.Add(counter);
        }

        /// <summary>Adds a <see cref="Range" /> to this <see cref="Range" />.</summary>
        /// <param name="range"></param>
        public void Add(Range range)
        {
            if (range == null)
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
        }

        /// <summary>Checks whether a specified value is part of the <see cref="Range" /> or not.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(int value)
        {
            if (counters.Count == 0)
            {
                return (value >= Minimum) && (value <= Maximum);
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

        /// <summary>Checks whether a specified <see cref="Counter" /> is part of the <see cref="Range" /> or not.</summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        public bool Contains(Counter counter)
        {
            if (counter == null)
            {
                throw new ArgumentNullException(nameof(counter));
            }

            if (counters.Count == 0)
            {
                return Contains(counter.Start) && Contains(counter.End);
            }

            foreach (var c in counters)
            {
                if (counter.Contains(c))
                {
                    return true;
                }
            }

            foreach (Counter c in counter)
            {
                if (Contains(c))
                {
                    return true;
                }
            }

            return false;
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
                    result.Append(ValueSeparator);
                }

                if (counter.Start < Minimum)
                {
                    return "Invalid";
                }

                if (counter.Count > 1)
                {
                    if (counter.Step != 1)
                    {
                        if (counter.Start == Minimum)
                        {
                            return AllValuesString + RepetitionSeparator + counter.Step;
                        }

                        result.Append($"{counter.Start}{RepetitionSeparator}{counter.Step}");
                        continue;
                    }

                    result.Append($"{counter.Start}{RangeSeparator}{counter.End}");
                    continue;
                }

                result.Append($"{counter.Start}");
            }

            currentString = $"{result}";
            return currentString;
        }

        /// <inheritdoc />
        public override int GetHashCode() => ToString().GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Range range ? Equals(range) : false;

        #endregion

        #region IEnumerable Member

        class RangeEnumerator : IEnumerator<int>, IEnumerator
        {
            readonly Range range;
            long current;

            public RangeEnumerator(Range range)
            {
                this.range = range;
                Reset();
            }

            #region IEnumerator Member

            public int Current
            {
                get
                {
                    if (current < range.Minimum)
                    {
                        throw new InvalidOperationException("Invalid operation, use MoveNext() first!");
                    }

                    if (current > range.Maximum)
                    {
                        throw new InvalidOperationException("Invalid operation, moved out of range!");
                    }

                    return (int) current;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() => throw new NotImplementedException();

            public bool MoveNext()
            {
                if (current > range.Maximum)
                {
                    throw new InvalidOperationException("Invalid operation, use Reset() first!");
                }

                while (!range.Contains((int) ++current))
                {
                    if (current > range.Maximum)
                    {
                        return false;
                    }
                }

                return true;
            }

            public void Reset() => current = range.Minimum - 1L;

            #endregion
        }

        /// <summary>Gets an <see cref="IEnumerator" />.</summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => new RangeEnumerator(this);

        /// <summary>Gets an <see cref="IEnumerator" />.</summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator() => new RangeEnumerator(this);

        /// <inheritdoc />
        public bool Equals(Range other) => string.Equals(ToString(), other?.ToString(), StringComparison.OrdinalIgnoreCase);

        #endregion
    }
}
