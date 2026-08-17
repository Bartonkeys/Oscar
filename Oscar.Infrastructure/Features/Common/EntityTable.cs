using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oscar.Infrastructure.Features.Common
{
    public interface IEntityTable<out T> where T : new()
    {
        IEntityTable<T> WithTotal(int total);
        int TotalRecords { get; }
        IEnumerable<T> Records { get; }
    }

    public class EntityTable<T> : IEntityTable<T> where T : new()
    {
        public int TotalRecords { get; private set; }
        public IEnumerable<T> Records { get; private set; }

        private EntityTable() { }

        public static IEntityTable<T> Create(IEnumerable<T> records)
        {
            return new EntityTable<T> { Records = records };
        }

        public IEntityTable<T> WithTotal(int total)
        {
            TotalRecords = total;
            return this;
        }
    }
}
