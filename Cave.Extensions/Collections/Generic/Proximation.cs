using System;
using System.Collections.Generic;

namespace Cave.Collections.Generic
{
    /// <summary>
    /// Provides a basic moving average calculated with values based on a time axis (no continuous sampling needed).
    /// </summary>
    public class Proximation
    {
        class ProximationValue
        {
            public DateTime TimeStamp;
            public long Value;

            public ProximationValue(DateTime timeStamp, long value)
            {
                Value = value;
                TimeStamp = timeStamp;
            }

            public ProximationValue(long value)
            {
                Value = value;
                TimeStamp = DateTime.UtcNow;
            }
        }

        LinkedList<ProximationValue> items = new LinkedList<ProximationValue>();

        /// <summary>
        /// Adds a value to the proximation.
        /// </summary>
        /// <param name="value">The value to add.</param>
        public void AddValue(long value)
        {
            items.AddLast(new ProximationValue(value));
            if (value > Maximum)
            {
                Maximum = value;
            }
            else if (value < Minimum)
            {
                Minimum = value;
            }
        }

        /// <summary>Adds a value to the proximation.</summary>
        /// <param name="timeStamp">The time stamp of the value.</param>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">TimeStamp.</exception>
        public void AddValue(DateTime timeStamp, long value)
        {
            if ((items.Count > 0) && (items.Last.Value.TimeStamp >= timeStamp))
            {
                throw new ArgumentOutOfRangeException(nameof(timeStamp));
            }

            items.AddLast(new ProximationValue(timeStamp, value));
            if (value > Maximum)
            {
                Maximum = value;
            }
            else if (value < Minimum)
            {
                Minimum = value;
            }
        }

        /// <summary>
        /// Clears all recorded values.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Clears all values with a specified age or older.
        /// </summary>
        /// <param name="age">The maximum age for values to keep.</param>
        public void ClearOlderThan(TimeSpan age)
        {
            DateTime earliest = EndTime - age;
            while ((items.Count > 0) && (items.First.Value.TimeStamp < earliest))
            {
                items.RemoveFirst();
            }
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public long Maximum { get; private set; }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public long Minimum { get; private set; }

        /// <summary>
        /// Obtains the (local) datetime of the first recorded value.
        /// </summary>
        public DateTime StartTime => (items.Count == 0) ? default(DateTime) : items.First.Value.TimeStamp;

        /// <summary>
        /// Obtains the (local) datetime of the last recorded value.
        /// </summary>
        public DateTime EndTime => (items.Count == 0) ? default(DateTime) : items.First.Value.TimeStamp;

        /// <summary>
        /// Obtains the duration between StartTime and EndTime.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Obtains the current moving average.
        /// </summary>
        public long Average
        {
            get
            {
                if (items.Count == 0)
                {
                    return 0;
                }

                long result = 0;
                foreach (ProximationValue i in items)
                {
                    result += i.Value;
                }
                return result / items.Count;
            }
        }

        /// <summary>
        /// Obtains the weighted average of the whole recorded timeline. Startweight is 0% and endweight is 100%.
        /// </summary>
        public long WeightedAverage
        {
            get
            {
                if (items.Count == 0)
                {
                    return 0;
                }

                long duration = Duration.Ticks;
                double result = 0;
                double div = 0;
                foreach (ProximationValue i in items)
                {
                    long pos = (i.TimeStamp - StartTime).Ticks;
                    long weight = duration / pos;
                    result += i.Value * weight;
                    div += weight;
                }
                return (long)(result / div);
            }
        }

        /// <summary>
        /// Obtains the reverse weighted average of the whole recorded timeline. Startweight is 100% and endweight is 0%.
        /// </summary>
        public long ReverseWeightedAverage
        {
            get
            {
                if (items.Count == 0)
                {
                    return 0;
                }

                long duration = Duration.Ticks;
                double result = 0;
                double div = 0;
                foreach (ProximationValue i in items)
                {
                    long pos = (i.TimeStamp - StartTime).Ticks;
                    long weight = pos / duration;
                    result += i.Value * weight;
                    div += weight;
                }
                return (long)(result / div);
            }
        }
    }
}
