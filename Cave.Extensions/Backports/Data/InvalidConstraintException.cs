#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
namespace System.Data;

public class InvalidConstraintException : DataException
{
    #region Constructors

    public InvalidConstraintException() { }

    public InvalidConstraintException(string s) : base(s) { }

    public InvalidConstraintException(string message, Exception innerException) : base(message, innerException) { }

    #endregion Constructors
}

#endif
