namespace Drexel.DataSources.FolderData.Tests.Mocks
{
    internal class MockFolderDataChangeEventArgs : IFolderDataChangeEventArgs
    {
        private IFileInformation information;

        public MockFolderDataChangeEventArgs(
            FilePath path,
            DataSourceChangeEventType type,
            IFileInformation information)
        {
            this.Path = path;
            this.Name = information.Name;
            this.EventType = type;
            this.information = information;
        }

        public DataSourceChangeEventType EventType { get; private set; }

        public string Name { get; private set; }

        public FilePath Path { get; private set; }

        public IFileInformation GetChange() => this.information;
    }
}
