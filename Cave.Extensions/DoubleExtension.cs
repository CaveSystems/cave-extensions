#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion

using System;
using System.Globalization;
using System.Threading;

namespace Cave
{
    /// <summary>
    /// Provides extensions for double to decimal conversion
    /// </summary>
    public static class DoubleExtension
    {
        /// <summary>
        /// Removes rouding errors on doubles and converts them to decimals
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="digits">The number of digits needed</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place</returns>
        public static decimal ToDecimal(this double value, int digits)
        {
            if (double.IsNaN(value))
            {
                return decimal.Zero;
            }

            if (double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value) || (value < (double)decimal.MinValue) || (value > (double)decimal.MaxValue))
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
            return ((decimal)Math.Round(value * (double)p)) / p;
        }

        /// <summary>
        /// Removes rouding errors on doubles and converts them to decimals
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="digits">The number of digits needed</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place</returns>
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
            return ((decimal)Math.Round(value * (double)p)) / p;
        }

        /// <summary>
        /// Removes rouding errors on doubles and converts them to decimals
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="multiplier">The multiplier to shift the double to a whole number</param>
        /// <param name="divisor">The divisor to build the decimal from the whole number</param>
        /// <returns>Returns decimal rounded to the leas significant decimal place</returns>
        public static decimal ToDecimal(this double value, decimal multiplier, long divisor)
        {
            return ((decimal)Math.Round(value * divisor)) * multiplier;
        }

        /// <summary>Formats the price.</summary>
        /// <param name="price">The price.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static string FormatPrice(this double price, CultureInfo culture = null)
        {
#if !NETSTANDARD13
            if (culture == null)
            {
                culture = Thread.CurrentThread.CurrentCulture;
            }
#endif

            // maximum 5 digits
            long decimalValue = (long)Math.Round((price % 1) * 100000);
            if (decimalValue % 100 != 0)
            {
                // need all (5) digits
                return price.ToString("N5", culture);
            }
            if (decimalValue % 1000 != 0)
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
    }
}
