using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a factory which produces <see cref="IFolderDataWatcher"/>s.
    /// </summary>
    public interface IFolderDataWatcherFactory
    {
        /// <summary>
        /// Produces an <see cref="IFolderDataWatcher"/> using the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path of the directory to watch.
        /// </param>
        /// <returns>
        /// An <see cref="IFolderDataWatcher"/> instance.
        /// </returns>
        IFolderDataWatcher MakeFolderDataWatcher(FilePath path);
    }
}
