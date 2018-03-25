namespace Drexel.DataSources
{
    /// <summary>
    /// Represents the type of an <see cref="IDataSourceChangeEventArgs{T}"/>.
    /// </summary>
    public class DataSourceChangeEventType
    {
        private const string AddedName = "Added";
        private const string ChangedName = "Changed";
        private const string MovedName = "Moved";
        private const string RemovedName = "Removed";

        private string name;

        private DataSourceChangeEventType(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Indicates that the associated <see cref="IDataSourceChangeEventArgs{T}"/> was triggered by an addition.
        /// </summary>
        public static DataSourceChangeEventType Added { get; } = new DataSourceChangeEventType(AddedName);

        /// <summary>
        /// Indicates that the associated <see cref="IDataSourceChangeEventArgs{T}"/> was triggered by a change.
        /// </summary>
        public static DataSourceChangeEventType Changed { get; } = new DataSourceChangeEventType(ChangedName);

        /// <summary>
        /// Indicates that the associated <see cref="IDataSourceChangeEventArgs{T}"/> was triggered by a move.
        /// </summary>
        public static DataSourceChangeEventType Moved { get; } = new DataSourceChangeEventType(MovedName);

        /// <summary>
        /// Indicates that the associated <see cref="IDataSourceChangeEventArgs{T}"/> was triggered by a removal.
        /// </summary>
        public static DataSourceChangeEventType Removed { get; } = new DataSourceChangeEventType(RemovedName);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">
        /// An <see cref="object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> refers to the same reference as this instance; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return object.ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            const int added = 0;
            const int changed = 1;
            const int moved = 2;
            const int removed = 3;
            const int unknown = -1;

            if (this.Equals(DataSourceChangeEventType.Added))
            {
                return added;
            }
            else if (this.Equals(DataSourceChangeEventType.Changed))
            {
                return changed;
            }
            else if (this.Equals(DataSourceChangeEventType.Moved))
            {
                return moved;
            }
            else if (this.Equals(DataSourceChangeEventType.Removed))
            {
                return removed;
            }
            else
            {
                return unknown;
            }
        }

        /// <summary>
        /// Returns a string that represents this <see cref="DataSourceChangeEventType"/> instance.
        /// </summary>
        /// <returns>
        /// A string that represents this <see cref="DataSourceChangeEventType"/> instance.
        /// </returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
