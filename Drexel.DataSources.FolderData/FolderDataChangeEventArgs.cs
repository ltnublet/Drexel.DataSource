using System;
using System.IO;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IFolderDataChangeEventArgs"/> implementation which wraps
    /// <see cref="FileSystemEventArgs"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Suffix is correct.")]
    public class FolderDataChangeEventArgs : IFolderDataChangeEventArgs
    {
        /// <summary>
        /// Instantiates a new <see cref="FolderDataChangeEventArgs"/> wrapping the supplied
        /// <see cref="FileSystemEventArgs"/> <paramref name="e"/>.
        /// </summary>
        /// <param name="e">
        /// The <see cref="FileSystemEventArgs"/> to wrap.
        /// </param>
        public FolderDataChangeEventArgs(FileSystemEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    this.EventType = DataSourceChangeEventType.Added;
                    break;
                case WatcherChangeTypes.Changed:
                    this.EventType = DataSourceChangeEventType.Changed;
                    break;
                case WatcherChangeTypes.Renamed:
                    this.EventType = DataSourceChangeEventType.Moved;
                    break;
                case WatcherChangeTypes.Deleted:
                    this.EventType = DataSourceChangeEventType.Removed;
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.Name = e.Name;
            this.Path = new FilePath(e.FullPath, new PathInteractor());
        }

        /// <inheritdoc />
        public DataSourceChangeEventType EventType { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public FilePath Path { get; private set; }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "This method does work, and thus shouldn't be a property.")]
        public IFileInformation GetChange()
        {
            return new FileInformation(this.Path);
        }

        IFileInformation IDataSourceChangeEventArgs<IFileInformation>.GetChange()
        {
            return this.GetChange();
        }
    }
}
