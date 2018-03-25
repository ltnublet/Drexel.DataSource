namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a class which produces <see cref="IFileInformation"/>s from <see cref="FilePath"/>s.
    /// </summary>
    public interface IFileInformationFactory
    {
        /// <summary>
        /// Produces an <see cref="IFileInformation"/> from the given <see cref="FilePath"/> <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// An <see cref="IFileInformation"/> produced from the given <see cref="FilePath"/> <paramref name="path"/>.
        /// </returns>
        IFileInformation FromPath(FilePath path);
    }
}
