namespace Drexel.DataSources.Tests.Mocks
{
    public class MockDataSourceChangeEventArgs : IDataSourceChangeEventArgs<MockData>
    {
        private MockData change;

        public MockDataSourceChangeEventArgs(MockData change, DataSourceChangeEventType eventType)
        {
            this.change = data;
            this.EventType = eventType;
        }

        public DataSourceChangeEventType EventType { get; private set; }

        public MockData GetCurrentState() => this.change;
    }
}
