using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public interface IQuerySpecification<TRecord, TKey> where TRecord : IRecord<TKey>
    {
        IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates);
    }

    public interface IQuerySpecification<TRecord> : IQuerySpecification<TRecord, long>
        where TRecord : IRecord
    {
    }
}