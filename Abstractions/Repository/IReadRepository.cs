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
    public interface IReadRepository<TRecord> where TRecord : Record
    {
        Task<TRecord> Load(long id, CancellationToken cancellationToken = default);
        Task<TRecord> Load(long id, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken = default);
        Task<TRecord> Find(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken = default);
        Task<TRecord> Find(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken = default);
        Task<int> Count(CancellationToken cancellationToken = default);
        Task<int> Count(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
    }
}