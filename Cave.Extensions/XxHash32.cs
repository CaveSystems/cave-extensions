﻿/*
  This implementation is based on the .net 6 HashCode class
  https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/HashCode.cs
  which is based on the code published by Yann Collet:
  https://raw.githubusercontent.com/Cyan4973/xxHash/5c174cfa4e45a42f94082dc0d4539b39696afea1/xxhash.c

  xxHash - Fast Hash algorithm
  Copyright (C) 2012-2016, Yann Collet
  BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)
  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are
  met:
  * Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.
  * Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following disclaimer
  in the documentation and/or other materials provided with the
  distribution.
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  You can contact the author at :
  - xxHash homepage: http://www.xxhash.com
  - xxHash source repository : https://github.com/Cyan4973/xxHash
*/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cave;

/// <summary>Provides a fast hash algorithm without random seed (the .net hashcode class does not calculate deterministic hashes).</summary>
public struct XxHash32 : IHashingFunction
{
    const uint seed = 1667331685U;
    const uint prime1 = 2654435761U;
    const uint prime2 = 2246822519U;
    const uint prime3 = 3266489917U;
    const uint prime4 = 668265263U;
    const uint prime5 = 374761393U;

    uint v1, v2, v3, v4;
    uint q1, q2, q3;
    uint len;

    [MethodImpl((MethodImplOptions)256)]
    static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
    {
        unchecked
        {
            v1 = seed + prime1 + prime2;
            v2 = seed + prime2;
            v3 = seed;
            v4 = seed - prime1;
        }
    }

    [MethodImpl((MethodImplOptions)256)]
    static uint RotateLeft(uint value, int bits) => (value << bits) | (value >> (32 - bits));

    [MethodImpl((MethodImplOptions)256)]
    static uint Round(uint hash, uint input) => RotateLeft(hash + (input * prime2), 13) * prime1;

    [MethodImpl((MethodImplOptions)256)]
    static uint QueueRound(uint hash, uint queuedValue) => RotateLeft(hash + (queuedValue * prime3), 17) * prime4;

    [MethodImpl((MethodImplOptions)256)]
    static uint MixState(uint v1, uint v2, uint v3, uint v4) => RotateLeft(v1, 1) + RotateLeft(v2, 7) + RotateLeft(v3, 12) + RotateLeft(v4, 18);

    [MethodImpl((MethodImplOptions)256)]
    static uint MixEmptyState() => seed + prime5;

    [MethodImpl((MethodImplOptions)256)]
    static uint MixFinal(uint hash)
    {
        unchecked
        {
            hash ^= hash >> 15;
            hash *= prime2;
            hash ^= hash >> 13;
            hash *= prime3;
            hash ^= hash >> 16;
            return hash;
        }
    }

    /// <summary>Add a items hash to the hashcode.</summary>
    /// <typeparam name="T">Type of the item to add (prevents unboxing).</typeparam>
    /// <param name="value">Item to add.</param>
    public void Add<T>(T value)
    {
        unchecked
        {
            // The original xxHash works as follows:
            // 0. Initialize immediately. We can't do this in a struct (no
            //    default ctor).
            // 1. Accumulate blocks of length 16 (4 uints) into 4 accumulators.
            // 2. Accumulate remaining blocks of length 4 (1 uint) into the
            //    hash.
            // 3. Accumulate remaining blocks of length 1 into the hash.

            // There is no need for #3 as this type only accepts ints. _queue1,
            // _queue2 and _queue3 are basically a buffer so that when
            // ToHashCode is called we can execute #2 correctly.

            // We need to initialize the xxHash32 state (_v1 to _v4) lazily (see
            // #0) nd the last place that can be done if you look at the
            // original code is just before the first block of 16 bytes is mixed
            // in. The xxHash32 state is never used for streams containing fewer
            // than 16 bytes.

            // To see what's really going on here, have a look at the Combine
            // methods.

            var val = (uint)(value?.GetHashCode() ?? 0);

            // Storing the value of _length locally shaves of quite a few bytes
            // in the resulting machine code.
            var previousLength = len++;
            var position = previousLength % 4;

            // Switch can't be inlined.

            if (position == 0)
            {
                q1 = val;
            }
            else if (position == 1)
            {
                q2 = val;
            }
            else if (position == 2)
            {
                q3 = val;
            }
            else // position == 3
            {
                if (previousLength == 3)
                {
                    Initialize(out v1, out v2, out v3, out v4);
                }

                v1 = Round(v1, q1);
                v2 = Round(v2, q2);
                v3 = Round(v3, q3);
                v4 = Round(v4, val);
            }
        }
    }

    /// <summary>Returns the resulting hashcode.</summary>
    /// <returns></returns>
    public int ToHashCode()
    {
        unchecked
        {
            // Storing the value of _length locally shaves of quite a few bytes
            // in the resulting machine code.
            var length = len;

            // position refers to the *next* queue position in this method, so
            // position == 1 means that _queue1 is populated; _queue2 would have
            // been populated on the next call to Add.
            var position = length % 4;

            // If the length is less than 4, _v1 to _v4 don't contain anything
            // yet. xxHash32 treats this differently.

            var hash = length < 4 ? MixEmptyState() : MixState(v1, v2, v3, v4);

            // _length is incremented once per Add(Int32) and is therefore 4
            // times too small (xxHash length is in bytes, not ints).

            hash += length * 4;

            // Mix what remains in the queue

            // Switch can't be inlined right now, so use as few branches as
            // possible by manually excluding impossible scenarios (position > 1
            // is always false if position is not > 0).
            if (position > 0)
            {
                hash = QueueRound(hash, q1);
                if (position > 1)
                {
                    hash = QueueRound(hash, q2);
                    if (position > 2)
                    {
                        hash = QueueRound(hash, q3);
                    }
                }
            }

            hash = MixFinal(hash);
            return (int)hash;
        }
    }

#pragma warning disable 0809
    // Obsolete member 'memberA' overrides non-obsolete member 'memberB'.
    // Disallowing GetHashCode and Equals is by design

    // * We decided to not override GetHashCode() to produce the hash code
    //   as this would be weird, both naming-wise as well as from a
    //   behavioral standpoint (GetHashCode() should return the object's
    //   hash code, not the one being computed).

    // * Even though ToHashCode() can be called safely multiple times on
    //   this implementation, it is not part of the contract. If the
    //   implementation has to change in the future we don't want to worry
    //   about people who might have incorrectly used this type.

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <summary>NotSupported</summary>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => throw new NotSupportedException();
#pragma warning restore 0809
}
