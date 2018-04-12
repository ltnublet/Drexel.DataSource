using System;
using System.Collections.Generic;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a data source containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <see cref="object"/> contained within this <see cref="IDataSource{T}"/>.
    /// </typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Implements additional behavior beyond being a simple collection.")]
    public interface IDataSource<T> : IEnumerable<T>, IDisposable
        where T : IUniquelyIdentifiable
    {
        /// <summary>
        /// Indicates that the underlying data source has had a change applied to it.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1009:DeclareEventHandlersCorrectly",
            Justification = "IDataSourceChangeEventArgs is an interface by design, and thus cannot implement the EventArgs class.")]
        event EventHandler<IDataSourceChangeEventArgs<T>> OnChange;

        /// <summary>
        /// The number of <typeparamref name="T"/>s contained within this <see cref="IDataSource{T}"/>.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The name of this <see cref="IDataSource{T}"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns a collection of <see cref="IDataSource{T}"/>s contained within this instance.
        /// </summary>
        /// <returns>
        /// An <see cref="IReadOnlyList{T}"/> of <see cref="Type"/> <see cref="IDataSource{T}"/> whose contents are the
        /// <see cref="IDataSource{T}"/>s contained within this instance.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Required by design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "We assume the underlying implementation could do work; thus, a property is not appropriate.")]
        IReadOnlyList<IDataSource<T>> GetSubDataSources();
    }
}
