namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a unique indentifier.
    /// </summary>
    public interface IUniqueIdentifier
    {
        /// <summary>
        /// Indicates whether the <see cref="IUniqueIdentifier"/> <paramref name="identifier"/> equals this instance.
        /// </summary>
        /// <param name="identifier">
        /// The unique identifier to check equality against.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IUniqueIdentifier"/>s are equal; <see langword="false"/>
        /// otherwise.
        /// </returns>
        bool Equals(IUniqueIdentifier identifier);
    }
}
