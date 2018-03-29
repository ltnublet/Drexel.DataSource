using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Drexel.DataSources.FolderData.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.DataSources.FolderData.Tests
{
    [TestClass]
    public class FolderDataSourceTests
    {
        private const string RootPath = "Root Path";

        private static readonly IReadOnlyCollection<FilePath> SuppliedFiles = new FilePath[]
        {
            new FilePath("fileOne.txt"),
            new FilePath("fileTwo.txt"),
            new FilePath("fileThree.txt")
        };

        private static readonly IReadOnlyCollection<FilePath> SuppliedDirectories = new FilePath[]
        {
            new FilePath("Directory One"),
            new FilePath("Directory Two"),
            new FilePath("Directory Three")
        };

        [TestMethod]
        public void FolderDataSourceFactory_MakeDataSource()
        {
            IDataSource<IFileInformation> source = FolderDataSourceTests.CreateFolderDataSource().Source;

            try
            {
                CollectionAssert.AreEquivalent(
                    FolderDataSourceTests.SuppliedFiles.Select(x => x.Path).ToArray(),
                    source.Select(x => x.Name).ToArray());
                CollectionAssert.AreEquivalent(
                    FolderDataSourceTests.SuppliedDirectories.Select(x => x.Path).ToArray(),
                    source.GetSubDataSources().Select(x => x.Name).ToArray());
            }
            finally
            {
                source.Dispose();
            }
        }

        [TestMethod]
        public void FolderDataSource_OnChange()
        {
            const int expectedAdded = 3;
            const int expectedChanged = 5;
            const int expectedMoved = 2;
            const int expectedRemoved = 12;

            const string addedPathFormatString = @"C:\Just\Got\Added{0}.txt";
            const string changedPathFormatString = @"C:\Just\Got\Changed{0}.txt";
            const string movedPathFormatString = @"C:\Just\Got\Moved{0}.txt";
            const string removedPathFormatString = @"C:\Just\Got\Removed{0}.txt";

            DataSourceTuple tuple = FolderDataSourceTests.CreateFolderDataSource();

            try
            {
                int actualAdded = 0;
                int actualChanged = 0;
                int actualMoved = 0;
                int actualRemoved = 0;

                tuple.Source.OnChange += (obj, e) =>
                {
                    if (e.EventType == DataSourceChangeEventType.Added)
                    {
                        actualAdded++;
                    }
                    else if (e.EventType == DataSourceChangeEventType.Changed)
                    {
                        actualChanged++;
                    }
                    else if (e.EventType == DataSourceChangeEventType.Moved)
                    {
                        actualMoved++;
                    }
                    else if (e.EventType == DataSourceChangeEventType.Removed)
                    {
                        actualRemoved++;
                    }
                    else
                    {
                        throw new InternalTestFailureException("Encountered unexpected `DataSourceChangeEventType`.");
                    }
                };

                for (int counter = 0; counter < expectedAdded; counter++)
                {
                    tuple.InvokeAdded.Invoke(
                        null,
                        new MockFolderDataChangeEventArgs(
                            new FilePath(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    addedPathFormatString,
                                    counter)),
                            DataSourceChangeEventType.Added,
                            null));
                }

                for (int counter = 0; counter < expectedChanged; counter++)
                {
                    tuple.InvokeChanged.Invoke(
                        null,
                        new MockFolderDataChangeEventArgs(
                            new FilePath(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    changedPathFormatString,
                                    counter)),
                            DataSourceChangeEventType.Changed,
                            null));
                }

                for (int counter = 0; counter < expectedMoved; counter++)
                {
                    tuple.InvokeMoved.Invoke(
                        null,
                        new MockFolderDataChangeEventArgs(
                            new FilePath(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    movedPathFormatString,
                                    counter)),
                            DataSourceChangeEventType.Moved,
                            null));
                }

                for (int counter = 0; counter < expectedRemoved; counter++)
                {
                    tuple.InvokeRemoved.Invoke(
                        null,
                        new MockFolderDataChangeEventArgs(
                            new FilePath(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    removedPathFormatString,
                                    counter)),
                            DataSourceChangeEventType.Removed,
                            null));
                }

                Assert.AreEqual(expectedAdded, actualAdded);
                Assert.AreEqual(expectedChanged, actualChanged);
                Assert.AreEqual(expectedMoved, actualMoved);
                Assert.AreEqual(expectedRemoved, actualRemoved);
            }
            finally
            {
                tuple.Source.Dispose();
            }
        }

        private static DataSourceTuple CreateFolderDataSource()
        {
            MockDirectoryInteractorFactory mockDirectoryInteractorFactory =
                new MockDirectoryInteractorFactory(
                    FolderDataSourceTests.SuppliedFiles,
                    FolderDataSourceTests.SuppliedDirectories);

            Action<object, IFolderDataChangeEventArgs> added = null;
            Action<object, IFolderDataChangeEventArgs> changed = null;
            Action<object, IFolderDataChangeEventArgs> moved = null;
            Action<object, IFolderDataChangeEventArgs> removed = null;

            MockFolderDataWatcherFactory mockFolderDataWatcherFactory =
                new MockFolderDataWatcherFactory(
                    _ =>
                    {
                        MockFolderDataWatcher result = new MockFolderDataWatcher(
                            out Action<object, IFolderDataChangeEventArgs> invokeAdded,
                            out Action<object, IFolderDataChangeEventArgs> invokeChanged,
                            out Action<object, IFolderDataChangeEventArgs> invokeMoved,
                            out Action<object, IFolderDataChangeEventArgs> invokeRemoved);

                        added = invokeAdded;
                        changed = invokeChanged;
                        moved = invokeMoved;
                        removed = invokeRemoved;

                        return result;
                    });

            FolderDataSourceFactory factory = new FolderDataSourceFactory();
            IReadOnlyDictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [factory.Requirements[0]] = new FilePath(FolderDataSourceTests.RootPath),
                    [factory.Requirements[3]] = mockFolderDataWatcherFactory,
                    [factory.Requirements[4]] = mockDirectoryInteractorFactory
                };

            IBoundConfiguration configuration = factory.Configure(bindings);

            return new DataSourceTuple(
                factory.MakeDataSource(configuration),
                added,
                changed,
                moved,
                removed);
        }

        private class DataSourceTuple
        {
            public DataSourceTuple(
                IDataSource<IFileInformation> source,
                Action<object, IFolderDataChangeEventArgs> invokeAdded,
                Action<object, IFolderDataChangeEventArgs> invokeChanged,
                Action<object, IFolderDataChangeEventArgs> invokeMoved,
                Action<object, IFolderDataChangeEventArgs> invokeRemoved)
            {
                this.Source = source;
                this.InvokeAdded = invokeAdded;
                this.InvokeChanged = invokeChanged;
                this.InvokeMoved = invokeMoved;
                this.InvokeRemoved = invokeRemoved;
            }

            public IDataSource<IFileInformation> Source { get; private set; }

            public Action<object, IFolderDataChangeEventArgs> InvokeAdded { get; private set; }

            public Action<object, IFolderDataChangeEventArgs> InvokeChanged { get; private set; }

            public Action<object, IFolderDataChangeEventArgs> InvokeMoved { get; private set; }

            public Action<object, IFolderDataChangeEventArgs> InvokeRemoved { get; private set; }
        }
    }
}
