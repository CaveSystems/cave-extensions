#if NET20 || NET35 || NETSTANDARD10
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// Provides support for lazy initialization.
    /// </summary>
    /// <typeparam name="T">Specifies the type of element being lazily initialized.</typeparam>
    /// <remarks>
    /// <para>
    /// By default, all public and protected members of <see cref="Lazy{T}"/> are thread-safe and may be used concurrently from multiple threads. These
    /// thread-safety guarantees may be removed optionally and per instance using parameters to the type's constructors.
    /// </para>
    /// </remarks>
    [ComVisible(false)]
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
    public class Lazy<T>
    {
        #region Private Fields

        Exception createException;
        Func<T> factory;
        object value;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class that uses <typeparamref name="T"/>'s default constructor for lazy initialization.
        /// </summary>
        /// <remarks>An instance created with this constructor may be used concurrently from multiple threads.</remarks>
        public Lazy() => factory = () => (T)Activator.CreateInstance(typeof(T));

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class that uses <typeparamref name="T"/>'s default constructor and a specified thread-safety mode.
        /// </summary>
        /// <param name="isThreadSafe">
        /// true if this instance should be usable by multiple threads concurrently; false if the instance will only be used by one thread at a time.
        /// </param>
        public Lazy(bool isThreadSafe) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class that uses a specified initialization function and a specified thread-safety mode.
        /// </summary>
        /// <param name="valueFactory">The <see cref="T:System.Func{T}"/> invoked to produce the lazily-initialized value when it is needed.</param>
        /// <param name="isThreadSafe">
        /// true if this instance should be usable by multiple threads concurrently; false if the instance will only be used by one thread at a time.
        /// </param>
        /// <exception cref="System.ArgumentNullException"><paramref name="valueFactory"/> is a null reference (Nothing in Visual Basic).</exception>
        public Lazy(Func<T> valueFactory, bool isThreadSafe)
            : this(valueFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class that uses a specified initialization function.
        /// </summary>
        /// <param name="valueFactory">The <see cref="T:System.Func{T}"/> invoked to produce the lazily-initialized value when it is needed.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="valueFactory"/> is a null reference (Nothing in Visual Basic).</exception>
        /// <remarks>An instance created with this constructor may be used concurrently from multiple threads.</remarks>
        public Lazy(Func<T> valueFactory) => factory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="Lazy{T}"/> has been initialized.
        /// </summary>
        /// <value>true if the <see cref="Lazy{T}"/> instance has been initialized; otherwise, false.</value>
        /// <remarks>
        /// The initialization of a <see cref="Lazy{T}"/> instance may result in either a value being produced or an exception being thrown. If an exception
        /// goes unhandled during initialization, <see cref="IsValueCreated"/> will return false.
        /// </remarks>
        public bool IsValueCreated => factory == null && createException == null;

        /// <summary>
        /// Gets the lazily initialized value of the current <see cref="Lazy{T}"/>.
        /// </summary>
        /// <value>The lazily initialized value of the current <see cref="Lazy{T}"/>.</value>
        /// <exception cref="MissingMemberException">
        /// The <see cref="Lazy{T}"/> was initialized to use the default constructor of the type being lazily initialized, and that type does not have a public,
        /// parameterless constructor.
        /// </exception>
        /// <exception cref="MemberAccessException">
        /// The <see cref="Lazy{T}"/> was initialized to use the default constructor of the type being lazily initialized, and permissions to access the
        /// constructor were missing.
        /// </exception>
        /// <remarks>If <see cref="IsValueCreated"/> is false, accessing <see cref="Value"/> will force initialization.</remarks>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                if (factory == null)
                {
                    if (createException != null)
                    {
                        throw new Exception($"{typeof(T)} constructor exception during lazy inititalizer!", createException);
                    }
                    return (T)value;
                }

                lock (this)
                {
                    if (factory == null)
                    {
                        return (T)value;
                    }

                    try
                    {
                        return (T)(value = factory());
                    }
                    catch (Exception ex)
                    {
                        createException = ex;
                        throw;
                    }
                    finally
                    {
                        factory = null;
                    }
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates and returns a string representation of this instance.
        /// </summary>
        /// <returns>The result of calling <see cref="object.ToString"/> on the <see cref="Value"/>.</returns>
        /// <exception cref="T:System.NullReferenceException">The <see cref="Value"/> is null.</exception>
        public override string ToString() => IsValueCreated ? Value.ToString() : null;

        #endregion Public Methods
    }
}
#endif
