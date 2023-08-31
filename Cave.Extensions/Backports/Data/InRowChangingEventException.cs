#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP2_0_OR_GREATER)
namespace System.Data;

public class InRowChangingEventException : DataException
{
    #region Constructors

    public InRowChangingEventException() { }

    public InRowChangingEventException(string s) : base(s) { }

    public InRowChangingEventException(string message, Exception innerException) : base(message, innerException) { }

    #endregion Constructors
}

#endif
