namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a change in an <see cref="FolderDataSource"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Suffix is correct.")]
    public interface IFolderDataChangeEventArgs : IDataSourceChangeEventArgs<IFileInformation>
    {
        /// <summary>
        /// The name of the local file or directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The local file path associated with this change.
        /// </summary>
        FilePath Path { get; }
    }
}
