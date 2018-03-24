using System;
using System.Collections.Generic;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a configuration requirement of an <see cref="IDataSource{T}"/>.
    /// </summary>
    public interface IConfigurationRequirement
    {
        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        IEnumerable<IConfigurationRequirement> DependsOn { get; }

        /// <summary>
        /// The description of this requirement.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this requirement.
        /// </summary>
        IEnumerable<IConfigurationRequirement> ExclusiveWith { get; }

        /// <summary>
        /// <see langword="true"/> if this requirement requires a collection of
        /// <see cref="ConfigurationRequirementType"/> <see cref="OfType"/>; <see langword="false"/> if this
        /// requirement requires a single instance of <see cref="ConfigurationRequirementType"/> <see cref="OfType"/>.
        /// </summary>
        bool IsCollection { get; }

        /// <summary>
        /// <see langword="true"/> if this requirement is optional; <see langword="false"/> if this requirement is
        /// required.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// The type of this requirement. This indicates what the expected type of the input to
        /// <see cref="Validate(object)"/> is.
        /// </summary>
        ConfigurationRequirementType OfType { get; }

        /// <summary>
        /// The name of this requirement.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Validates the supplied <see cref="object"/> for this requirement.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="object"/> to perform validation upon.
        /// </param>
        /// <returns>
        /// <see langword="null"/> if the supplied <see cref="object"/> <paramref name="instance"/> passed validation;
        /// an <see cref="Exception"/> describing the validation failure otherwise.
        /// </returns>
        Exception Validate(object instance);
    }
}
