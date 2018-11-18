#region CopyRight 2018
/*
    Copyright (c) 2005-2018 Andreas Rohleder (andreas@rohleder.cc)
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
#endregion License
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion Authors & Contributors

using System;

namespace Cave
{
    /// <summary>
    /// Provides information about the current state of a long running progress.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        bool doBreak;

        /// <summary>
        /// User item used at parent function
        /// </summary>
        public object UserItem { get; }

        /// <summary>
        /// Current Position
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Part done between last callback and current.
        /// </summary>
        public int Part { get; }

        /// <summary>
        /// Overall count
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// Gets or sets a value indicating that the current process should break.
        /// </summary>
        public bool Break
        {
            get => doBreak;
            set
            {
                if (value && !CanBreak) throw new InvalidOperationException($"Break operation is not allowed!");
                if (!value && doBreak) throw new InvalidOperationException($"Break flag has already been set!");
                doBreak |= value;
            }
        }

        /// <summary>
        /// Gets a value whether breaking the current operation is possible or not
        /// </summary>
        public bool CanBreak { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ProgressEventArgs"/>
        /// </summary>
        /// <param name="userItem"></param>
        /// <param name="position"></param>
        /// <param name="part"></param>
        /// <param name="count"></param>
        /// <param name="canBreak"></param>
        public ProgressEventArgs(object userItem, long position, int part, long count, bool canBreak)
        {
            CanBreak = canBreak;
            UserItem = userItem;
            Position = position;
            Count = count;
            Part = part;
        }
    }
}
