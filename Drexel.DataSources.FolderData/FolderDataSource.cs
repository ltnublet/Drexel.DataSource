using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drexel.DataSources.External;
using Drexel.Formats.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IDataSource"/> implementation which uses a local directory as the backing data store.
    /// </summary>
    public class FolderDataSource : IDataSource
    {
        private readonly IDirectoryInteractorFactory interactorFactory;
        private readonly IFolderDataWatcherFactory watcherFactory;

        private IDirectoryInteractor interactor;
        private FilePath root;
        private IFolderDataWatcher watcher;

        private Dictionary<FilePath, IDataInformation> files;

        private bool disposed;
        private object disposalLock;

        internal FolderDataSource(
            FilePath root,
            IFolderDataWatcherFactory watcherFactory = null,
            IDirectoryInteractorFactory interactorFactory = null)
        {
            this.files = new Dictionary<FilePath, IDataInformation>();
            this.root = root;

            this.interactorFactory = interactorFactory ?? new DirectoryInteractorFactory();
            this.watcherFactory = watcherFactory ?? new FolderDataWatcherFactory();

            this.interactor = this.interactorFactory.MakeInteractor();
            this.watcher = this.watcherFactory.MakeFolderDataWatcher(root);

            this.disposed = false;
            this.disposalLock = new object();

            watcher.EnableRaisingEvents = true;

            this.watcher.Added += (obj, e) =>
            {
                this.Add(e);
                this.OnChange.Invoke(obj, e);
            };
            this.watcher.Changed += (obj, e) =>
            {
                this.Update(e);
                this.OnChange.Invoke(obj, e);
            };
            this.watcher.Removed += (obj, e) =>
            {
                this.Remove(e);
                this.OnChange.Invoke(obj, e);
            };

            foreach (FilePath path in this.interactor.EnumerateFiles(this.root))
            {
                this.files.Add(path, this.CreateIImageInformationFromPath(path));
            }
        }

        /// <summary>
        /// Raised when a change occurs within the underlying directory.
        /// </summary>
        public event EventHandler<IDataSourceChangeEventArgs> OnChange;

        /// <inheritdoc />
        public int Count => this.files.Count;

        /// <inheritdoc />
        public string Name => this.root.Path;

        /// <inheritdoc />
        public void Dispose()
        {
            lock (this.disposalLock)
            {
                if (!this.disposed)
                {
                    this.disposed = true;

                    this.OnChange = null;
                    this.watcher.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public IEnumerator<IDataInformation> GetEnumerator()
        {
            return this.files.Select(x => x.Value).GetEnumerator();
        }

        /// <inheritdoc />
        public IReadOnlyList<IDataSource> GetSubDataSources()
        {
            return this
                .interactor
                .EnumerateDirectories(this.root)
                .Select(x => new FolderDataSource(x, this.watcherFactory, this.interactorFactory))
                .ToList();
        }

        private IDataInformation CreateIImageInformationFromPath(FilePath path)
        {
            return new Placeholder(path.Path);
        }

        private void Add(IFolderDataChangeEventArgs args)
        {
            this.files.Add(args.Path, args.GetImageInformation());
        }

        private void Remove(IFolderDataChangeEventArgs args)
        {
            this.files.Remove(args.Path);
        }

        private void Update(IFolderDataChangeEventArgs args)
        {
            this.files[args.Path] = args.GetImageInformation();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class Placeholder : IDataInformation
        {
            public Placeholder(string name)
            {
                this.Name = name;
            }

            public DateTime Created => DateTime.UtcNow;

            public DateTime LastModified => DateTime.UtcNow;

            public string Name { get; private set; }

            public bool TryOpen(out Stream stream, out Exception failureReason)
            {
                stream = null;
                failureReason = new NotImplementedException();

                return false;
            }
        }
    }
}
