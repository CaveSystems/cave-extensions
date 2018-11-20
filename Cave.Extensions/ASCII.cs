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
using System.Collections.Generic;
using System.IO;

namespace Cave
{
    /// <summary>
    /// Provides access to 7Bit ASCII chars / bytes
    /// </summary>
    public static class ASCII
    {
        #region Static implementation

        /// <summary>
        /// Obtains whether the string contains non 7Bit ASCII chars (!=17..127)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsClean(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            for (int i = 0; i < text.Length; i++)
            {
                int value = text[i];
                if ((value < 17) || (value > 127))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Cleans a string from all non ascii and control characters by replacing invalid chars</summary>
        /// <param name="text">The string to clean</param>
        /// <param name="start">The start index</param>
        /// <param name="count">the length</param>
        /// <param name="minimumCharacter">The minimum character value to keep (defaults to 32 = space).</param>
        /// <param name="replacer">The replacer.</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        public static string Clean(string text, int start = 0, int count = -1, char minimumCharacter = ' ', char replacer = ' ', bool termination = false)
        {
            if (count < 0)
            {
                count = text.Length - start;
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            char[] result = text.ToCharArray(start, count);
            for (int i = 0; i < result.Length; i++)
            {
                int value = result[i];
                if (termination && value == 0)
                {
                    new string(result, 0, i);
                }

                if ((value < minimumCharacter) || (value > 127))
                {
                    result[i] = replacer;
                }
            }
            return new string(result);
        }

        /// <summary>Obtains the bytes for a specified 7Bit ASCII string</summary>
        /// <param name="text">The text.</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static byte[] GetBytes(string text, bool termination = false)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return GetBytes(text, 0, -1, termination);
        }

        /// <summary>Obtains the bytes for a specified 7Bit ASCII string</summary>
        /// <param name="text">String to encode</param>
        /// <param name="start">Startindex at the string to encode</param>
        /// <param name="count">Length in characters to encode</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static byte[] GetBytes(string text, int start, int count, bool termination = false)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            unchecked
            {
                if (count < 0)
                {
                    count = termination ? text.Length + 1 : text.Length;
                }

                byte[] result = new byte[count];
                int c = 0;
                for (int i = start; c < count; i++, c++)
                {
                    uint value = text[i];
                    if (termination && value == 0)
                    {
                        result.GetRange(0, i);
                    }

                    if (value > 127)
                    {
                        throw new InvalidDataException(string.Format("Character '{0}' at index '{1}' is not a valid ASCII character!", text[i], i));
                    }

                    result[c] = (byte)value;
                }
                return result;
            }
        }

        /// <summary>Obtains the string for the specified 7Bit ASCII bytes</summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">bytes</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static string GetString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            return GetString(bytes, 0, bytes.Length);
        }

        /// <summary>Obtains the chars for the specified 7Bit ASCII bytes</summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">bytes</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static char[] GetChars(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            return GetChars(bytes, 0, bytes.Length);
        }

        /// <summary>Obtains the chars for the specified 7Bit ASCII bytes</summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <param name="start">Startindex at the array to decode</param>
        /// <param name="count">Length in bytes to decode</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">bytes</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static char[] GetChars(byte[] bytes, int start, int count, bool termination = false)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            unchecked
            {
                int c = 0;
                char[] result = new char[count];
                for (int i = start; c < count; i++, c++)
                {
                    uint value = bytes[i];
                    if (termination && value == 0)
                    {
                        return result.GetRange(0, i);
                    }

                    if (value > 127)
                    {
                        throw new InvalidDataException(string.Format("Byte '{0}' at index '{1}' is not a valid ASCII character!", bytes[i], i));
                    }

                    result[c] = (char)value;
                }
                return result;
            }
        }

        /// <summary>Obtains the string for the specified 7Bit ASCII bytes</summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <param name="start">Startindex at the array to decode</param>
        /// <param name="count">Length in bytes to decode</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">bytes</exception>
        /// <exception cref="InvalidDataException">Byte '{0}' at index '{1}' is not a valid ASCII character!</exception>
        public static string GetString(byte[] bytes, int start, int count, bool termination = false)
        {
            return new string(GetChars(bytes, start, count, termination));
        }

        /// <summary>
        /// Obtains the string for the specified 7Bit ASCII bytes (0..127).
        /// Any invalid character is replaced by char 255.
        /// </summary>
        /// <param name="bytes">Bytes to decode</param>
        /// <param name="start">Startindex at the array to decode</param>
        /// <param name="count">Length in bytes to decode</param>
        /// <param name="termination">if set to <c>true</c> [obey termination].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">bytes</exception>
        public static string GetCleanString(byte[] bytes, int start = 0, int count = -1, bool termination = false)
        {
            if (count < 0)
            {
                count = bytes.Length - start;
            }

            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            unchecked
            {
                int c = 0;
                char[] result = new char[count];
                for (int i = start; c < count; i++, c++)
                {
                    uint value = bytes[i];
                    if (termination && value == 0)
                    {
                        return new string(result, 0, i);
                    }

                    if ((value < 17) || (value > 127))
                    {
                        value = 255;
                    }

                    result[c] = (char)value;
                }
                return new string(result);
            }
        }

        /// <summary>
        /// Obtains a part of a string
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <param name="start">Start index to begin parsing (use -1 to use index of StartMark)</param>
        /// <param name="startMark">StartMark to check/search for</param>
        /// <param name="endMark">EndMark to search for</param>
        /// <returns></returns>
        public static string GetString(byte[] data, int start, byte startMark, byte endMark)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (start < 0)
            {
                start = Array.IndexOf(data, startMark);
            }

            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("Start index invalid!"), "Start");
            }
            if (data[start] != startMark)
            {
                throw new ArgumentOutOfRangeException(string.Format("Start mark invalid!"), "StartMark");
            }
            int l_End = Array.IndexOf(data, endMark, start + 1);
            if (l_End <= start)
            {
                throw new ArgumentOutOfRangeException(string.Format("End index invalid!"), "End");
            }
            return GetString(data, start + 1, l_End - start - 1);
        }

        /// <summary>Gets the hexadecimal character for the lower nibble.</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static char GetHexChar(int value)
        {
            value = value & 0xF;
            if (value < 10)
            {
                return (char)(value + '0');
            }
            return (char)(value - 10 + 'A');
        }

        /// <summary>
        /// Escapes the character by its hexadecimal representation (<![CDATA[\'x'YY]]> or <![CDATA[\'u'YYYY]]> depending on the charset).
        /// </summary>
        /// <param name="c">The character.</param>
        /// <param name="escapeCharacter">The escape character.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Cannot escape character {0}!</exception>
        public static char[] EscapeHex(char c, char escapeCharacter = '\\')
        {
            if (c < 256)
            {
                return new char[] { escapeCharacter, 'x', GetHexChar(c >> 4), GetHexChar(c) };
            }
            if (c < 65536)
            {
                return new char[] { escapeCharacter, 'u', GetHexChar(c >> 12), GetHexChar(c >> 8), GetHexChar(c >> 4), GetHexChar(c) };
            }
            throw new InvalidOperationException("Cannot escape character {0}!");
        }

        /// <summary>Escapes all invalid characters (newline, tab, ... and everything above us ascii 127)</summary>
        /// <param name="text">The text.</param>
        /// <param name="escapeCharacter">The escape character to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        public static string Escape(string text, char escapeCharacter = '\\')
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            List<char> result = new List<char>();
            foreach (char c in text)
            {
                if (c == escapeCharacter)
                {
                    result.Add(c);
                    result.Add(c);
                    continue;
                }
                switch (c)
                {
                    // bell
                    case '\a': result.Add(escapeCharacter); result.Add('a'); break;

                    // backspace
                    case '\b': result.Add(escapeCharacter); result.Add('b'); break;

                    // formfeed
                    case '\f': result.Add(escapeCharacter); result.Add('f'); break;

                    // tab
                    case '\t': result.Add(escapeCharacter); result.Add('t'); break;

                    // newline
                    case '\n': result.Add(escapeCharacter); result.Add('n'); break;

                    // return
                    case '\r': result.Add(escapeCharacter); result.Add('r'); break;

                    // 11
                    case '\v': result.Add(escapeCharacter); result.Add('v'); break;

                    // backslash
                    case '\\': result.Add(escapeCharacter); result.Add('\\'); break;

                    // textmarker
                    case '"':
                    case '\'':
                    // else
                    default:
                        if (c > 127)
                        {
                            result.AddRange(EscapeHex(c, escapeCharacter));
                        }
                        else
                        {
                            result.Add(c);
                        }
                        break;
                }
            }
            return new string(result.ToArray());
        }

        /// <summary>Reverts a previous <see cref="Escape(string, char)" /></summary>
        /// <param name="text">The text.</param>
        /// <param name="escapeCharacter">The escape character.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">text</exception>
        /// <exception cref="InvalidDataException"></exception>
        public static string Unescape(string text, char escapeCharacter = '\\')
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            List<char> result = new List<char>();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == escapeCharacter)
                {
                    c = text[++i];
                    if (c != escapeCharacter)
                    {
                        switch (text[i])
                        {
                            case 'a': result.Add('\a'); break;
                            case 'b': result.Add('\b'); break;
                            case 'f': result.Add('\f'); break;
                            case 't': result.Add('\t'); break;
                            case 'n': result.Add('\n'); break;
                            case 'r': result.Add('\r'); break;
                            case 'v': result.Add('\v'); break;
                            case '\\': result.Add('\\'); break;
                            case 'x': result.Add((char)Convert.ToInt32(text.Substring(i + 1, 2), 16)); i += 2; break;
                            case 'u': result.Add((char)Convert.ToInt32(text.Substring(i + 1, 4), 16)); i += 4; break;
                            default: throw new InvalidDataException();
                        }
                        continue;
                    }
                }
                result.Add(c);
            }
            return new string(result.ToArray());
        }
        #endregion

        #region Bytes subclass

        /// <summary>
        /// Provides access to ASCII.Bytes
        /// </summary>
        public static class Bytes
        {
            /// <summary>
            /// Gets space [ ]
            /// </summary>
            public const byte Space = 0x20;

            /// <summary>
            /// Gets space and tab [ \t]
            /// </summary>
            public static byte[] Spaces => new byte[] { 0x20, 0x09 };

            /// <summary>
            /// Gets lower case letters [a-z]
            /// </summary>
            public static byte[] LowercaseLetters => new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a };

            /// <summary>
            /// Gets upper case letters [A-Z]
            /// </summary>
            public static byte[] UppercaseLetters => new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a };

            /// <summary>
            /// Gets all letters [A-Z], [a-z]
            /// </summary>
            public static byte[] Letters => new byte[]
            {
                0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75,
                0x76, 0x77, 0x78, 0x79, 0x7a, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50,
                0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a
            };

            /// <summary>
            /// Gets digits [0-9]
            /// </summary>
            public static byte[] Digits => new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39 };

            /// <summary>
            /// Gets non zero digits [1-9]
            /// </summary>
            public static byte[] NonZeroDigits => new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39 };

            /// <summary>
            /// Gets punctuation marks
            /// </summary>
            public static byte[] PunctuationMarks => new byte[] { 0x21, 0x2e, 0x2c, 0x3a, 0x3b, 0x3d, 0x3f };

            /// <summary>
            /// Gets arithmetic operators
            /// </summary>
            public static byte[] ArithmeticOperators => new byte[] { 0x2b, 0x2d, 0x2a, 0x2f, 0x5e };

            /// <summary>
            /// Gets brackets
            /// </summary>
            public static byte[] Brackets => new byte[] { 0x28, 0x29, 0x5b, 0x5d, 0x7b, 0x7d };

            /// <summary>
            /// Gets printable ascii chars
            /// </summary>
            public static byte[] Printable => new byte[]
            {
                0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 0x32, 0x33,
                0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a,
                0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f, 0x60, 0x61,
                0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78,
                0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e
            };

            /// <summary>
            /// Gets the utf8 bom
            /// </summary>
            public static byte[] UTF8BOM => new byte[] { 0xef, 0xbb, 0xbf };

            /// <summary>
            /// Carriage return = 0x13
            /// </summary>
            public const byte CR = 0x0d;

            /// <summary>
            /// Line feed = 0x10
            /// </summary>
            public const byte LF = 0x0a;

            /// <summary>
            /// Gets carriage return &amp; line feed
            /// </summary>
            public static byte[] CRLF => new byte[] { CR, LF };

            /// <summary>
            /// Gets characters for safe names (usable as filesystem item, database item, ...)
            /// </summary>
            public static byte[] SafeName => new byte[]
            {
                0x20, 0x23, 0x28, 0x29, 0x2B, 0x2D, 0x2E, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38,
                0x39, 0x3D, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D,
                0x4E, 0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x5F, 0x61, 0x62,
                0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72,
                0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A
            };
        }
        #endregion

        #region Strings subclass

        /// <summary>
        /// Provides access to ASCII.Strings
        /// </summary>
        public static class Strings
        {
            /// <summary>
            /// space [ ]
            /// </summary>
            public const string Space = " ";

            /// <summary>
            /// space and tab [ \t]
            /// </summary>
            public const string Spaces = " \t";

            /// <summary>
            /// lowercase letters [a-z]
            /// </summary>
            public const string LowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

            /// <summary>
            /// upper case letters [A-Z]
            /// </summary>
            public const string UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            /// <summary>
            /// all letters [A-Z], [a-z]
            /// </summary>
            public const string Letters = UppercaseLetters + LowercaseLetters;

            /// <summary>
            /// all digits [0-9]
            /// </summary>
            [Obsolete("Use Digits instead!")]
            public const string Numbers = "0123456789";

            /// <summary>
            /// digits [0-9]
            /// </summary>
            public const string Digits = "0123456789";

            /// <summary>
            /// non zero digits [1-9]
            /// </summary>
            public const string NonZeroDigits = "123456789";

            /// <summary>
            /// punctuation marks [!.,:;?"]
            /// </summary>
            public const string PunctuationMarks = "!.,:;?";

            /// <summary>
            /// arithmetic operators [+-*/=^]
            /// </summary>
            public const string ArithmeticOperators = "+-*/=^";

            /// <summary>
            /// brackets [()[]{}]
            /// </summary>
            public const string Brackets = "()[]{}";

            /// <summary>
            /// printable 7Bit ASCII chars
            /// </summary>
            public const string Printable = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

            /// <summary>
            /// utf8 bom
            /// </summary>
            public const string UTF8BOM = "\xef\xbb\xbf";

            /// <summary>
            /// Carriage return = 0x13
            /// </summary>
            public const string CR = "\r";

            /// <summary>
            /// Line feed = 0x10
            /// </summary>
            public const string LF = "\n";

            /// <summary>
            /// Carriage return <![CDATA[&]]> line feed
            /// </summary>
            public const string CRLF = "\r\n";

            /// <summary>
            /// Default newline marking
            /// </summary>
            public const string NewLine = CRLF;

            /// <summary>
            /// Characters for safe names (usable as filesystem item, database item, ...)
            /// </summary>
            public const string SafeName = Letters + Digits + " #()+-.=@_";

            /// <summary>
            /// Characters for safe url names RFC 3986
            /// </summary>
            public const string SafeUrlOptions = Letters + Digits + "-._~";
        }
        #endregion
    }
}
