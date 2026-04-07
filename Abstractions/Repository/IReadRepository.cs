using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Ordering;

namespace TeamSL.Data.Abstractions.Repository
{
    public interface IReadRepository<TRecord> : IReadRepository<TRecord, long>
    where TRecord : IRecord
    {
    }

    public interface IReadRepository<TRecord, TKey> where TRecord : IRecord<TKey>
    {
        Task<TRecord> Load(TKey id, CancellationToken cancellationToken = default);
        Task<TRecord> Load(TKey id, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default);
        Task<TRecord> Find(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default);
        Task<TRecord> Find(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default);
        Task<int> Count(CancellationToken cancellationToken = default);
        Task<int> Count(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
    }
}