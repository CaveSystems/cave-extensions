#pragma warning disable CS1591 // No comments for backports
#pragma warning disable IDE0130 // Namespace does not match folder structure

#if (NET20_OR_GREATER && !NET5_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER) || (NETCOREAPP1_0_OR_GREATER && !NETCOREAPP3_0_OR_GREATER) || (NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER)

namespace System.Runtime.CompilerServices;

public static class RuntimeHelpers
{
    #region Static

    public static T[] GetSubArray<T>(T[] array, Range range)
    {
        //end is exclusive, start inclusive
        var start = range.Start.GetOffset(array.Length);
        var end = range.End.GetOffset(array.Length);
        var count = end - start;
        var result = new T[count];
        Array.Copy(array, start, result, 0, count);
        return result;
    }

    #endregion Static
}

#endif
