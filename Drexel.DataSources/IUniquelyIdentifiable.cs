namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a uniquely identifiable resource.
    /// </summary>
    public interface IUniquelyIdentifiable
    {
        /// <summary>
        /// Indicates whether the <see cref="IUniquelyIdentifiable"/> instance <paramref name="identifiable"/> matches
        /// this instance.
        /// </summary>
        /// <param name="identifiable">
        /// The <see cref="IUniquelyIdentifiable"/> to compare against.
        /// </param>
        /// <returns>
        /// A <see cref="ComparisonResults"/> containing the results of the comparison.
        /// </returns>
        ComparisonResults Compare(IUniquelyIdentifiable identifiable);

        /// <summary>
        /// Computes the unique identifier of this <see cref="IUniquelyIdentifiable"/> instance.
        /// </summary>
        /// <returns>
        /// The unique identifier of this instance.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "A property is not appropriate because the underlying implementation may do work.")]
        IUniqueIdentifier GetIdentifier();
    }
}
