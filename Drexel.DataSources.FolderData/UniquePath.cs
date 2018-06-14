using System;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData
{
    public class UniquePath : IUniqueIdentifier
    {
        public UniquePath(FilePath inner)
        {
            this.Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public FilePath Inner { get; private set; }

        public bool Equals(IUniqueIdentifier identifier)
        {
            return (identifier != null && identifier is UniquePath other && other.Inner.Equals(this));
        }
    }
}
