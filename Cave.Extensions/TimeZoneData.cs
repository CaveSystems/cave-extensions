using System;

namespace Cave;

/// <summary>Gets time zone data.</summary>
/// <remarks>Initializes a new instance of the <see cref="TimeZoneData"/> class.</remarks>
/// <param name="description">The description.</param>
/// <param name="name">the name.</param>
/// <param name="offset">the offset.</param>
public sealed class TimeZoneData(string description, string name, TimeSpan offset)
{
    #region Public Properties

    /// <summary>Gets the description of the time zone.</summary>
    public string Description => description;

    /// <summary>Gets the name of the time zone.</summary>
    public string Name => name;

    /// <summary>Gets the current date time for this time zone.</summary>
    public DateTime Now => DateTime.UtcNow + Offset;

    /// <summary>Gets the time zones offset from UTC/GMT.</summary>
    public TimeSpan Offset => offset;

    #endregion Public Properties

    #region Public Methods

    /// <summary>Searches for a timezone by its name.</summary>
    /// <param name="name">The name.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The timezone.</returns>
    public static TimeZoneData Get(string name, TimeSpan offset)
    {
        foreach (var data in TimeZones.GetList())
        {
            if (data.Name == name)
            {
                return new(data.Description, data.Name, data.Offset + offset);
            }
        }
        throw new TimeZoneNotFoundException();
    }

    /// <summary>Parses a TimeZoneData from a string with a time zone name and optional [+/- Offset].</summary>
    /// <param name="text">Zone[+/-Offset].</param>
    /// <returns>The parsed timezone data.</returns>
    public static TimeZoneData? Parse(string text)
    {
        _ = DateTimeParser.ParseTimeZone(text, out var result);
        return result;
    }

    /// <summary>Checks for equality with another object.</summary>
    /// <param name="obj">The other time zone.</param>
    /// <returns>True if the time zones are equal.</returns>
    public override bool Equals(object? obj) => obj is not null && (ToString() == obj.ToString());

    /// <summary>Gets the hashcode of this instance based on the offset from utc.</summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() => Offset.GetHashCode();

    /// <summary>Gets the timezone as UTC+Offset string.</summary>
    /// <returns>The timezone es string.</returns>
    public override string ToString()
    {
        if (Offset == TimeSpan.Zero)
        {
            return Name;
        }

        return Offset > TimeSpan.Zero
            ? $"{Name}+{Offset.Hours,2:00}:{Offset.Minutes,2:00}"
            : $"{Name}-{Offset.Hours,2:00}:{Offset.Minutes,2:00}";
    }

    #endregion Public Methods
}
