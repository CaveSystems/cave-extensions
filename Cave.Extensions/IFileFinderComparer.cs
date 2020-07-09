namespace Cave
{
    /// <summary>
    /// Provides an interface for <see cref="FileFinder"/> file comparer.
    /// </summary>
    public interface IFileFinderComparer
    {
        /// <summary>
        /// Determines whether a file matches the wanted criterias or not.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>Returns true if the file matches, false otherwise.</returns>
        bool FileMatches(FileItem file);
    }
}
