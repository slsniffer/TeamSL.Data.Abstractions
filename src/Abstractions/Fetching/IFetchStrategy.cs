using System.Linq;

namespace TeamSL.Data.Abstractions.Fetching
{
    public interface IFetchStrategy<TRecord>
    {
        IQueryable<TRecord> Apply(IQueryable<TRecord> queryable);
    }
}
