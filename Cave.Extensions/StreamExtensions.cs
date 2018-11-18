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

using Cave.IO;
using System;
using System.IO;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides extensions to <see cref="Stream"/> implementations
    /// </summary>
    public static class StreamExtensions
    {
        static int s_BlockSize = 64 * 1024;

        /// <summary>
        /// Blocksize to be used on any stream operations. Defaults to 32kb.
        /// </summary>
        public static int BlockSize { get { return s_BlockSize; } set { s_BlockSize = Math.Min(1024, value); } }

        /// <summary>Does a stream copy from source to destination.</summary>
        /// <param name="source">Source stream</param>
        /// <param name="target">Target stream</param>
        /// <param name="length">Number of bytes to copy</param>
        /// <param name="callback">Callback to be called during copy or null</param>
        /// <param name="userItem">The user item.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// source
        /// or
        /// target
        /// </exception>
        public static long CopyBlocksTo(this Stream source, Stream target, long length = -1, ProgressCallback callback = null, object userItem = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");
            long written = 0;
            if (length <= 0)
            {
                if (source.CanSeek)
                {
                    length = source.Length - source.Position; 
                }
            }
            if (length <= 0)
            {
                length = long.MaxValue;
            }
            byte[] buffer = new byte[s_BlockSize];
            while (written < length)
            {
                int block = (int)Math.Min(buffer.Length, length - written);
                if (block == 0) break;

                int read = source.Read(buffer, 0, block);
                if (read <= 0) break;
                target.Write(buffer, 0, read);
                written += read;

                if (callback != null)
                {
                    ProgressEventArgs e = new ProgressEventArgs(userItem, written, read, length, true);
                    callback.Invoke(source, e);
                    if (e.Break) break;
                }
            }
            return written;
        }

        /// <summary>Reads all bytes from the specified stream</summary>
        /// <param name="source">Source stream</param>
        /// <param name="length">The number of bytes to read or -1 to read the whole stream.</param>
        /// <param name="callback">Callback to be called during copy or null</param>
        /// <param name="userItem">The user item.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.EndOfStreamException"></exception>
        public static byte[] ReadAllBytes(this Stream source, long length = -1, ProgressCallback callback = null, object userItem = null)
        {
            //if (length == 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (length <= 0 && source.CanSeek) length = (source.Length - source.Position);
            if (length > 0)
            {
                byte[] buffer = new byte[length];
                int done = 0;
                while (done < length)
                {
                    int read = source.Read(buffer, done, (int)length - done);
                    var e = new ProgressEventArgs(userItem, done, read, length, true);
                    callback?.Invoke(source, e);
                    if (read == -1 || e.Break) break;
                    done += read;
                }
                if (done != length) throw new EndOfStreamException();
                return buffer;
            }
            using (MemoryStream buffer = new MemoryStream())
            {
                CopyBlocksTo(source, buffer, -1, callback, userItem);
                return buffer.ToArray();
            }
        }

        /// <summary>
        /// Reads all bytes from the specified stream
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public static byte[] ReadBlock(this Stream source, int count)
        {
            return ReadBlock(source, count, null);
        }

        /// <summary>Reads a block from the specified stream (nonblocking)</summary>
        /// <param name="source">Source stream</param>
        /// <param name="count">The number of bytes to read</param>
        /// <param name="callback">Callback to be called during copy or null</param>
        /// <param name="userItem">The user item.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static byte[] ReadBlock(this Stream source, int count, ProgressCallback callback, object userItem = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            byte[] buf = new byte[count];
            int pos = 0;
            while (pos < count)
            {
                int size = source.Read(buf, pos, Math.Min(count - pos, s_BlockSize));
                if (size != count)
                {
                    Array.Resize(ref buf, size);
                    return buf;
                }
                pos += size;
                if (callback != null)
                {
                    ProgressEventArgs e = new ProgressEventArgs(userItem, pos, size, count, true);
                    callback.Invoke(source, e);
                    if (e.Break) break;
                }
            }
            return buf;
        }

        /// <summary>Writes an UTF8 string to the stream.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="text">The text.</param>
        /// <exception cref="ArgumentNullException">stream</exception>
        public static void WriteUtf8(this Stream stream, string text)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            byte[] data = Encoding.UTF8.GetBytes(text);
            stream.Write(data, 0, data.Length);
        }
    }
}
