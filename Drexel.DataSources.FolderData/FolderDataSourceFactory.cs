using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Drexel.DataSources.External;
using Drexel.DataSources.ReferenceImplementation;
using Drexel.DataSources.FolderData.Internationalization;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IDataSourceFactory"/> implementation which produces <see cref="FolderDataSource"/>s.
    /// </summary>
    [Export(typeof(IDataSourceFactory))]
    public class FolderDataSourceFactory : IDataSourceFactory
    {
        private static IConfigurationRequirement path =
            ConfigurationRequirement.Path(en_us.RootPathName, en_us.RootPathDescription, false);
        private static IConfigurationRequirement factoryChangeFilterTicks =
            ConfigurationRequirement.Int64(en_us.ChangeFilterTicksName, en_us.ChangeFilterTicksDescription, true);
        private static IConfigurationRequirement factoryFilter =
            ConfigurationRequirement.String(en_us.FilterName, en_us.FilterDescription, true);
        private static IConfigurationRequirement watcherFactory =
            new ConfigurationRequirement(
                en_us.ConfigurationWatcherFactoryName,
                en_us.ConfigurationWatcherFactoryDescription,
                new ConfigurationRequirementType(typeof(IFolderDataWatcherFactory)),
                false,
                true,
                x => FolderDataSourceFactory.CheckTypeMatch(typeof(IFolderDataWatcherFactory), x),
                exclusiveWith: new IConfigurationRequirement[]
                {
                    FolderDataSourceFactory.factoryChangeFilterTicks,
                    FolderDataSourceFactory.factoryFilter
                });
        private static IConfigurationRequirement interactorFactory =
            new ConfigurationRequirement(
                en_us.ConfigurationInteractorFactoryName,
                en_us.ConfigurationInteractorFactoryDescription,
                new ConfigurationRequirementType(typeof(IDirectoryInteractorFactory)),
                false,
                true,
                x => FolderDataSourceFactory.CheckTypeMatch(typeof(IDirectoryInteractorFactory), x));

        private static IReadOnlyList<IConfigurationRequirement> requirements =
            new List<IConfigurationRequirement>()
            {
                FolderDataSourceFactory.path,
                FolderDataSourceFactory.factoryChangeFilterTicks,
                FolderDataSourceFactory.factoryFilter,
                FolderDataSourceFactory.watcherFactory,
                FolderDataSourceFactory.interactorFactory
            };

        /// <inheritdoc />
        public IReadOnlyList<IConfigurationRequirement> Requirements => FolderDataSourceFactory.requirements;

        /// <inheritdoc />
        public IDataSource MakeDataSource(IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            if (bindings == null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            List<Exception> failures = new List<Exception>();
            Dictionary<IConfigurationRequirement, object> successes =
                new Dictionary<IConfigurationRequirement, object>();
            foreach (IConfigurationRequirement requirement in this.Requirements)
            {
                bool present = bindings.TryGetValue(requirement, out object binding);
                if (!present && !requirement.IsOptional)
                {
                    failures.Add(
                        new ArgumentException(
                            string.Format(
                                en_us.Culture,
                                en_us.FolderDataSourceFactoryMissingRequirement,
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
                        successes.Add(requirement, binding);
                    }
                }
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                successes
                    .Where(x => !x.Key.DependsOn.All(y => successes.ContainsKey(y)))
                    .ToArray())
            {
                successes.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            en_us.Culture,
                            en_us.FolderDataSourceFactoryDependenciesNotSatisfied,
                            pair.Key.Name)));
            }

            foreach (KeyValuePair<IConfigurationRequirement, object> pair in
                successes
                    .Where(x => x.Key.ExclusiveWith.Any(y => successes.ContainsKey(y)))
                    .ToArray())
            {
                successes.Remove(pair.Key);
                failures.Add(
                    new ArgumentException(
                        string.Format(
                            en_us.Culture,
                            en_us.FolderDataSourceFactoryConflictingRequirementsSpecified,
                            pair.Key.Name)));
            }

            if (failures.Any())
            {
                throw new AggregateException(
                    en_us.FolderDataSourceFactoryRequirementsFailedValidation,
                    failures);
            }

            IFolderDataWatcherFactory factory =
                (IFolderDataWatcherFactory)(successes.GetOrDefault(
                    FolderDataSourceFactory.watcherFactory,
                    () => new FolderDataWatcherFactory(
                        (string)successes.GetOrDefault(FolderDataSourceFactory.factoryFilter, () => null),
                        (long)successes.GetOrDefault(FolderDataSourceFactory.factoryChangeFilterTicks, () => 0L))));

            return new FolderDataSource(
                (FilePath)successes[FolderDataSourceFactory.path],
                factory,
                (IDirectoryInteractorFactory)successes[FolderDataSourceFactory.interactorFactory]);
        }

        private static Exception CheckTypeMatch(Type expected, object actualInstance)
        {
            if (actualInstance != null)
            {
                Type actual = actualInstance.GetType();

                if (!expected.IsAssignableFrom(actual))
                {
                    return new ArgumentException(
                        string.Format(
                            en_us.Culture,
                            en_us.ConfigurationFactorySuppliedButWrongType,
                            actual.ToString()));
                }
            }

            return null;
        }
    }
}
