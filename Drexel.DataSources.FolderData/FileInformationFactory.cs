namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Produces <see cref="FileInformation"/>s from <see cref="FilePath"/>s.
    /// </summary>
    /// <seealso cref="IFileInformationFactory" />
    public class FileInformationFactory : IFileInformationFactory
    {
        /// <summary>
        /// Produces an <see cref="IFileInformation" /> from the given <see cref="FilePath" /> <paramref name="path" />.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// An <see cref="IFileInformation" /> produced from the given <see cref="FilePath" /> <paramref name="path" />.
        /// </returns>
        public IFileInformation FromPath(FilePath path)
        {
            return new FileInformation(path);
        }
    }
}
