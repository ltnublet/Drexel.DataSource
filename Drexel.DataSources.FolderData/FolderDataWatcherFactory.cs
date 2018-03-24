using Drexel.DataSources.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IFolderDataWatcherFactory"/> implementation which produces
    /// <see cref="FolderDataWatcher"/>s.
    /// </summary>
    public class FolderDataWatcherFactory : IFolderDataWatcherFactory
    {
        /// <summary>
        /// Instantiates a new <see cref="FolderDataWatcherFactory"/> using the supplied parameters.
        /// </summary>
        /// <param name="filter">
        /// The search filter to use for produced <see cref="FolderDataWatcher"/>s.
        /// </param>
        /// <param name="changeFilterTicks">
        /// The number of ticks to wait between raises of the <see cref="FolderDataWatcher.Changed"/> event.
        /// </param>
        public FolderDataWatcherFactory(string filter = null, long changeFilterTicks = 0L)
        {
            this.Filter = filter;
            this.ChangeFilterTicks = changeFilterTicks;
        }

        /// <summary>
        /// The search filter to use for produced <see cref="FolderDataWatcher"/>s.
        /// </summary>
        public string Filter { get; private set; }

        /// <summary>
        /// The number of ticks to wait between raises of the <see cref="FolderDataWatcher.Changed"/> event.
        /// </summary>
        public long ChangeFilterTicks { get; private set; }

        /// <inheritdoc />
        public IFolderDataWatcher MakeFolderDataWatcher(FilePath path)
        {
            return new FolderDataWatcher(path, this.Filter, this.ChangeFilterTicks);
        }
    }
}
