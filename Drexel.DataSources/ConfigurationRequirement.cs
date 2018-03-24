using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Drexel.DataSources
{
    /// <summary>
    /// Validates a supplied <see cref="object"/>.
    /// </summary>
    /// <param name="instance">
    /// The <see cref="object"/> to validate.
    /// </param>
    /// <returns>
    /// <see langword="null"/> if the object passed validation; else, an <see cref="Exception"/> describing why the
    /// supplied <paramref name="instance"/> failed validation.
    /// </returns>
    public delegate Exception Validator(object instance);

    /// <summary>
    /// A simple <see cref="IConfigurationRequirement"/> implementation.
    /// </summary>
    public class ConfigurationRequirement : IConfigurationRequirement
    {
        private const string SuppliedObjectIsOfWrongType = "Supplied object is of wrong type. Expected type: '{0}'.";
        private const string StringMustBeNonWhitespace = "String must not be whitespace.";

        private Validator validator;

        /// <summary>
        /// Instantiates a new <see cref="ConfigurationRequirement"/> instance using the supplied parameters.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="ofType">
        /// The <see cref="ConfigurationRequirementType"/> of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isCollection">
        /// Indicates whether this <see cref="ConfigurationRequirement"/> is for a single instance of the type
        /// indicated by <paramref name="ofType"/>, or a collection.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether this <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="validator">
        /// Validates <see cref="object"/>s to determine if they satisfy the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        public ConfigurationRequirement(
            string name,
            string description,
            ConfigurationRequirementType ofType,
            bool isCollection,
            bool isOptional,
            Validator validator,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            ConfigurationRequirement.ThrowIfBadString(name, nameof(name));
            ConfigurationRequirement.ThrowIfBadString(description, nameof(description));
            this.Name = name;
            this.Description = description;
            this.OfType = ofType;
            this.IsCollection = isCollection;
            this.IsOptional = isOptional;
            this.DependsOn = dependsOn ?? new IConfigurationRequirement[0];
            this.ExclusiveWith = exclusiveWith ?? new IConfigurationRequirement[0];
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <inheritdoc />
        public IEnumerable<IConfigurationRequirement> DependsOn { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IConfigurationRequirement> ExclusiveWith { get; private set; }

        /// <inheritdoc />
        public bool IsCollection { get; private set; }

        /// <inheritdoc />
        public bool IsOptional { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public ConfigurationRequirementType OfType { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.String"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.String"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement String(
            string name,
            string description,
            bool isOptional,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.String,
                false,
                isOptional,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.String.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.String.Type.ToString()));
                    }

                    return null;
                },
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Path"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Path"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Path(
            string name,
            string description,
            bool isOptional,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Path,
                false,
                isOptional,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.Path.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.Path.Type.ToString()));
                    }

                    return null;
                },
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Int64"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether the <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Int64"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Int64(
            string name,
            string description,
            bool isOptional,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Int64,
                false,
                isOptional,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.Int64.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.Int64.Type.ToString()));
                    }

                    return null;
                },
                dependsOn,
                exclusiveWith);
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "By design, no exception should escape the validation call.")]
        public Exception Validate(object instance)
        {
            try
            {
                return this.validator.Invoke(instance);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            string JsonEscape(string toEscape)
            {
                StringBuilder builder = new StringBuilder();
                foreach (char @char in toEscape)
                {
                    switch (@char)
                    {
                        case '\b':
                            builder.Append(@"\b");
                            break;
                        case '\f':
                            builder.Append(@"\f");
                            break;
                        case '\n':
                            builder.Append(@"\n");
                            break;
                        case '\r':
                            builder.Append(@"\r");
                            break;
                        case '\t':
                            builder.Append(@"\t");
                            break;
                        case '\"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append(@"\");
                            break;
                        default:
                            builder.Append(@char);
                            break;
                    }
                }

                return builder.ToString();
            }

            const string @null = "<null>";

            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ \"{0}\": \"{1}\", \"{2}\": \"{3}\", \"{4}\": \"{5}\", \"{6}\": \"{7}\", \"{8}\": \"{9}\", \"{10}\": [{11}], \"{12}\": [{13}] }}",
                nameof(this.Name),
                JsonEscape(this.Name),
                nameof(this.OfType),
                JsonEscape(this.OfType.ToString()),
                nameof(this.IsCollection),
                JsonEscape(this.IsCollection.ToString()),
                nameof(this.IsOptional),
                JsonEscape(this.IsOptional.ToString()),
                nameof(this.Description),
                JsonEscape(this.Description),
                nameof(this.DependsOn),
                JsonEscape(string.Join(", ", this.DependsOn.Select(x => "\"" + (x?.Name ?? @null) + "\""))),
                nameof(this.ExclusiveWith),
                JsonEscape(string.Join(", ", this.ExclusiveWith.Select(x => "\"" + (x?.Name ?? @null) + "\""))));
        }

        [DebuggerHidden]
        private static void ThrowIfBadString(string @string, string name)
        {
            if (@string == null)
            {
                throw new ArgumentNullException(name);
            }
            else if (string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentException(ConfigurationRequirement.StringMustBeNonWhitespace, name);
            }
        }
    }
}
