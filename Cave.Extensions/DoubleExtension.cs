using System;
using System.Globalization;
using System.Threading;

namespace Cave
{
    /// <summary>Gets extensions for double to decimal conversion.</summary>
    public static class DoubleExtension
    {
        #region Static

        /// <summary>Formats the price.</summary>
        /// <param name="price">The price.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The formated string.</returns>
        public static string FormatPrice(this double price, CultureInfo culture = null)
        {
#if !NETSTANDARD13
            if (culture == null)
            {
                culture = Thread.CurrentThread.CurrentCulture;
            }
#endif

            // maximum 5 digits
            var decimalValue = (long)Math.Round(price % 1 * 100000);
            if ((decimalValue % 100) != 0)
            {
                // need all (5) digits
                return price.ToString("N5", culture);
            }

            if ((decimalValue % 1000) != 0)
            {
                // need 3 digits
                return price.ToString("N3", culture);
            }

            if (decimalValue != 0)
            {
                // need 2 digits
                return price.ToString("N2", culture);
            }

            // no digits at all
            return price.ToString("N0", culture);
        }

        /// <summary>Removes rouding errors on doubles and converts them to decimals.</summary>
        /// <param name="value">The double value.</param>
        /// <param name="digits">The number of digits needed.</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place.</returns>
        public static decimal ToDecimal(this double value, int digits)
        {
            if (double.IsNaN(value))
            {
                return decimal.Zero;
            }

            if (double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value) || (value < (double)decimal.MinValue) ||
                (value > (double)decimal.MaxValue))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (digits == 0)
            {
                return (decimal)Math.Round(value);
            }

            if (digits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(digits));
            }

            decimal p = 10;
            while (--digits > 0)
            {
                p *= 10;
            }

            return (decimal)Math.Round(value * (double)p) / p;
        }

        /// <summary>Removes rouding errors on doubles and converts them to decimals.</summary>
        /// <param name="value">The double value.</param>
        /// <param name="multiplier">The multiplier to shift the double to a whole number.</param>
        /// <param name="divisor">The divisor to build the decimal from the whole number.</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place.</returns>
        public static decimal ToDecimal(this double value, decimal multiplier, long divisor) => (decimal)Math.Round(value * divisor) * multiplier;

        /// <summary>Removes rouding errors on doubles and converts them to decimals.</summary>
        /// <param name="value">The double value.</param>
        /// <param name="digits">The number of digits needed.</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place.</returns>
        public static decimal ToDecimalOrDefault(this double value, int digits)
        {
            if (double.IsNaN(value))
            {
                return decimal.Zero;
            }

            if (double.IsNegativeInfinity(value))
            {
                return decimal.Zero;
            }

            if (double.IsPositiveInfinity(value))
            {
                return decimal.Zero;
            }

            if (value < (double)decimal.MinValue)
            {
                return decimal.Zero;
            }

            if (value > (double)decimal.MaxValue)
            {
                return decimal.Zero;
            }

            if (digits == 0)
            {
                return (decimal)Math.Round(value);
            }

            if (digits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(digits));
            }

            decimal p = 10;
            while (--digits > 0)
            {
                p *= 10;
            }

            return (decimal)Math.Round(value * (double)p) / p;
        }

        #endregion
    }
}
