using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a collection of <see cref="IDataSource{T}"/>s, where each <see cref="IDataSource{T}"/> is of the
    /// same <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of <see cref="object"/> the underlying <see cref="IDataSource{T}"/>s contain.
    /// </typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Implements additional behavior beyond being a simple collection.")]
    public class CompositeDataSource<T> : IDataSource<T>, IEnumerable<IDataSource<T>>
        where T : IUniquelyIdentifiable
    {
        private IList<IDataSource<T>> dataSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeDataSource{T}"/> class, wrapping the supplied
        /// <see cref="IDataSource{T}"/>s in <paramref name="dataSources"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="CompositeDataSource{T}"/> instance.
        /// </param>
        /// <param name="dataSources">
        /// The <see cref="IDataSource{T}"/>s to wrap.
        /// </param>
        public CompositeDataSource(string name, params IDataSource<T>[] dataSources)
            : this(name, dataSources.AsEnumerable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeDataSource{T}"/> class, wrapping the supplied
        /// <see cref="IDataSource{T}"/>s in <paramref name="dataSources"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="CompositeDataSource{T}"/> instance.
        /// </param>
        /// <param name="dataSources">
        /// The <see cref="IDataSource{T}"/>s to wrap.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Design requires it.")]
        public CompositeDataSource(string name, IEnumerable<IDataSource<T>> dataSources)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException(nameof(dataSources));
            }

            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.dataSources = dataSources.ToList();

            foreach (IDataSource<T> dataSource in this.dataSources)
            {
                dataSource.OnChange += this.OnChange.Invoke;
            }
        }

        /// <inheritdoc />
        public event EventHandler<IDataSourceChangeEventArgs<T>> OnChange;

        /// <inheritdoc />
        public int Count => this.dataSources.Count;

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T value in this.dataSources)
            {
                yield return value;
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<IDataSource<T>> GetSubDataSources()
        {
            IEnumerable<IDataSource<T>> GetSubDataSourcesInternal()
            {
                foreach (IDataSource<T> dataSource in this)
                {
                    foreach (IDataSource<T> subDataSource in dataSource.GetSubDataSources())
                    {
                        yield return subDataSource;
                    }
                }
            }

            return GetSubDataSourcesInternal().ToList();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerableInternal().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<IDataSource<T>> IEnumerable<IDataSource<T>>.GetEnumerator()
        {
            return this.GetEnumerableInternal().GetEnumerator();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching them so that we can throw an AggregateException.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1065:DoNotRaiseExceptionsInUnexpectedLocations",
            Justification = "If one of the data sources would throw, we're going to encounter an exception anyway.")]
        protected virtual void Dispose(bool disposing)
        {
            foreach (IDataSource<T> dataSource in this.dataSources)
            {
                List<Exception> exceptions = new List<Exception>();
                try
                {
                    dataSource.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        private IEnumerable<IDataSource<T>> GetEnumerableInternal()
        {
            foreach (IDataSource<T> dataSource in this.dataSources)
            {
                yield return dataSource;
            }
        }
    }
}
