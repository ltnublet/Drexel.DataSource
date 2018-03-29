using System;

namespace Drexel.DataSources.FolderData.Tests.Mocks
{
    internal class MockFolderDataWatcher : IFolderDataWatcher
    {
        public MockFolderDataWatcher(
            out Action<object, IFolderDataChangeEventArgs> invokeAdded,
            out Action<object, IFolderDataChangeEventArgs> invokeChanged,
            out Action<object, IFolderDataChangeEventArgs> invokeMoved,
            out Action<object, IFolderDataChangeEventArgs> invokeRemoved)
        {
            invokeAdded = (obj, e) => this.Added.Invoke(obj, e);
            invokeChanged = (obj, e) => this.Changed.Invoke(obj, e);
            invokeMoved = (obj, e) => this.Moved.Invoke(obj, e);
            invokeRemoved = (obj, e) => this.Removed.Invoke(obj, e);
        }

        public event EventHandler<IFolderDataChangeEventArgs> Added;

        public event EventHandler<IFolderDataChangeEventArgs> Changed;

        public event EventHandler<IFolderDataChangeEventArgs> Moved;

        public event EventHandler<IFolderDataChangeEventArgs> Removed;

        public bool EnableRaisingEvents { get; set; }

        public void Dispose()
        {
            this.Added = null;
            this.Changed = null;
            this.Removed = null;
        }
    }
}
