using System.Collections.Generic;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a means by which to interact with a directory.
    /// </summary>
    public interface IDirectoryInteractor
    {
        /// <summary>
        /// Enumerates files contained within the associated <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path to a directory to enumerate files within.
        /// </param>
        /// <param name="pattern">
        /// An optional search pattern to apply while enumerating files.
        /// </param>
        /// <returns>
        /// A collection of <see cref="FilePath"/>s contained within the specified <paramref name="path"/> that
        /// satisfied supplied <paramref name="pattern"/>, if any. These <see cref="FilePath"/>s are paths to files.
        /// </returns>
        IEnumerable<FilePath> EnumerateFiles(FilePath path, SearchPattern pattern = null);

        /// <summary>
        /// Enumerates directories contained within the associated <paramref name="path"/>.
        /// </summary>
        /// <param name="path">
        /// The path to a directory to enumerate directories within.
        /// </param>
        /// <param name="pattern">
        /// An optional search pattern to apply while enumerating directories.
        /// </param>
        /// <returns>
        /// A collection of <see cref="FilePath"/>s contained within the specified <paramref name="path"/> that
        /// satisfied supplied <paramref name="pattern"/>, if any. These <see cref="FilePath"/>s are paths to
        /// directories.
        /// </returns>
        IEnumerable<FilePath> EnumerateDirectories(FilePath path, SearchPattern pattern = null);
    }
}
