using System;
using System.IO;
using Drexel.DataSources.External;
using Drexel.DataSources.ReferenceImplementation;
using Drexel.Formats.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IFolderDataChangeEventArgs"/> implementation which wraps
    /// <see cref="FileSystemEventArgs"/>.
    /// </summary>
    public class FolderDataChangeEventArgs : IFolderDataChangeEventArgs
    {
        private string name;

        /// <summary>
        /// Instantiates a new <see cref="FolderDataChangeEventArgs"/> wrapping the supplied
        /// <see cref="FileSystemEventArgs"/> <paramref name="e"/>.
        /// </summary>
        /// <param name="e">
        /// The <see cref="FileSystemEventArgs"/> to wrap.
        /// </param>
        public FolderDataChangeEventArgs(FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    this.EventType = DataSourceChangeEventType.Added;
                    break;
                case WatcherChangeTypes.Changed:
                case WatcherChangeTypes.Renamed:
                    this.EventType = DataSourceChangeEventType.Changed;
                    break;
                case WatcherChangeTypes.Deleted:
                    this.EventType = DataSourceChangeEventType.Removed;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.name = e.Name;
            this.Path = new FilePath(e.FullPath);
        }

        /// <inheritdoc />
        public DataSourceChangeEventType EventType { get; private set; }

        /// <inheritdoc />
        public FilePath Path { get; private set; }

        /// <inheritdoc />
        public IDataInformation GetImageInformation()
        {
            FileInfo info = new FileInfo(this.Path.Path);
            return new DataInformation(
                this.name,
                info.CreationTimeUtc,
                () => info.LastWriteTimeUtc,
                () => info.OpenRead());
        }

        IDataInformation IDataSourceChangeEventArgs.GetImageInformation()
        {
            return this.GetImageInformation();
        }
    }
}
