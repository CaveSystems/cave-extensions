using System;
using System.Collections;
using System.Collections.Generic;

namespace Cave.Collections
{
    /// <summary>
    /// Provides an <see cref="IEnumerable"/> implementation for simple integer counting.
    /// </summary>
    public class Counter : IEnumerable<int>, IComparable, IEnumerable
    {
        /// <summary>Implements the operator ==.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Counter counter1, Counter counter2)
        {
            if (counter1 is null)
            {
                return counter2 is null;
            }

            if (counter2 is null)
            {
                return false;
            }

            return counter1.Count == counter2.Count && counter1.Start == counter2.Start && counter1.End == counter2.End && counter1.Step == counter2.Step;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Counter counter1, Counter counter2)
        {
            if (counter1 is null)
            {
                return !(counter2 is null);
            }

            if (counter2 is null)
            {
                return true;
            }

            return counter1.Count != counter2.Count || counter1.Start != counter2.Start || counter1.End != counter2.End || counter1.Step != counter2.Step;
        }

        /// <summary>Implements the operator &lt;.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(Counter counter1, Counter counter2)
        {
            if (counter1 is null)
            {
                return true;
            }

            if (counter2 is null)
            {
                return false;
            }

            return counter1.End < counter2.Start;
        }

        /// <summary>Implements the operator &gt;.</summary>
        /// <param name="counter1">The c1.</param>
        /// <param name="counter2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(Counter counter1, Counter counter2)
        {
            if (counter2 is null)
            {
                return true;
            }

            if (counter1 is null)
            {
                return false;
            }

            return counter1.Start > counter2.End;
        }

        /// <summary>
        /// Creates a new <see cref="Counter"/> from the specified start and end values.
        /// </summary>
        /// <param name="start">The first value.</param>
        /// <param name="end">The last value to be part of the counter.</param>
        /// <returns>Returns a new <see cref="Counter"/> instance.</returns>
        public static Counter Create(int start, int end)
        {
            return new Counter(start, end - start + 1);
        }

        /// <summary>
        /// Creates a new <see cref="Counter"/> from the specified start and end values.
        /// </summary>
        /// <param name="start">The first value.</param>
        /// <param name="end">The last value.</param>
        /// <param name="step">The step between two values.</param>
        /// <returns>Returns a new <see cref="Counter"/> instance.</returns>
        public static Counter Create(int start, int end, int step)
        {
            return new Counter(start, end - start + 1, step);
        }

        long current;

        /// <summary>
        /// Obtains the start value of the counter.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Obtains the number of steps the counter will move.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Obtains the end value of the counter.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Obtains the step between two values.
        /// </summary>
        public int Step { get; }

        /// <summary>
        /// Creates an new <see cref="Counter"/> with an initial start value. This instance will
        /// count until <see cref="int.MaxValue"/> is reached.
        /// </summary>
        /// <param name="start">The first value.</param>
        public Counter(int start)
            : this(start, int.MaxValue - Math.Abs(start), 1)
        {
        }

        /// <summary>
        /// Creates an new <see cref="CountEnumerator"/> with an initial start value and a maximum count.
        /// </summary>
        /// <param name="start">The first value.</param>
        /// <param name="count">The value count.</param>
        public Counter(int start, int count)
            : this(start, count, 1)
        {
        }

        /// <summary>
        /// Creates an new <see cref="CountEnumerator"/> with an initial start value and a maximum count.
        /// </summary>
        /// <param name="start">The first value.</param>
        /// <param name="count">The value count.</param>
        /// <param name="step">The step between two values.</param>
        public Counter(int start, int count, int step)
        {
            Start = start;
            Count = count;
            End = Start + Count - 1;
            Step = step;
            if (Count < 0)
            {
                throw new ArgumentException(string.Format("Argument {0} has an invalid value!", "Count"));
            }

            if (Step < 1)
            {
                throw new ArgumentException(string.Format("Argument {0} has an invalid value!", "Step"));
            }

            Reset();
        }

        /// <summary>
        /// Checks whether a specified value is part of the <see cref="Counter"/> or not.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <returns>Returns true if the value is part of the counter.</returns>
        public bool Contains(int value)
        {
            if (value > End)
            {
                return false;
            }

            if (value < Start)
            {
                return false;
            }

            return ((value - Start) % Step) == 0;
        }

        /// <summary>
        /// Checks whether a specified <see cref="Counter"/> is part of the <see cref="Counter"/> or not.
        /// </summary>
        /// <param name="counter">The <see cref="Counter"/> whose values to be checked.</param>
        /// <returns>Returns true if the specified counter is part of the counter.</returns>
        public bool Contains(Counter counter)
        {
            if (counter == null)
            {
                throw new ArgumentNullException("counter");
            }

            if (counter.Start < Start)
            {
                return false;
            }

            if (counter.Start > End)
            {
                return false;
            }

            if (counter.End > End)
            {
                return false;
            }

            if (counter.End < Start)
            {
                return false;
            }

            if ((counter.Step % Step) > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Steps to the next value.
        /// </summary>
        /// <returns>Returns true if the next value is part of the counter and can be retrieved.</returns>
        public bool MoveNext()
        {
            if (current > End)
            {
                throw new InvalidOperationException(string.Format("Moving out of range!"));
            }

            current += Step;
            return current <= End;
        }

        /// <summary>
        /// Resets the counter.
        /// </summary>
        public void Reset()
        {
            current = Start - Step;
        }

        /// <summary>
        /// Obtains the current value.
        /// </summary>
        public int Current
        {
            get
            {
                if (current < Start)
                {
                    throw new InvalidOperationException(string.Format("Invalid operation, use MoveNext() first!"));
                }

                if (current > End)
                {
                    throw new InvalidOperationException(string.Format("Invalid operation, moved out of range!"));
                }

                return (int)current;
            }
        }

        /// <summary>
        /// Obtains whether the counter was started already or not.
        /// </summary>
        public bool Started => current >= Start;

        /// <summary>
        /// Checks another <see cref="Counter"/> for equality.
        /// </summary>
        /// <param name="obj">The <see cref="Counter"/> instance to check for equality.</param>
        /// <returns>Returns true if the specified object equals this one.</returns>
        public override bool Equals(object obj)
        {
            Counter other = obj as Counter;
            if (other is null)
            {
                return false;
            }

            return (other.Start == Start) && (other.End == End) && (other.Step == Step);
        }

        /// <summary>
        /// Obtains the counter properties as string.
        /// </summary>
        /// <returns>Returns a string representing this object.</returns>
        public override string ToString() => "x = k * " + Step + " | " + (Start - 1L) + " < k < " + (End + 1L);

        /// <summary>
        /// Obtains a hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #region IEnumerable<int> Member

        /// <summary>
        /// Obtains a <see cref="CountEnumerator"/>.
        /// </summary>
        /// <returns>Returns a new IEnumerator{int} instance.</returns>
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return new CountEnumerator(this);
        }

        #endregion

        #region IEnumerable Member

        /// <summary>
        /// Obtains a <see cref="CountEnumerator"/>.
        /// </summary>
        /// <returns>Returns a new IEnumerator instance.</returns>
        public IEnumerator GetEnumerator()
        {
            return new CountEnumerator(this);
        }

        #endregion

        #region IComparable Member

        /// <summary>
        /// Compares the start of two <see cref="Counter"/>s.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Counter other = obj as Counter;
            if (other == null)
            {
                return -1;
            }

            return Start.CompareTo(other.Start);
        }

        #endregion
    }
}
