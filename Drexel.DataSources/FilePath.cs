using System;
using System.Runtime.CompilerServices;
using Sys = System.IO;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a local file path.
    /// </summary>
    public class FilePath : IUniqueIdentifier
    {
        private const string InvalidPath = "The specified path is not a valid fully-qualified path.";

        /// <summary>
        /// Instantiates a new <see cref="FilePath"/> instance.
        /// </summary>
        /// <param name="path">
        /// The local file path, formatted as a <see langword="string"/>. Must be fully-qualified (not relative).
        /// </param>
        /// <param name="caseSensitive">
        /// Indicates whether the file path should be treated case-sensitively.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The System.IO.Path class forces us to do this.")]
        public FilePath(string path, bool caseSensitive = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            bool valid = true;
            try
            {
                valid &= FilePath.InvariantCultureStringEquals(path, Sys.Path.GetFullPath(path), caseSensitive);
            }
            catch
            {
                // Using exceptions for control flow is bad practice, but such is life.
                valid = false;
            }

            valid &= Sys.Path.IsPathRooted(path);

            if (!valid)
            {
                throw new ArgumentException(FilePath.InvalidPath, nameof(path));
            }

            this.Path = path;
        }

        /// <summary>
        /// Indicates whether the path is considered case-sensitive.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if case sensitive; <see langword="false"/> otherwise.
        /// </value>
        public bool CaseSensitive { get; private set; }

        /// <summary>
        /// The local file path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Returns a string that represents this <see cref="FilePath"/> instance.
        /// </summary>
        /// <returns>
        /// A string that represents this <see cref="FilePath"/> instance.
        /// </returns>
        public override string ToString()
        {
            return this.Path;
        }

        /// <inheritdoc />
        bool IUniqueIdentifier.Equals(IUniqueIdentifier identifier)
        {
            if (identifier is FilePath other)
            {
                return FilePath.InvariantCultureStringEquals(
                    this.Path,
                    other.Path,
                    this.CaseSensitive || other.CaseSensitive);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InvariantCultureStringEquals(string a, string b, bool caseSensitive)
        {
            return string.Equals(
                a,
                b,
                caseSensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }
    }
}
