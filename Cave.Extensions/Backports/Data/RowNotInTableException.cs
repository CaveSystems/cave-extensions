#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
namespace System.Data;

public class RowNotInTableException : DataException
{
    #region Constructors

    public RowNotInTableException() { }

    public RowNotInTableException(string s) : base(s) { }

    public RowNotInTableException(string message, Exception innerException) : base(message, innerException) { }

    #endregion Constructors
}

#endif
