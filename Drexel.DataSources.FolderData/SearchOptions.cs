namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents the depth a search should be performed for.
    /// </summary>
    public enum SearchOption
    {
        /// <summary>
        /// All directories in the directory hierarchy (using the specified directory as the root) are included in the
        /// search.
        /// </summary>
        AllDirectories = 1,

        /// <summary>
        /// Only the specified directory is included in the search.
        /// </summary>
        TopDirectoryOnly = 0
    }
}
