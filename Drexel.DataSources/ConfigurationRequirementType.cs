using System;
using System.Security;

namespace Drexel.DataSources
{
    /// <summary>
    /// Represents a configuration type.
    /// </summary>
    public sealed class ConfigurationRequirementType
    {
        /// <summary>
        /// Instantiates a new <see cref="ConfigurationRequirementType"/> of the specified <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="System.Type"/> of the <see cref="ConfigurationRequirementType"/>.
        /// </param>
        public ConfigurationRequirementType(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see langword="string"/>.
        /// </summary>
        public static ConfigurationRequirementType String { get; } =
            new ConfigurationRequirementType(typeof(string));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see langword="int"/>.
        /// </summary>
        public static ConfigurationRequirementType Int32 { get; } =
            new ConfigurationRequirementType(typeof(int));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see langword="long"/>.
        /// </summary>
        public static ConfigurationRequirementType Int64 { get; } =
            new ConfigurationRequirementType(typeof(long));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see langword="bool"/>.
        /// </summary>
        public static ConfigurationRequirementType Bool { get; } =
            new ConfigurationRequirementType(typeof(bool));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="FilePath"/>.
        /// </summary>
        public static ConfigurationRequirementType Path { get; } =
            new ConfigurationRequirementType(typeof(FilePath));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type <see cref="System.Uri"/>.
        /// </summary>
        public static ConfigurationRequirementType Uri { get; } =
            new ConfigurationRequirementType(typeof(Uri));

        /// <summary>
        /// Indicates the associated <see cref="IConfigurationRequirement"/> is of type
        /// <see cref="System.Security.SecureString"/>.
        /// </summary>
        public static ConfigurationRequirementType SecureString { get; } =
            new ConfigurationRequirementType(typeof(SecureString));

        /// <summary>
        /// The <see cref="System.Type"/> of the associated <see cref="IConfigurationRequirement"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "The distinction between the inherited method GetType and the property Type is clear.")]
        public Type Type { get; private set; }
    }
}
