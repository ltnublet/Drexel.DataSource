using System;
using System.IO;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents information about a file.
    /// </summary>
    public interface IFileInformation
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The location of the file.
        /// </summary>
        FilePath Path { get; }

        /// <summary>
        /// The creation date of the file.
        /// </summary>
        DateTime? Created { get; }

        /// <summary>
        /// The last modified date of the file.
        /// </summary>
        DateTime? LastModified { get; }

        /// <summary>
        /// Tries to read the file.
        /// </summary>
        /// <param name="stream">
        /// <see langword="null"/> if the file could not be read; otherwise, a <see cref="Stream"/> by which to
        /// access the file contents.
        /// </param>
        /// <param name="failureReason">
        /// <see langword="null"/> if the file was successfully read; <see langword="null"/> otherwise.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the file was sucessfully read; <see langword="false"/> otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1021:AvoidOutParameters",
            MessageId = "0#",
            Justification = "It is a Try method.")]
        bool TryRead(out Stream stream, out Exception failureReason);
    }
}
