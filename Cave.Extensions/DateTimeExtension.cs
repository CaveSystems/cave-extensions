using System;

namespace Cave
{
    /// <summary>
    /// Date Time extensions.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>Determines whether the specified date is a valid date.</summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <returns><c>true</c> if [is valid date]; otherwise, <c>false</c>.</returns>
        public static bool IsValidDate(int year, int month, int day)
        {
            return year >= 1 && year <= 9999
                && month >= 1 && month <= 12
                && day >= 1 && day <= DateTime.DaysInMonth(year, month);
        }

        /// <summary>Determines whether the specified time is a valid time.</summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <returns><c>true</c> if [is valid time]; otherwise, <c>false</c>.</returns>
        public static bool IsValidTime(int hours, int minutes, int seconds)
        {
            return hours >= 0 && hours < 24
                && minutes >= 0 && minutes < 60
                && seconds >= 0 && seconds < 60;
        }
    }
}
