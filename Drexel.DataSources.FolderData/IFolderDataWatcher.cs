using System;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a directory watcher that raises events when a directory changes.
    /// </summary>
    public interface IFolderDataWatcher : IDisposable
    {
        /// <summary>
        /// Raised when the change to the directory was triggered by an addition.
        /// </summary>
        event EventHandler<IFolderDataChangeEventArgs> Added;

        /// <summary>
        /// Raised when the change to the directory was triggered by a change.
        /// </summary>
        event EventHandler<IFolderDataChangeEventArgs> Changed;

        /// <summary>
        /// Raised when teh change to the directory was triggered by a removal.
        /// </summary>
        event EventHandler<IFolderDataChangeEventArgs> Removed;

        /// <summary>
        /// <see langword="true"/> when changes will raise events; <see langword="false"/> otherwise.
        /// </summary>
        bool EnableRaisingEvents { get; set; }
    }
}
