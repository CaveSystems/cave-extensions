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

namespace Cave.Text
{
    /// <summary>
    /// Provides a string search result containing start index an length
    /// </summary>
    public struct SubStringResult
    {
        /// <summary>
        /// Checks two SubStringResult instances for equality
        /// </summary>
        /// <param name="subStringResult1"></param>
        /// <param name="subStringResult2"></param>
        /// <returns></returns>
        public static bool operator ==(SubStringResult subStringResult1, SubStringResult subStringResult2)
        {
            if (ReferenceEquals(subStringResult1, null))
            {
                return ReferenceEquals(subStringResult2, null);
            }

            if (ReferenceEquals(subStringResult2, null))
            {
                return false;
            }

            return subStringResult1.Equals(subStringResult2);
        }

        /// <summary>
        /// Checks two SubStringResult instances for inequality
        /// </summary>
        /// <param name="subStringResult1"></param>
        /// <param name="subStringResult2"></param>
        /// <returns></returns>
        public static bool operator !=(SubStringResult subStringResult1, SubStringResult subStringResult2)
        {
            if (ReferenceEquals(subStringResult1, null))
            {
                return !ReferenceEquals(subStringResult2, null);
            }

            if (ReferenceEquals(subStringResult2, null))
            {
                return true;
            }

            return !subStringResult1.Equals(subStringResult2);
        }

        /// <summary>
        /// Searches for a sub string at the specified string
        /// </summary>
        /// <param name="text">Full string</param>
        /// <param name="value">Substring to be searched</param>
        /// <returns></returns>
        public static SubStringResult Find(string text, string value)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            int index = text.IndexOf(value);
            if (index < 0)
            {
                return default(SubStringResult);
            }

            return new SubStringResult(text, index, value.Length);
        }

        /// <summary>
        /// Searches for a sub string at the specified string
        /// </summary>
        /// <param name="text">Full string</param>
        /// <param name="value">Substring to be searched</param>
        /// <param name="startIndex">Startindex to begin searching at</param>
        /// <returns></returns>
        public static SubStringResult Find(string text, string value, int startIndex)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            int index = text.IndexOf(value, startIndex);
            return new SubStringResult(text, index, index < 0 ? 0 : value.Length);
        }

        /// <summary>
        /// Creates a new instance from the specified start and end indices
        /// </summary>
        /// <param name="text">Full string</param>
        /// <param name="startIndex">start index</param>
        /// <param name="endIndex">end index</param>
        /// <returns></returns>
        public static SubStringResult FromIndices(string text, int startIndex, int endIndex)
        {
            return new SubStringResult(text, startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Creates new string bounds with the specified data
        /// </summary>
        /// <param name="text">The String contining the Substring</param>
        /// <param name="index">Start index of the substring</param>
        /// <param name="count">Length of the substring</param>
        public SubStringResult(string text, int index, int count)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            Index = index;
            Length = count;
            EndIndex = Index + Length;
            Text = text.Substring(Index, Length);
        }

        /// <summary>
        /// Checks whether an index is part of this substring or not
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Contains(int index)
        {
            return (index >= Index) && (index < EndIndex);
        }

        /// <summary>
        /// Obtains whether the substring is valid or not
        /// </summary>
        public bool Valid => Length > 0;

        /// <summary>
        /// Substring Text
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Index of the sub string
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Length of the sub string
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Endindex (Index+Length)
        /// </summary>
        public readonly int EndIndex;

        /// <summary>
        /// Obtains a string "[Length] Start..End 'Text'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Index < 0)
            {
                return "Invalid";
            }

            return "[" + Length + "] " + Index + ".." + EndIndex + " = '" + Text + "'";
        }

        /// <summary>
        /// Obtains the hash code for this instance
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Index ^ Length ^ Text.GetHashCode();
        }

        /// <summary>
        /// Checks this instance with another one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is SubStringResult)
            {
                SubStringResult other = (SubStringResult)obj;
                return (other.Index == Index) && (other.Length == Length) && (other.Text == Text);
            }
            return false;
        }
    }
}
