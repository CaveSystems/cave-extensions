#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if NET20 || NET35

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System
{
    [ComVisible(false)]
    [DebuggerDisplay("ThreadSafetyMode={Mode}, IsValueCreated={IsValueCreated}, IsValueFaulted={IsValueFaulted}, Value={ValueForDebugDisplay}")]
    public class Lazy<T>
    {
        #region Fields

        Exception createException;
        Func<T> factory;
        object value;

        #endregion Fields

        #region Constructors

        public Lazy() => factory = () => (T)Activator.CreateInstance(typeof(T));

        public Lazy(bool isThreadSafe) { }

        public Lazy(Func<T> valueFactory, bool isThreadSafe)
            : this(valueFactory) { }

        public Lazy(Func<T> valueFactory) => factory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));

        #endregion Constructors

        #region Properties

        public bool IsValueCreated => (factory == null) && (createException == null);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                if (factory == null)
                {
                    if (createException != null)
                    {
                        throw new($"{typeof(T)} constructor exception during lazy inititalizer!", createException);
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

        #endregion Properties

        #region Overrides

        #region Public Methods

        public override string ToString() => IsValueCreated ? Value.ToString() : null;

        #endregion Public Methods

        #endregion Overrides
    }
}

#endif
