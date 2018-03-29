using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Drexel.DataSources
{
    /// <summary>
    /// A simple implementation of <see cref="IBoundConfiguration"/>.
    /// </summary>
    public class BoundConfiguration : IBoundConfiguration
    {
        private const string ConfigurableRequirementsMustNotBeNull = "Configuration requirements must not be null.";
        private const string MissingRequirement = "Missing required argument. Name: '{0}'.";
        private const string DependenciesNotSatisfied = "Argument name '{0}' does not have its dependencies fulfilled.";
        private const string ConflictingRequirementsSpecified = "Argument name '{0}' has conflicting requirements specified.";
        private const string RequirementsFailedValidation = "Supplied requirements failed validation.";

        private Dictionary<IConfigurationRequirement, object> backingDictionary;
        private Lazy<IBinding[]> backingBindings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundConfiguration"/> class.
        /// </summary>
        /// <param name="configurable">
        /// The configurable.
        /// </param>
        /// <param name="bindings">
        /// The bindings.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Occurs when the specified <paramref name="bindings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="AggregateException">
        /// Occurs when the specified <paramref name="bindings"/> is invalid; the
        /// <see cref="AggregateException.InnerExceptions"/> are the specific reasons the <paramref name="bindings"/>
        /// were invalid.
        /// </exception>
        public BoundConfiguration(
            IConfigurable configurable,
            IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            if (configurable == null)
            {
                throw new ArgumentNullException(nameof(configurable));
            }

            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            if (configurable.Requirements == null)
            {
                throw new ArgumentException(
                    BoundConfiguration.ConfigurableRequirementsMustNotBeNull,
                    nameof(configurable));
            }

            List<Exception> failures = new List<Exception>();
            this.backingDictionary = new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in configurable.Requirements)
            {
                bool present = bindings.TryGetValue(requirement, out object binding);
                if (!present && !requirement.IsOptional)
                {
                    failures.Add(
                        new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                BoundConfiguration.MissingRequirement,
                                requirement.Name)));
                }
                else if (present)
                {
                    Exception exception = requirement.Validate(binding);

                    if (exception != null)
                    {
                        failures.Add(exception);
                    }
                    else
                    {
                        this.backingDictionary.Add(requirement, binding);
                    }
                }
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                this.backingDictionary
                    .Where(x => !x.Key.DependsOn.All(y => this.backingDictionary.ContainsKey(y)))
                    .ToArray())
            {
                this.backingDictionary.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            BoundConfiguration.DependenciesNotSatisfied,
                            pair.Key.Name)));
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                this.backingDictionary
                    .Where(x => x.Key.ExclusiveWith.Any(y => this.backingDictionary.ContainsKey(y)))
                    .ToArray())
            {
                this.backingDictionary.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            BoundConfiguration.ConflictingRequirementsSpecified,
                            pair.Key.Name)));
            }

            if (failures.Any())
            {
                throw new AggregateException(
                    BoundConfiguration.RequirementsFailedValidation,
                    failures);
            }

            this.backingBindings =
                new Lazy<IBinding[]>(() => this.backingDictionary.Select(x => new Binding(x.Key, x.Value)).ToArray());
        }

        /// <inheritdoc />
        public IEnumerable<IBinding> Bindings => this.backingBindings.Value;

        /// <inheritdoc />
        public object this[IConfigurationRequirement requirement] => this.backingDictionary[requirement];

        /// <inheritdoc />
        public object GetOrDefault(IConfigurationRequirement requirement, Func<object> defaultValueFactory)
        {
            if (defaultValueFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultValueFactory));
            }

            if (!this.backingDictionary.TryGetValue(requirement, out object result))
            {
                result = defaultValueFactory.Invoke();
            }

            return result;
        }
    }
}
