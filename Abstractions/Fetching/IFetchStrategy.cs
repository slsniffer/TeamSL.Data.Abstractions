using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Fetching
{
    public interface IFetchStrategy<TRecord> where TRecord : Record
    {
        IQueryable<TRecord> Apply(IQueryable<TRecord> queryable);
    }
}