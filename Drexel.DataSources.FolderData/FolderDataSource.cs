using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IDataSource{IFileInformation}"/> implementation which uses a local directory as the
    /// backing data store.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Naming convention is DataSource.")]
    public sealed class FolderDataSource : IDataSource<IFileInformation>
    {
        private readonly IDirectoryInteractorFactory interactorFactory;
        private readonly IFolderDataWatcherFactory watcherFactory;
        private readonly IFileInformationFactory fileInformationFactory;

        private IDirectoryInteractor interactor;
        private FilePath root;
        private IFolderDataWatcher watcher;

        private Dictionary<FilePath, IFileInformation> files;

        private bool disposed;
        private object disposalLock;

        internal FolderDataSource(
            FilePath root,
            IFolderDataWatcherFactory watcherFactory = null,
            IDirectoryInteractorFactory interactorFactory = null,
            IFileInformationFactory fileInformationFactory = null)
        {
            this.files = new Dictionary<FilePath, IFileInformation>();
            this.root = root;

            this.interactorFactory = interactorFactory ?? new DirectoryInteractorFactory();
            this.watcherFactory = watcherFactory ?? new FolderDataWatcherFactory();
            this.fileInformationFactory = fileInformationFactory ?? new FileInformationFactory();

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
            this.watcher.Moved += (obj, e) =>
            {
                this.Move(e);
                this.OnChange.Invoke(obj, e);
            };
            this.watcher.Removed += (obj, e) =>
            {
                this.Remove(e);
                this.OnChange.Invoke(obj, e);
            };

            foreach (FilePath path in this.interactor.EnumerateFiles(this.root))
            {
                this.files.Add(path, this.fileInformationFactory.FromPath(path));
            }
        }

        /// <summary>
        /// Raised when a change occurs within the underlying directory.
        /// </summary>
        public event EventHandler<IDataSourceChangeEventArgs<IFileInformation>> OnChange;

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
        public IEnumerator<IFileInformation> GetEnumerator()
        {
            return this.files.Select(x => x.Value).GetEnumerator();
        }

        /// <inheritdoc />
        public IReadOnlyList<IDataSource<IFileInformation>> GetSubDataSources()
        {
            return this
                .interactor
                .EnumerateDirectories(this.root)
                .Select(x => new FolderDataSource(x, this.watcherFactory, this.interactorFactory))
                .ToList();
        }

        private void Add(IFolderDataChangeEventArgs args)
        {
            this.files.Add(args.Path, args.GetChange());
        }

        private void Move(IFolderDataChangeEventArgs args)
        {
            ////IFileInformation cached = this.files[args.OldPath];
            ////this.files.Remove(args.OldPath);
            ////this.files.Add(args.NewPath, cached);
            throw new NotImplementedException();
        }

        private void Remove(IFolderDataChangeEventArgs args)
        {
            this.files.Remove(args.Path);
        }

        private void Update(IFolderDataChangeEventArgs args)
        {
            this.files[args.Path] = args.GetChange();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
