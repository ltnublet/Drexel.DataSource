using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Drexel.Configurables;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.External;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IDataSourceFactory{IFileInformation}"/> implementation which produces
    /// <see cref="FolderDataSource"/>s.
    /// </summary>
    [Export(typeof(IDataSourceFactory<IFileInformation>))]
    public class FolderDataSourceFactory : IDataSourceFactory<IFileInformation>
    {
        private const string RootPathName = "Root Path";
        private const string RootPathDescription = "The root path to monitor for changes.";
        private const string ChangeFilterTicksName = "Change Filter Ticks";
        private const string ChangeFilterTicksDescription = "The number of ticks to wait between change events firing.";
        private const string FilterName = "Filter Name";
        private const string FilterDescription = "The filter to apply to filenames when determining if it should be monitored.";
        private const string InteractorFactoryName = "DirectoryInteractor Factory";
        private const string InteractorFactoryDescription = "Overrides the default DirectoryInteractor factory.";
        private const string WatcherFactoryName = "FolderDataWatcher Factory";
        private const string WatcherFactoryDescription = "Overrides the default FolderDataWatcher factory.";

        private const string FactorySuppliedButWrongType = "Expected a factory of type {0}, but got {1}.";

        private static IConfigurationRequirement path =
            ConfigurationRequirement.FilePath(
                FolderDataSourceFactory.RootPathName,
                FolderDataSourceFactory.RootPathDescription,
                false);
        private static IConfigurationRequirement factoryChangeFilterTicks =
            ConfigurationRequirement.Int64(
                FolderDataSourceFactory.ChangeFilterTicksName,
                FolderDataSourceFactory.ChangeFilterTicksDescription,
                true);
        private static IConfigurationRequirement factoryFilter =
            ConfigurationRequirement.String(
                FolderDataSourceFactory.FilterName,
                FolderDataSourceFactory.FilterDescription,
                true);
        private static IConfigurationRequirement interactorFactory =
            new ConfigurationRequirement(
                FolderDataSourceFactory.InteractorFactoryName,
                FolderDataSourceFactory.InteractorFactoryDescription,
                new ConfigurationRequirementType(typeof(IDirectoryInteractorFactory)),
                false,
                validator: (x, y, z) =>
                    FolderDataSourceFactory.CheckTypeMatch(typeof(IDirectoryInteractorFactory), y));
        private static IConfigurationRequirement watcherFactory =
            new ConfigurationRequirement(
                FolderDataSourceFactory.WatcherFactoryName,
                FolderDataSourceFactory.WatcherFactoryDescription,
                new ConfigurationRequirementType(typeof(IFolderDataWatcherFactory)),
                false,
                validator: (x, y, z) => FolderDataSourceFactory.CheckTypeMatch(typeof(IFolderDataWatcherFactory), y),
                exclusiveWith: new IConfigurationRequirement[]
                {
                    FolderDataSourceFactory.factoryChangeFilterTicks,
                    FolderDataSourceFactory.factoryFilter
                });

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
        public IDataSource<IFileInformation> MakeDataSource(IBoundConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            IFolderDataWatcherFactory factory =
                (IFolderDataWatcherFactory)(configuration.GetOrDefault(
                    FolderDataSourceFactory.watcherFactory,
                    () => new FolderDataWatcherFactory(
                        (string)configuration.GetOrDefault(FolderDataSourceFactory.factoryFilter, () => null),
                        (long)configuration.GetOrDefault(FolderDataSourceFactory.factoryChangeFilterTicks, () => 0L))));

            return new FolderDataSource(
                (FilePath)configuration[FolderDataSourceFactory.path],
                factory,
                (IDirectoryInteractorFactory)configuration[FolderDataSourceFactory.interactorFactory]);
        }

        /// <inheritdoc />
        public IBoundConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            return new BoundConfiguration(this, bindings);
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
                            CultureInfo.InvariantCulture,
                            FolderDataSourceFactory.FactorySuppliedButWrongType,
                            expected.ToString(),
                            actual.GetType().ToString()));
                }
            }

            return null;
        }
    }
}
