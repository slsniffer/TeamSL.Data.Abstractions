using System.Linq;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Repository
{
    public abstract class RepositoryImpl<TRecord> : RepositoryImpl<TRecord, long>
        where TRecord : IRecord
    {
    }

    public abstract class RepositoryImpl<TRecord, TKey> where TRecord : IRecord<TKey>
    {
        protected abstract IQueryable<TRecord> Table { get; }

        protected IQueryable<TRecord> Load(TKey id, IFetchStrategy<TRecord, TKey> fetchStrategy)
        {
            Checks.NotNull(fetchStrategy, nameof(fetchStrategy));
            var queryable = Table.Where(x => x.Id!.Equals(id));
            return queryable.Fetch(fetchStrategy);
        }

        protected IQueryable<TRecord> Find(IQuerySpecification<TRecord, TKey> specification)
        {
            Checks.NotNull(specification, nameof(specification));
            return Table.FilterBy(specification);
        }

        protected IQueryable<TRecord> Find(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy)
        {
            Checks.NotNull(specification, nameof(specification));
            Checks.NotNull(fetchStrategy, nameof(fetchStrategy));
            return Table.FilterBy(specification).Fetch(fetchStrategy);
        }

        protected int Count()
        {
            return Table.Count();
        }

        protected int Count(IQuerySpecification<TRecord, TKey> specification)
        {
            Checks.NotNull(specification, nameof(specification));
            return Table.FilterBy(specification).Count();
        }

        protected IQueryable<TRecord> FindAll()
        {
            return Table;
        }

        protected IQueryable<TRecord> FindAll(IQuerySpecification<TRecord, TKey> specification)
        {
            Checks.NotNull(specification, nameof(specification));
            return Table.FilterBy(specification);
        }

        protected IQueryable<TRecord> FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy)
        {
            Checks.NotNull(fetchStrategy, nameof(fetchStrategy));
            return Table.Fetch(fetchStrategy);
        }

        protected IQueryable<TRecord> FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy)
        {
            Checks.NotNull(specification, nameof(specification));
            Checks.NotNull(fetchStrategy, nameof(fetchStrategy));
            return Table.FilterBy(specification).Fetch(fetchStrategy);
        }
    }
}