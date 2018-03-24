using System;
using System.IO;
using System.Reactive.Linq;
using Drexel.DataSources.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IFolderDataWatcher"/> implementation which operates on a local directory.
    /// </summary>
    public class FolderDataWatcher : IFolderDataWatcher
    {
        private FileSystemWatcher fileSystemWatcherBackingField;
        private string filterBackingField;
        private FilePath pathBackingField;

        private bool disposed;
        private object disposalLock;

        /// <summary>
        /// Instantiates a new <see cref="FolderDataWatcher"/> using the specified arguments.
        /// </summary>
        /// <param name="path">
        /// The path to the directory to watch for changes.
        /// </param>
        /// <param name="filter">
        /// An optional filter that controls the type of files to watch. For example, "*.txt" watches for changes to
        /// text files.
        /// </param>
        /// <param name="changeFilterTicks">
        /// An optional number of ticks to wait between change event firings. This is useful for cases where a file
        /// is modified by an application that performs sequential distinct writes (ex. writes new file content, then
        /// new metadata.)
        /// 
        /// Note that can cause valid change events to be dropped - for example, if two files are both changed within
        /// the specified number of ticks, one of the changes will not raise an event.
        /// </param>
        public FolderDataWatcher(FilePath path, string filter = null, long changeFilterTicks = 0L)
        {
            this.Path = path;
            this.Filter = filter;
            this.FileSystemWatcher =
                filter == null
                    ? new FileSystemWatcher(this.Path.Path)
                    : new FileSystemWatcher(this.Path.Path, this.Filter);

            this.disposed = false;
            this.disposalLock = new object();

            // Known issue - changed event fires multiple times for some applications, because they "write"
            // multiple times (ex. update file contents, then as a separate step update metadata).
            if (changeFilterTicks > 0L)
            {
                Observable
                    .FromEventPattern<FileSystemEventArgs>(
                        this.FileSystemWatcher,
                        nameof(this.FileSystemWatcher.Changed))
                    .Throttle(new TimeSpan(changeFilterTicks))
                    .Subscribe(e => this.Changed.Invoke(e.Sender, new FolderDataChangeEventArgs(e.EventArgs)));
            }
            else
            {
                this.FileSystemWatcher.Changed +=
                    (obj, e) => this.Changed.Invoke(obj, new FolderDataChangeEventArgs(e));
            }

            this.FileSystemWatcher.Created +=
                (obj, e) => this.Added.Invoke(obj, new FolderDataChangeEventArgs(e));
            this.FileSystemWatcher.Deleted +=
                (obj, e) => this.Removed.Invoke(obj, new FolderDataChangeEventArgs(e));
            this.FileSystemWatcher.Renamed +=
                (obj, e) => this.Changed.Invoke(obj, new FolderDataChangeEventArgs(e));
        }

        /// <inheritdoc />
        public virtual bool EnableRaisingEvents
        {
            get
            {
                return this.FileSystemWatcher.EnableRaisingEvents;
            }
            set
            {
                this.FileSystemWatcher.EnableRaisingEvents = value;
            }
        }

        /// <summary>
        /// The underlying <see cref="System.IO.FileSystemWatcher"/> used to monitor the directory being watched.
        /// </summary>
        protected virtual FileSystemWatcher FileSystemWatcher
        {
            get
            {
                return this.fileSystemWatcherBackingField;
            }
            set
            {
                this.fileSystemWatcherBackingField = value;
            }
        }

        /// <summary>
        /// The filter that controls what types of files to watch.
        /// </summary>
        protected virtual string Filter
        {
            get
            {
                return this.filterBackingField;
            }
            set
            {
                this.filterBackingField = value;
            }
        }

        /// <summary>
        /// The path to the directory being watched.
        /// </summary>
        protected virtual FilePath Path
        {
            get
            {
                return this.pathBackingField;
            }
            set
            {
                this.pathBackingField = value;
            }
        }

        /// <inheritdoc />
        public event EventHandler<IFolderDataChangeEventArgs> Added;

        /// <inheritdoc />
        public event EventHandler<IFolderDataChangeEventArgs> Changed;

        /// <inheritdoc />
        public event EventHandler<IFolderDataChangeEventArgs> Removed;

        /// <inheritdoc />
        public void Dispose()
        {
            lock (this.disposalLock)
            {
                if (!this.disposed)
                {
                    this.disposed = true;

                    this.Added = null;
                    this.Changed = null;
                    this.Removed = null;

                    this.FileSystemWatcher.Dispose();
                }
            }
        }
    }
}
