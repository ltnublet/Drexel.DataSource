using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.DataSources
{
    /// <summary>
    /// A <see cref="IDataSource{T}"/> which wraps another <see cref="IDataSource{T}"/>, converting all instances of
    /// <typeparamref name="TFrom"/> to <typeparamref name="TTo"/>.
    /// </summary>
    /// <typeparam name="TFrom">
    /// The type of object to transform from.
    /// </typeparam>
    /// <typeparam name="TTo">
    /// The type of object to transform to.
    /// </typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Suffix is correct.")]
    public class TransformDataSource<TFrom, TTo> : IDataSource<TTo>
        where TTo : IUniquelyIdentifiable
        where TFrom : IUniquelyIdentifiable
    {
        private object cacheLock;
        private IDataSource<TFrom> backingSource;
        private Func<string> nameBackingFunc;
        private List<TTo> cachedValues;

        private Func<TFrom, TTo> instanceTransform;
        private Func<IDataSourceChangeEventArgs<TFrom>, IDataSourceChangeEventArgs<TTo>> eventTransform;
        private Func<IDataSource<TFrom>, IDataSource<TTo>> subDataSourceTransform;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformDataSource{TFrom, TTo}"/> class.
        /// </summary>
        /// <param name="source">
        /// The underlying <see cref="IDataSource{T}"/> to wrap.
        /// </param>
        /// <param name="instanceTransform">
        /// Transforms a <typeparamref name="TFrom"/> to a <typeparamref name="TTo"/>.
        /// </param>
        /// <param name="eventTransform">
        /// Transforms a <see cref="IDataSourceChangeEventArgs{T}"/> of type <typeparamref name="TFrom"/> to a
        /// <see cref="IDataSourceChangeEventArgs{T}"/> of type <typeparamref name="TTo"/>.
        /// </param>
        /// <param name="subDataSourceTransform">
        /// Transforms a sub-data source retrieved by calling the <see cref="IDataSource{T}.GetSubDataSources"/>
        /// method of the <paramref name="source"/> object from <see cref="IDataSource{T}"/> of type
        /// <typeparamref name="TFrom"/> to an <see cref="IDataSource{T}"/> of type <typeparamref name="TTo"/>.
        /// </param>
        /// <param name="nameFactory">
        /// The name factory. Must be safe to invoke multiple times.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when a supplied argument was <see langword="null"/>, but the constructor required it to be non-null.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "By design, this is required.")]
        public TransformDataSource(
            IDataSource<TFrom> source,
            Func<TFrom, TTo> instanceTransform,
            Func<IDataSourceChangeEventArgs<TFrom>, IDataSourceChangeEventArgs<TTo>> eventTransform,
            Func<IDataSource<TFrom>, IDataSource<TTo>> subDataSourceTransform,
            Func<string> nameFactory = null)
        {
            this.backingSource =
                source ?? throw new ArgumentNullException(nameof(source));
            this.instanceTransform =
                instanceTransform ?? throw new ArgumentNullException(nameof(instanceTransform));
            this.eventTransform = eventTransform ?? throw new ArgumentNullException(nameof(eventTransform));
            this.subDataSourceTransform =
                subDataSourceTransform ?? throw new ArgumentNullException(nameof(subDataSourceTransform));

            this.cacheLock = new object();
            this.nameBackingFunc = nameFactory ?? (() => source.Name);

            this.backingSource.OnChange +=
                (obj, e) =>
                {
                    lock (this.cacheLock)
                    {
                        this.cachedValues = null;
                    }

                    this.OnChange.Invoke(obj, this.eventTransform(e));
                };
        }

        /// <inheritdoc />
        public event EventHandler<IDataSourceChangeEventArgs<TTo>> OnChange;

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (this.cacheLock)
                {
                    if (this.cachedValues != null)
                    {
                        return this.cachedValues.Count;
                    }
                    else
                    {
                        return this.AsEnumerable().Count();
                    }
                }
            }
        }

        /// <inheritdoc />
        public string Name => this.nameBackingFunc.Invoke();

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public IEnumerator<TTo> GetEnumerator()
        {
            lock (this.cacheLock)
            {
                if (this.cachedValues == null)
                {
                    this.cachedValues = new List<TTo>();
                    foreach (TFrom from in this.backingSource)
                    {
                        TTo to = this.instanceTransform(from);
                        this.cachedValues.Add(to);
                        yield return to;
                    }
                }
                else
                {
                    foreach (TTo to in this.cachedValues)
                    {
                        yield return to;
                    }
                }
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<IDataSource<TTo>> GetSubDataSources()
        {
            return this
                .backingSource
                .GetSubDataSources()
                .Select(x => this.subDataSourceTransform(x))
                .ToList();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.OnChange = null;
                lock (this.cacheLock)
                {
                    this.cachedValues = null;
                }

                this.backingSource.Dispose();
            }
        }
    }
}
