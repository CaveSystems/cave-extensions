using System;
using System.Linq;
using System.Text;

namespace Cave.Security
{
    /// <summary>Provides functions for password testing.</summary>
    public static class PasswordTest
    {
        #region Public Methods

        /// <summary>Guesses the complexity (time needed using a brute force attack with a matching character set) of a specified salt or password</summary>
        /// <param name="data">The password or salt.</param>
        /// <returns>Returns the estimated time needed at a specialized system.</returns>
        public static TimeSpan GuessBruteForceTime(string data)
        {
            //in year 2020 we can check 10^14 passwords per second
            var triesPerSecond = Math.Pow(10, 14);
            //assume speed increase of ^2 per year
            triesPerSecond *= Math.Pow(2, DateTime.Now.Year - 2020);

            var maximumTries = GuessBruteForceTries(data);
            var seconds = maximumTries / triesPerSecond;
            var ticks = seconds * TimeSpan.TicksPerSecond;
            if (ticks >= TimeSpan.MaxValue.Ticks) return TimeSpan.MaxValue;
            return new TimeSpan((long)ticks);
        }

        /// <summary>Guesses the complexity (number of tries using a brute force attack with a matching character set) of a specified salt or password</summary>
        /// <param name="data">The password or salt.</param>
        /// <returns>Returns the estimated strength (rounds needed).</returns>
        public static double GuessBruteForceTries(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var charsetCount = 0;
            var charCount = data.Length;

            var hasUpper = data.HasValidChars(ASCII.Strings.UppercaseLetters);
            var hasLower = data.HasValidChars(ASCII.Strings.LowercaseLetters);
            var hasDigit = data.HasValidChars(ASCII.Strings.Digits);
            var isSimple = !data.HasInvalidChars(ASCII.Strings.Letters + ASCII.Strings.Digits);
            var isPrintable = !isSimple && !data.HasInvalidChars(ASCII.Strings.Printable);
            var is7BitAscii = !isPrintable && !data.Any(d => d > 127);

            if (isSimple)
            {
                charsetCount = (hasUpper ? 26 : 0) + (hasLower ? 26 : 0) + (hasDigit ? 10 : 0);
            }
            else if (isPrintable)
            {
                charsetCount = ASCII.Bytes.Printable.Count;
            }
            else if (is7BitAscii)
            {
                charsetCount = 128;
            }
            else
            {
                //select shortest byte representation to be sure
                charsetCount = 256;
                charCount = Math.Min(Encoding.UTF8.GetByteCount(data), Encoding.Unicode.GetByteCount(data));
            }

            return Math.Pow(charsetCount, charCount);
        }

        /// <summary>Guesses the complexity (bit variation strength) of a specified salt or password.</summary>
        /// <param name="data">The password or salt.</param>
        /// <returns>Returns the estimated strength</returns>
        public static int GuessComplexity(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var result = 1;
            for (var i = 1; i < data.Length; i++)
            {
                var diff = Math.Abs(data[0] - data[1]);
                while (diff > 0)
                {
                    diff >>= 1;
                    result++;
                }
            }

            return result;
        }

        #endregion Public Methods
    }
}
