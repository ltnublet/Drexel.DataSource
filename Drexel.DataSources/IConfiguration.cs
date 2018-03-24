using System.Collections.Generic;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a configuration applicable to an <see cref="IConfigurable"/>.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// A collection of <see cref="IBinding"/>s contained by this <see cref="IConfiguration"/>.
        /// </summary>
        IEnumerable<IBinding> Bindings { get; }
    }
}
