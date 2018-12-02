using System;

namespace Cave
{
    /// <summary>
    /// Provides time zone data
    /// </summary>
    public sealed class TimeZoneData
    {
        /// <summary>
        /// Parses a TimeZoneData from a string with a time zone name and optional [+/- Offset]
        /// </summary>
        /// <param name="text">Zone[+/-Offset]</param>
        /// <returns></returns>
        public static TimeZoneData Parse(string text)
        {
            DateTimeParser.ParseTimeZone(text, out TimeZoneData result);
            return result;
        }

        /// <summary>
        /// Searches for a timezone by its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static TimeZoneData Get(string name, TimeSpan offset)
        {
            foreach (TimeZoneData data in TimeZones.GetList())
            {
                if (data.Name == name)
                {
                    return new TimeZoneData(data.Description, data.Name, data.Offset + offset);
                }
            }
#if NET35 || NET40 || NET45 || NET46 || NET47 || NETSTANDARD20
            throw new TimeZoneNotFoundException();
#elif NET20 || NETSTANDARD13
            throw new ArgumentException("Timezone not found!");
#else
#error No code defined for the current framework or NETXX version define missing!
#endif
        }

        /// <summary>
        /// Gets the current date time for this time zone
        /// </summary>
        public DateTime Now => DateTime.UtcNow + Offset;

        /// <summary>
        /// Gets the description of the time zone
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the name of the time zone
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the time zones offset from UTC/GMT
        /// </summary>
        public TimeSpan Offset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneData"/> class.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        public TimeZoneData(string description, string name, TimeSpan offset)
        {
            Name = name;
            Description = description;
            Offset = offset;
        }

        /// <summary>
        /// Obtains the timezone as UTC+Offset string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Offset == TimeSpan.Zero)
            {
                return Name;
            }

            if (Offset > TimeSpan.Zero)
            {
                return string.Format("{0}+{1,2:00}:{2,2:00}", Name, Offset.Hours, Offset.Minutes);
            }
            else
            {
                return string.Format("{0}-{1,2:00}:{2,2:00}", Name, Offset.Hours, Offset.Minutes);
            }
        }

        /// <summary>
        /// Checks for equality with another object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return ToString() == obj.ToString();
        }

        /// <summary>
        /// Obtains the hashcode of this instance based on the offset from utc
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Offset.GetHashCode();
        }
    }
}
