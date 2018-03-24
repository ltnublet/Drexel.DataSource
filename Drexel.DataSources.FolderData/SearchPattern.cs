using System;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents a file search pattern.
    /// </summary>
    public class SearchPattern
    {
        /// <summary>
        /// Instantiates a <see cref="SearchPattern"/> instance using the supplied parameters.
        /// </summary>
        /// <param name="pattern">
        /// The filter string for this pattern.
        /// </param>
        /// <param name="option">
        /// The depth which searches associated with this pattern should be performed for.
        /// </param>
        public SearchPattern(string pattern, SearchOption? option = null)
        {
            this.Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            this.Option = option;
        }

        /// <summary>
        /// The filter string for this pattern.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The depth which searches associated with this pattern should be performed for.
        /// </summary>
        public SearchOption? Option { get; set; }
    }
}
