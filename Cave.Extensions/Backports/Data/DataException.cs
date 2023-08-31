#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
namespace System.Data;

public class DataException : Exception
{
    #region Constructors

    public DataException() { }

    public DataException(string s)
        : base(s) { }

    public DataException(string s, Exception innerException)
        : base(s, innerException) { }

    #endregion Constructors
}
#endif
