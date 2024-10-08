#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !(NETSTANDARD1_0_OR_GREATER && !NETSTANDARD2_0_OR_GREATER)

namespace Cave;

/// <summary>Gets an interface for <see cref="FileFinder"/> file comparer.</summary>
public interface IFileFinderComparer
{
    #region Members

    /// <summary>Determines whether a file matches the wanted criterias or not.</summary>
    /// <param name="file">The file to check.</param>
    /// <returns>Returns true if the file matches, false otherwise.</returns>
    bool FileMatches(FileItem file);

    #endregion Members
}

#endif
