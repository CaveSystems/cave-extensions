using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cave;

/// <summary>Gets extensions to <see cref="Stream"/> implementations.</summary>
public static class StreamExtensions
{
    #region Private Fields

    static int blockSize = 64 * 1024;

    #endregion Private Fields

    #region Public Properties

    /// <summary>Gets or sets the blocksize to be used on any stream operations. Defaults to 64kb.</summary>
    public static int BlockSize { get => blockSize; set => blockSize = Math.Min(1024, value); }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Does a stream copy from source to destination.</summary>
    /// <param name="source">Source stream.</param>
    /// <param name="target">Target stream.</param>
    /// <param name="length">Number of bytes to copy.</param>
    /// <param name="callback">Callback to be called during copy or null.</param>
    /// <param name="userItem">The user item.</param>
    /// <returns>The number of bytes copied.</returns>
    /// <exception cref="ArgumentNullException">source or target.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static long CopyBlocksTo(this Stream source, Stream target, long length = -1, ProgressCallback? callback = null, object? userItem = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

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

        var buffer = new byte[blockSize];
        while (written < length)
        {
            var block = (int)Math.Min(buffer.Length, length - written);
            if (block == 0)
            {
                break;
            }

            var read = source.Read(buffer, 0, block);
            if (read <= 0)
            {
                break;
            }

            target.Write(buffer, 0, read);
            written += read;
            if (callback != null)
            {
                var e = new ProgressEventArgs(userItem, written, read, length, true);
                callback.Invoke(source, e);
                if (e.Break)
                {
                    break;
                }
            }
        }

        return written;
    }

    /// <summary>Reads all bytes from the specified stream.</summary>
    /// <param name="source">Source stream.</param>
    /// <param name="length">The number of bytes to read or -1 to read the whole stream.</param>
    /// <param name="callback">Callback to be called during copy or null.</param>
    /// <param name="userItem">The user item.</param>
    /// <returns>The bytes read.</returns>
    /// <exception cref="EndOfStreamException">Thrown if the stream can seek but ends before the expected end.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static byte[] ReadAllBytes(this Stream source, long length = -1, ProgressCallback? callback = null, object? userItem = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if ((length <= 0) && source.CanSeek)
        {
            length = source.Length - source.Position;
        }

        if (length > 0)
        {
            var buffer = new byte[length];
            var done = 0;
            while (done < length)
            {
                var read = source.Read(buffer, done, (int)length - done);
                var e = new ProgressEventArgs(userItem, done, read, length, true);
                callback?.Invoke(source, e);
                if ((read == -1) || e.Break)
                {
                    break;
                }

                done += read;
            }

            if (done != length)
            {
                throw new EndOfStreamException();
            }

            return buffer;
        }

        using (var buffer = new MemoryStream())
        {
            _ = CopyBlocksTo(source, buffer, -1, callback, userItem);
            return buffer.ToArray();
        }
    }

    /// <summary>Reads all bytes from the specified stream.</summary>
    /// <param name="source">Source stream.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>The bytes read.</returns>
    [MethodImpl((MethodImplOptions)256)]
    public static byte[] ReadBlock(this Stream source, int count) => ReadBlock(source, count, null);

    /// <summary>Reads a block from the specified stream (nonblocking).</summary>
    /// <param name="source">Source stream.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <param name="callback">Callback to be called during copy or null.</param>
    /// <param name="userItem">The user item.</param>
    /// <returns>The bytes read.</returns>
    /// <exception cref="ArgumentNullException">source.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static byte[] ReadBlock(this Stream source, int count, ProgressCallback? callback, object? userItem = null)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var buf = new byte[count];
        var pos = 0;
        while (pos < count)
        {
            var size = source.Read(buf, pos, Math.Min(count - pos, blockSize));
            if (size != count)
            {
                Array.Resize(ref buf, size);
                return buf;
            }

            pos += size;
            if (callback != null)
            {
                var e = new ProgressEventArgs(userItem, pos, size, count, true);
                callback.Invoke(source, e);
                if (e.Break)
                {
                    break;
                }
            }
        }

        return buf;
    }

    /// <summary>Writes an UTF8 string to the stream.</summary>
    /// <param name="stream">The stream.</param>
    /// <param name="text">The text.</param>
    /// <exception cref="ArgumentNullException">stream.</exception>
    [MethodImpl((MethodImplOptions)256)]
    public static void WriteUtf8(this Stream stream, string text)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        text ??= string.Empty;
        var data = Encoding.UTF8.GetBytes(text);
        stream.Write(data, 0, data.Length);
    }

    #endregion Public Methods
}
