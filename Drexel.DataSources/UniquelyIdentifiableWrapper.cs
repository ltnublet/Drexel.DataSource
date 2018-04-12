using System;

namespace Drexel.DataSources
{
#pragma warning disable SA1600 // Elements must be documented. Justification: Internal.
    /// <summary>
    /// Represents a wrapper for <see cref="object"/>s which don't implement <see cref="IUniquelyIdentifiable"/>.
    /// Intended for use with <see cref="IDataSource{T}"/> where the desired T does not implement
    /// <see cref="IUniquelyIdentifiable"/>.
    /// </summary>
    internal class UniquelyIdentifiableWrapper : IUniquelyIdentifiable
    {
        private Lazy<HashWrapper> hashWrapper;

        public UniquelyIdentifiableWrapper(object toWrap)
        {
            this.Self = toWrap;
            this.hashWrapper =
                new Lazy<HashWrapper>(() => new HashWrapper(this.Self == null ? 0 : this.Self.GetHashCode()));
        }

        public object Self { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "CodeAnalysis doesn't see that the else case only occurs when both are non-null.")]
        public ComparisonResults Compare(IUniquelyIdentifiable identifiable)
        {
            bool selfNull = this.Self == null;
            bool otherNull = identifiable == null;

            if (selfNull && otherNull)
            {
                return ComparisonResults.Invalidating;
            }
            else if (selfNull ^ otherNull)
            {
                return ComparisonResults.Invalidating;
            }
            else
            {
                IUniqueIdentifier otherIdentifier = identifiable.GetIdentifier();
                HashWrapper selfIdentifier = this.hashWrapper.Value;
                if (otherIdentifier is HashWrapper wrapper)
                {
                    bool hashMatch = wrapper.Hash == selfIdentifier.Hash;
                    bool instanceMatch = wrapper.Instance == selfIdentifier.Instance;

                    if (hashMatch && instanceMatch)
                    {
                        return ComparisonResults.Match;
                    }
                    else if (hashMatch)
                    {
                        return ComparisonResults.DifferentButEquivalent;
                    }
                    else if (instanceMatch)
                    {
                        return ComparisonResults.Different | ComparisonResults.Invalidating;
                    }
                    else
                    {
                        return ComparisonResults.Different;
                    }
                }
                else
                {
                    // Since the other instance wasn't a wrapper, it's (probably) not a match. Also, since we have no
                    // idea what the type we contain is, assume we need to invalidate the cache.
                    return ComparisonResults.Different | ComparisonResults.Invalidating;
                }
            }
        }

        public IUniqueIdentifier GetIdentifier() => this.hashWrapper.Value;

        private class HashWrapper : IUniqueIdentifier
        {
            private static long instances = long.MinValue;

            public HashWrapper(int hash)
            {
                this.Hash = hash;
                if (HashWrapper.instances == long.MaxValue)
                {
                    HashWrapper.instances = long.MinValue;
                }
                else
                {
                    HashWrapper.instances++;
                }
            }

            public long Instance { get; private set; }

            public int Hash { get; private set; }

            public bool Equals(IUniqueIdentifier identifier)
            {
                if (identifier is HashWrapper wrapper)
                {
                    return wrapper.Hash == this.Hash && wrapper.Instance == this.Instance;
                }

                return false;
            }
        }
    }
#pragma warning restore SA1600
}
