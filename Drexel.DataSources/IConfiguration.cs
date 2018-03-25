using System;
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

        /// <summary>
        /// Gets the <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/>
        /// <paramref name="requirement"/>.
        /// </returns>
        object this[IConfigurationRequirement requirement] { get; }

        /// <summary>
        /// If the specified <see cref="IConfigurationRequirement"/> <paramref name="requirement"/> is contained by
        /// this <see cref="IConfiguration"/>, returns the <see cref="object"/> held by the associated
        /// <see cref="IBinding"/> in <see cref="IConfiguration.Bindings"/>; otherwise, returns the result of invoking
        /// the supplied <paramref name="defaultValueFactory"/>.
        /// </summary>
        /// <param name="requirement">
        /// The <see cref="IConfigurationRequirement"/>.
        /// </param>
        /// <param name="defaultValueFactory">
        /// The default value factory.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> associated with the specified <see cref="IConfigurationRequirement"/> if it is
        /// contained by this <see cref="IConfiguration"/>; otherwise, the value returned by
        /// <paramref name="defaultValueFactory"/>.
        /// </returns>
        object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory);
    }
}
