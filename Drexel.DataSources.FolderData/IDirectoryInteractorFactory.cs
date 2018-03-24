namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a factory which produces <see cref="IDirectoryInteractor"/>s.
    /// </summary>
    public interface IDirectoryInteractorFactory
    {
        /// <summary>
        /// Returns an <see cref="IDirectoryInteractor"/> instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IDirectoryInteractor"/> instance.
        /// </returns>
        IDirectoryInteractor MakeInteractor();
    }
}
