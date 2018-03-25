using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Drexel.DataSources.FolderData.Internationalization;

namespace Drexel.DataSources.FolderData
{
    /// <summary>
    /// Represents an <see cref="IDataSourceFactory{IFileInformation}"/> implementation which produces
    /// <see cref="FolderDataSource"/>s.
    /// </summary>
    [Export(typeof(IDataSourceFactory<IFileInformation>))]
    public class FolderDataSourceFactory : IDataSourceFactory<IFileInformation>
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
        public IDataSource<IFileInformation> MakeDataSource(IConfiguration configuration)
        {
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
        public IConfiguration Configure(IReadOnlyDictionary<IConfigurationRequirement, object> bindings)
        {
            return new Configuration(this, bindings);
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
