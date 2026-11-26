using System.Linq;

namespace TeamSL.Data.Abstractions.Filtering
{
    public interface IQuerySpecification<TRecord>
    {
        IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates);
    }
}
