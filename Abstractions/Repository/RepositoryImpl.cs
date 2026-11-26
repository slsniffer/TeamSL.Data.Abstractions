using System.Linq;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Repository
{
    /// <summary>Query-composition helpers shared by repository implementations.</summary>
    public abstract class RepositoryImpl<TRecord>
    {
        protected abstract IQueryable<TRecord> Table { get; }

        protected IQueryable<TRecord> Find(IQuerySpecification<TRecord> specification)
            => Table.FilterBy(specification);

        protected IQueryable<TRecord> Find(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy)
            => Table.FilterBy(specification).Fetch(fetchStrategy);

        protected IQueryable<TRecord> FindAll()
            => Table;

        protected IQueryable<TRecord> FindAll(IQuerySpecification<TRecord> specification)
            => Table.FilterBy(specification);

        protected IQueryable<TRecord> FindAll(IFetchStrategy<TRecord> fetchStrategy)
            => Table.Fetch(fetchStrategy);

        protected IQueryable<TRecord> FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy)
            => Table.FilterBy(specification).Fetch(fetchStrategy);
    }
}
