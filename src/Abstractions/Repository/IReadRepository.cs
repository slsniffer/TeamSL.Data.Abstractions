using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Ordering;

namespace TeamSL.Data.Abstractions.Repository
{
    /// <summary>
    /// Read operations. To get a record by Id, pass <see cref="ById{TRecord}"/> or <see cref="ById{TRecord, TKey}"/>
    /// as the specification, e.g. <c>Find(new ById&lt;User&gt;(id))</c>.
    /// </summary>
    public interface IReadRepository<TRecord> where TRecord : class
    {
        Task<TRecord?> Find(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken = default);
        Task<TRecord?> Find(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken = default);
        /// <summary>Returns the first record that satisfies the specification in the given order, or null. Unlike <c>Find</c>, several matches are fine.</summary>
        Task<TRecord?> FindFirst(IQuerySpecification<TRecord> specification, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        /// <summary>Returns the first record that satisfies the specification in the given order, with related data loaded, or null. Unlike <c>Find</c>, several matches are fine.</summary>
        Task<TRecord?> FindFirst(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
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
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default);
        Task<IList<TRecord>> FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default);
        /// <summary>Returns true when at least one record exists.</summary>
        Task<bool> Exists(CancellationToken cancellationToken = default);
        /// <summary>Returns true when at least one record satisfies the specification.</summary>
        Task<bool> Exists(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken = default);
    }
}
