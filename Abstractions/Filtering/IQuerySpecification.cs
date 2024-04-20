using System.Linq;

namespace TeamSL.Data.Abstractions.Filtering
{
    public interface IQuerySpecification<T>
    {
        IQueryable<T> Satisfy(IQueryable<T> candidates);
    }
}