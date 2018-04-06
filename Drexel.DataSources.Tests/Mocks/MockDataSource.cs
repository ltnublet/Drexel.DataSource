using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drexel.DataSources.Tests.Mocks
{
    public class MockDataSource : IDataSource<MockData>
    {
        private List<MockData> mockData;
        private Func<MockData, MockDataSourceChangeEventArgs> addEventFactory;
        private Func<MockData, MockDataSourceChangeEventArgs> removeEventFactory;

        public MockDataSource(
            Func<MockData, MockDataSourceChangeEventArgs> addEventFactory,
            Func<MockData, MockDataSourceChangeEventArgs> removeEventFactory,
            out Action<MockDataSourceChangeEventArgs> invokeChange,
            IEnumerable<MockData> initialData = null)
        {
            this.mockData = initialData == null ? new List<MockData>() : new List<MockData>(initialData);
            this.addEventFactory = addEventFactory;
            this.removeEventFactory = removeEventFactory;
            invokeChange = x => this.OnChange.Invoke(this, x);
        }

        public event EventHandler<IDataSourceChangeEventArgs<MockData>> OnChange;

        public int Count => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Add(MockData data)
        {
            this.mockData.Add(data);

            IDataSourceChangeEventArgs<MockData> args = this.addEventFactory == null
                ? new MockDataSourceChangeEventArgs(data, DataSourceChangeEventType.Added)
                : this.addEventFactory.Invoke(data);

            this.OnChange.Invoke(this, args)
        }

        public void Change(MockData toChange, Func<MockData, MockData> transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            int index = this.mockData.IndexOf(toChange);

            if (index == -1)
            {
                throw new InvalidOperationException("Specified data wasn't in collection.");
            }

            this.mockData[index] = transform.Invoke(toChange);

            this.OnChange.Invoke(this, this.mockData[index])
        }

        public void Clear()
        {
            foreach (MockData data in this.mockData)
            {
                this.mockData.Remove(data);

                IDataSourceChangeEventArgs<MockData> args = this.removeEventFactory == null
                    ? new MockDataSourceChangeEventArgs(data, DataSourceChangeEventType.Removed)
                    : this.removeEventFactory.Invoke(data);

                this.OnChange.Invoke(this, args);
            }
        }

        public void Remove(MockData data)
        {
            this.mockData.Remove(data);

            IDataSourceChangeEventArgs<MockData> args = this.removeEventFactory == null
                ? new MockDataSourceChangeEventArgs(data, DataSourceChangeEventType.Removed)
                : this.removeEventFactory.Invoke(data);
        }

        public IEnumerator<MockData> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IDataSource<MockData>> GetSubDataSources()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
