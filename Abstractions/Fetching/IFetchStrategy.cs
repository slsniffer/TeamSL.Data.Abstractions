using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Fetching
{
    public interface IFetchStrategy<TRecord> : IFetchStrategy<TRecord, long>
        where TRecord : IRecord
    {
    }

    public interface IFetchStrategy<TRecord, TKey> where TRecord : IRecord<TKey>
    {
        IQueryable<TRecord> Apply(IQueryable<TRecord> queryable);
    }
}