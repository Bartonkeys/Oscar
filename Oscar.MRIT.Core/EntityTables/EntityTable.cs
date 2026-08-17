namespace Oscar.MRIT.Core.EntityTables
{
    public class EntityTable<T> where T : new()
    {
        public int TotalRecords { get; private set; }
        public IEnumerable<T> Records { get; private set; }

        private EntityTable() { }

        public static EntityTable<T> Create(IEnumerable<T> records)
        {
            return new EntityTable<T> { Records = records };
        }

        public EntityTable<T> WithTotal(int total)
        {
            TotalRecords = total;
            return this;
        }
    }
}
