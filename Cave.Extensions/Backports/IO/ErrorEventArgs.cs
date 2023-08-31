#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

namespace System.IO
{
    public class ErrorEventArgs : EventArgs
    {
        #region Fields

        readonly Exception exception;

        #endregion Fields

        #region Constructors

        public ErrorEventArgs(Exception exception) => this.exception = exception;

        #endregion Constructors

        #region Members

        public virtual Exception GetException() => exception;

        #endregion Members
    }
}

#endif
