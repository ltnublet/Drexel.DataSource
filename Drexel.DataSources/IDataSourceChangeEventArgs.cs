using System;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a change in an <see cref="IDataSource{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <see cref="object"/> which this <see cref="IDataSourceChangeEventArgs{T}"/> contains.
    /// </typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Implementors of this interface are expected to be used in place of a concrete EventArgs implementation.")]
    public interface IDataSourceChangeEventArgs<out T>
        where T : IUniquelyIdentifiable
    {
        /// <summary>
        /// The type of the change.
        /// </summary>
        DataSourceChangeEventType EventType { get; }

        /// <summary>
        /// The <typeparamref name="T"/> associated with this change.
        /// </summary>
        /// <returns>
        /// A <typeparamref name="T"/> associated with this change.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "We assume the underlying implementation could do work; thus, a property is not appropriate.")]
        T GetChange();
    }
}
