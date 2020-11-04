using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cave.Collections
{
    /// <summary>Gets an <see cref="IEnumerable" /> implementation for simple integer counting.</summary>
    [SuppressMessage("Naming", "CA1710")]
    public class Counter : IEnumerable<int>, IComparable, IEnumerable
    {
        long current;

        /// <summary>Initializes a new instance of the <see cref="Counter" /> class.</summary>
        /// <param name="start">The first value.</param>
        public Counter(int start)
            : this(start, int.MaxValue - Math.Abs(start), 1)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Counter" /> class.</summary>
        /// <param name="start">The first value.</param>
        /// <param name="count">The value count.</param>
        public Counter(int start, int count)
            : this(start, count, 1)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="Counter" /> class.</summary>
        /// <param name="start">The first value.</param>
        /// <param name="count">The value count.</param>
        /// <param name="step">The step between two values.</param>
        public Counter(int start, int count, int step)
        {
            Start = start;
            Count = count;
            End = (Start + Count) - 1;
            Step = step;
            if (Count < 0)
            {
                throw new ArgumentException($"Argument {"Count"} has an invalid value!");
            }

            if (Step < 1)
            {
                throw new ArgumentException($"Argument {"Step"} has an invalid value!");
            }

            Reset();
        }

        /// <summary>Gets the start value of the counter.</summary>
        public int Start { get; }

        /// <summary>Gets the number of steps the counter will move.</summary>
        public int Count { get; }

        /// <summary>Gets the end value of the counter.</summary>
        public int End { get; }

        /// <summary>Gets the step between two values.</summary>
        public int Step { get; }

        /// <summary>Gets the current value.</summary>
        public int Current => 
            current >= Start
            ? current <= End ? (int)current : throw new InvalidOperationException("Invalid operation, moved out of range!")
            : throw new InvalidOperationException("Invalid operation, use MoveNext() first!");

        /// <summary>Gets a value indicating whether the counter was started already or not.</summary>
        public bool Started => current >= Start;

        #region IComparable Member

        /// <summary>Compares the start of two <see cref="Counter" />s.</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var other = obj as Counter;
            return other == null ? -1 : Start.CompareTo(other.Start);
        }

        #endregion

        #region IEnumerable<int> Member

        /// <summary>Gets a <see cref="CountEnumerator" />.</summary>
        /// <returns>Returns a new IEnumerator{int} instance.</returns>
        IEnumerator<int> IEnumerable<int>.GetEnumerator() => new CountEnumerator(this);

        #endregion

        #region IEnumerable Member

        /// <summary>Gets a <see cref="CountEnumerator" />.</summary>
        /// <returns>Returns a new IEnumerator instance.</returns>
        public IEnumerator GetEnumerator() => new CountEnumerator(this);

        #endregion

        /// <summary>Implements the operator ==.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Counter counter1, Counter counter2) =>
            counter1 is null
            ? counter2 is null
            : !(counter2 is null)
&& (counter1.Count == counter2.Count) && (counter1.Start == counter2.Start) && (counter1.End == counter2.End) &&
            (counter1.Step == counter2.Step);
        

        /// <summary>Implements the operator !=.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Counter counter1, Counter counter2) =>
            counter1 is null
            ? !(counter2 is null)
            : counter2 is null
|| (counter1.Count != counter2.Count) || (counter1.Start != counter2.Start) || (counter1.End != counter2.End) ||
            (counter1.Step != counter2.Step);

        /// <summary>Implements the operator &lt;.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Counter counter1, Counter counter2) => counter1 is null || (!(counter2 is null) && counter1.End < counter2.Start);

        /// <summary>Implements the operator &gt;.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Counter counter1, Counter counter2) => counter2 is null || (!(counter1 is null) && counter1.Start > counter2.End);

        /// <summary>Creates a new <see cref="Counter" /> from the specified start and end values.</summary>
        /// <param name="start">The first value.</param>
        /// <param name="end">The last value to be part of the counter.</param>
        /// <returns>Returns a new <see cref="Counter" /> instance.</returns>
        public static Counter Create(int start, int end) => new Counter(start, (end - start) + 1);

        /// <summary>Creates a new <see cref="Counter" /> from the specified start and end values.</summary>
        /// <param name="start">The first value.</param>
        /// <param name="end">The last value.</param>
        /// <param name="step">The step between two values.</param>
        /// <returns>Returns a new <see cref="Counter" /> instance.</returns>
        public static Counter Create(int start, int end, int step) => new Counter(start, (end - start) + 1, step);

        /// <summary>Checks whether a specified value is part of the <see cref="Counter" /> or not.</summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>Returns true if the value is part of the counter.</returns>
        public bool Contains(int value) => value <= End && value >= Start && ((value - Start) % Step) == 0;

        /// <summary>Checks whether a specified <see cref="Counter" /> is part of the <see cref="Counter" /> or not.</summary>
        /// <param name="counter">The <see cref="Counter" /> whose values to be checked.</param>
        /// <returns>Returns true if the specified counter is part of the counter.</returns>
        public bool Contains(Counter counter) =>
            counter == null
            ? throw new ArgumentNullException(nameof(counter))
            : counter.Start >= Start && counter.Start <= End && counter.End <= End && counter.End >= Start && (counter.Step % Step) <= 0;

        /// <summary>Steps to the next value.</summary>
        /// <returns>Returns true if the next value is part of the counter and can be retrieved.</returns>
        public bool MoveNext()
        {
            if (current > End)
            {
                throw new InvalidOperationException("Moving out of range!");
            }

            current += Step;
            return current <= End;
        }

        /// <summary>Resets the counter.</summary>
        public void Reset() => current = Start - Step;

        /// <summary>Checks another <see cref="Counter" /> for equality.</summary>
        /// <param name="obj">The <see cref="Counter" /> instance to check for equality.</param>
        /// <returns>Returns true if the specified object equals this one.</returns>
        public override bool Equals(object obj) => obj is Counter other && (other.Start == Start) && (other.End == End) && (other.Step == Step);

        /// <summary>Gets the counter properties as string.</summary>
        /// <returns>Returns a string representing this object.</returns>
        public override string ToString() => "x = k * " + Step + " | " + (Start - 1L) + " < k < " + (End + 1L);

        /// <summary>Gets a hash code for this instance.</summary>
        /// <returns></returns>
        public override int GetHashCode() => ToString().GetHashCode();
    }
}
