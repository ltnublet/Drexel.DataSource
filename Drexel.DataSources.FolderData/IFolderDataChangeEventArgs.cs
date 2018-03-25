namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a change in an <see cref="FolderDataSource"/>.
    /// </summary>
    public interface IFolderDataChangeEventArgs : IDataSourceChangeEventArgs<IFileInformation>
    {
        /// <summary>
        /// The local file path associated with this change.
        /// </summary>
        FilePath Path { get; }
    }
}
