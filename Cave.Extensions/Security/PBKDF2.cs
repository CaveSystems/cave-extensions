using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cave.Security
{
    /// <summary>
    /// Implements password-based key derivation functionality, PBKDF2, by using a pseudo-random number generator based on any HMAC
    /// algorithm.
    /// </summary>
    public class PBKDF2 : DeriveBytes
#if NET20 || NET35
, IDisposable
#endif
    {
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

        /// <summary>Creates a new instance with the specified HMAC.</summary>
        /// <returns></returns>
        public static PBKDF2 Create(HMAC algorithm) => new(algorithm);

        /// <summary>Creates a new instance using the specified private data containing the password, salt and iterations.</summary>
        /// <param name="data">The private data.</param>
        /// <returns></returns>
        public static PBKDF2 FromPrivate(string data)
        {
            var parts = data?.Split(';') ?? throw new ArgumentNullException(nameof(data));
            if (parts.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            var password = Base64.NoPadding.Decode(parts[0]);
            var salt = Base64.NoPadding.Decode(parts[1]);
            if (!int.TryParse(parts[2], out var iterations))
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            return new PBKDF2(password, salt, iterations);
        }

        int iterations = 1000;
        int hashNumber;
        byte[] salt;
        HMAC algorithm;
        byte[] buffer;

        PBKDF2(HMAC algorithm) => this.algorithm = algorithm ?? new HMACSHA512();

        /// <summary>Initializes a new instance of the <see cref="PBKDF2" /> class using the default <see cref="HMACSHA512" /> algorithm.</summary>
        public PBKDF2() : this(null) { }

        /// <summary>Initializes a new instance of the <see cref="PBKDF2" /> class.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="algorithm">The HMAC algorithm to use. Defaults to <see cref="HMACSHA512" />.</param>
        public PBKDF2(string password, byte[] salt, HMAC algorithm = null) : this(algorithm)
        {
            SetPassword(password);
            SetSalt(salt);
        }

        /// <summary>Initializes a new instance of the <see cref="PBKDF2" /> class.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="algorithm">The HMAC algorithm to use. Defaults to <see cref="HMACSHA512" />.</param>
        public PBKDF2(byte[] password, byte[] salt, HMAC algorithm = null) : this(algorithm)
        {
            SetPassword(password);
            SetSalt(salt);
        }

        /// <summary>Initializes a new instance of the <see cref="PBKDF2" /> class.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations. This value is not checked and allows invalid values!</param>
        /// <param name="algorithm">The HMAC algorithm to use. Defaults to <see cref="HMACSHA512" />.</param>
        public PBKDF2(byte[] password, byte[] salt, int iterations, HMAC algorithm = null) : this(algorithm)
        {
            this.iterations = iterations;
            SetPassword(password);
            SetSalt(salt);
        }

        /// <summary>Initializes a new instance of the <see cref="PBKDF2" /> class.</summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations. This value is not checked and allows invalid values!</param>
        /// <param name="algorithm">The HMAC algorithm to use. Defaults to <see cref="HMACSHA512" />.</param>
        public PBKDF2(string password, byte[] salt, int iterations, HMAC algorithm = null) : this(algorithm)
        {
            this.iterations = iterations;
            SetPassword(password);
            SetSalt(salt);
        }

        void FillBuffer()
        {
            var i = ++hashNumber;
            var s = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, s, 0, salt.Length);
            s[s.Length - 4] = (byte)(i >> 24);
            s[s.Length - 3] = (byte)(i >> 16);
            s[s.Length - 2] = (byte)(i >> 8);
            s[s.Length - 1] = (byte)i;
            // this is like j=0
            var u1 = algorithm.ComputeHash(s);
            var data = u1;
            // so we start at j=1
            for (var j = 1; j < iterations; j++)
            {
                var un = algorithm.ComputeHash(data);
                // xor
                for (var k = 0; k < u1.Length; k++)
                {
                    u1[k] = (byte)(u1[k] ^ un[k]);
                }

                data = un;
            }

            var oldLength = buffer.Length;
            Array.Resize(ref buffer, oldLength + u1.Length);
            u1.CopyTo(buffer, oldLength);
        }

        /// <summary>Gets or sets the iteration count.</summary>
        /// <value>The iteration count.</value>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException">IterationCount &lt; 1000</exception>
        public int IterationCount
        {
            get => iterations;
            set
            {
                if (buffer != null)
                {
                    throw new InvalidOperationException($"Cannot change the {nameof(IterationCount)} after calling GetBytes() the first time!");
                }

                if (value < 1000)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "IterationCount < 1000");
                }

                iterations = value;
            }
        }

        /// <summary>Gets or sets the salt.</summary>
        /// <value>The salt.</value>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException">Salt</exception>
        /// <exception cref="ArgumentException">Salt &lt; 8 bytes</exception>
        public string Private => $"{Base64.NoPadding.Encode(algorithm.Key)};{Base64.NoPadding.Encode(salt)};{IterationCount}";

        /// <summary>Sets the salt.</summary>
        /// <param name="salt">The salt.</param>
        public void SetSalt(string salt) => SetSalt(Encoding.UTF8.GetBytes(salt));

        /// <summary>Sets the salt.</summary>
        /// <param name="salt">The salt.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentException">Salt &lt; 8 bytes;value</exception>
        public void SetSalt(byte[] salt)
        {
            if (buffer != null)
            {
                throw new InvalidOperationException("Cannot change salt after calling GetBytes() the first time!");
            }

            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }

            if (salt.Length < 8)
            {
                throw new ArgumentOutOfRangeException(nameof(salt), "Salt < 8 bytes");
            }

            this.salt = (byte[])salt.Clone();
        }

        /// <summary>Sets the password.</summary>
        /// <param name="password">The password.</param>
        public void SetPassword(string password) => SetPassword(Encoding.UTF8.GetBytes(password));

        /// <summary>Sets the password.</summary>
        /// <param name="password">The password.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentException">Password &lt; 8 bytes;value</exception>
        public void SetPassword(byte[] password)
        {
            if (buffer != null)
            {
                throw new InvalidOperationException("Cannot change password after calling GetBytes() the first time!");
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (salt.Length < 8)
            {
                throw new ArgumentOutOfRangeException(nameof(password), "Password < 8 bytes");
            }

            algorithm.Key = (byte[])password.Clone();
        }

        /// <summary>Returns the next pseudo-random one time pad with the specified number of bytes.</summary>
        /// <param name="cb">Length of the byte buffer to retrieve.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Algorithm</exception>
        /// <exception cref="ArgumentException">Iterations &lt; 1000 or Salt &lt; 8 bytes or Password &lt; 8 bytes</exception>
        /// <exception cref="ArgumentOutOfRangeException">Length</exception>
        public override byte[] GetBytes(int cb)
        {
            if (algorithm == null)
            {
                throw new InvalidDataException("Algorithm unset!");
            }

            if (iterations < 1)
            {
                throw new ArgumentException("Iterations < 1");
            }

            if ((salt == null) | (salt.Length < 8))
            {
                throw new ArgumentException("Salt < 8 bytes");
            }

            if (cb < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cb));
            }

            if (buffer == null)
            {
                buffer = new byte[0];
            }

            //enough data present ?
            while (buffer.Length < cb)
            {
                //fill buffer
                FillBuffer();
            }

            var result = buffer.GetRange(0, cb);
            buffer = buffer.GetRange(cb);
            return result;
        }

        /// <summary>Resets the state of the operation.</summary>
        public override void Reset()
        {
            buffer = null;
            hashNumber = 0;
        }

        /// <summary>Releases the unmanaged resources used by this instance and optionally releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NET50
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                (algorithm as IDisposable).Dispose();
                algorithm = null;
            }
        }

#elif NET20_OR_GREATER || NET35
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (algorithm as IDisposable).Dispose(); algorithm = null;
            }
        }

        /// <summary>
        /// Releases all resources used by the this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
#else
#error NETXX not defined!
#endif
    }
}
