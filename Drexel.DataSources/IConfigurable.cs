using System;
using System.Collections.Generic;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a configurable <see cref="object"/>.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// The <see cref="IConfigurationRequirement"/>s accepted by this <see cref="IConfigurable"/>.
        /// </summary>
        IReadOnlyList<IConfigurationRequirement> Requirements { get; }

        /// <summary>
        /// Returns an <see cref="IConfiguration"/> applicable to this <see cref="IConfigurable"/>.
        /// </summary>
        /// <param name="bindings">
        /// A mapping of <see cref="IConfigurationRequirement"/> to <see cref="object"/>; used by the
        /// <see cref="IConfigurable"/> to produce the <see cref="IConfiguration"/>.
        /// </param>
        /// <returns>
        /// An <see cref="IConfiguration"/> applicable to this <see cref="IConfigurable"/> produced from the supplied
        /// <paramref name="bindings"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Occurs when the supplied <paramref name="bindings"/> are invalid, either for the
        /// <see cref="IConfigurationRequirement"/> they are mapped to, or the <see cref="IConfigurable"/>.
        /// </exception>
        IConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings);
    }
}
