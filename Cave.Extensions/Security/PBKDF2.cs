using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cave.Security;

/// <summary>
/// Implements password-based key derivation functionality, PBKDF2, by using a pseudo-random number generator based on any HMAC
/// algorithm.
/// </summary>
public class PBKDF2 : DeriveBytes
#if NET20 || NET35
  , IDisposable
#endif
{
    #region Private Fields

    HMAC algorithm;

    byte[] buffer;

    int hashNumber;

    int iterations = 1000;

    byte[] salt;

    #endregion Private Fields

    #region Private Constructors

    PBKDF2(HMAC algorithm)
    {
        this.algorithm = algorithm ?? new HMACSHA512();
        CreateSalt();
    }

    #endregion Private Constructors

    #region Private Methods

    void FillBuffer()
    {
        var i = ++hashNumber;
        var s = new byte[salt.Length + 4];
        Buffer.BlockCopy(salt, 0, s, 0, salt.Length);
        s[^4] = (byte)(i >> 24);
        s[^3] = (byte)(i >> 16);
        s[^2] = (byte)(i >> 8);
        s[^1] = (byte)i;
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

    #endregion Private Methods

    #region Public Constructors

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

    #endregion Public Constructors

    #region Public Properties

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

    #endregion Public Properties

    #region Public Methods

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

        return new(password, salt, iterations);
    }

    /// <summary>Guesses the complexity (bit variation strength) of a specified salt or password.</summary>
    /// <param name="data">The password or salt.</param>
    /// <returns>Returns the estimated strength</returns>
    [Obsolete("Use PasswordTest instead.")]
    public static int GuessComplexity(byte[] data) => PasswordTest.GuessComplexity(data);

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

    /// <summary>Creates a new salt.</summary>
    public void CreateSalt() => SetSalt(RNG.GetBytes(32));

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

    #endregion Public Methods

#if NET40_OR_GREATER || NETSTANDARD1_0_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            (algorithm as IDisposable).Dispose();
            algorithm = null;
        }
    }

#else //NET20 || NET35
    /// <summary>Releases the unmanaged resources used by this instance and optionally releases the managed resources.</summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            (algorithm as IDisposable).Dispose();
            algorithm = null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
#endif
}
