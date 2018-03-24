namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a local file path.
    /// </summary>
    public class FilePath
    {
        /// <summary>
        /// Instantiates a new <see cref="FilePath"/> instance.
        /// </summary>
        /// <param name="path">
        /// The local file path, formatted as a <see langword="string"/>.
        /// </param>
        public FilePath(string path)
        {
            this.Path = path;
        }

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
    }
}
