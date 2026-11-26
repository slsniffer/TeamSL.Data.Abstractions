using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Ordering;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

public class EntityFrameworkRepository<TRecord> : RepositoryImpl<TRecord>, IReadRepository<TRecord>
    where TRecord : class
{
    private static readonly string Entity = typeof(TRecord).Name;

    private readonly DbContext _context;
    private readonly ILogger<EntityFrameworkRepository<TRecord>> _logger;

    public EntityFrameworkRepository(IContextProvider contextProvider, ILogger<EntityFrameworkRepository<TRecord>> logger)
    {
        ArgumentNullException.ThrowIfNull(contextProvider);
        ArgumentNullException.ThrowIfNull(logger);
        _context = contextProvider.Context();
        _logger = logger;
    }

    /// <summary>
    /// Queries always track the returned records, even if the DbContext is configured as no-tracking,
    /// so loading a record, changing it and calling <see cref="IUnitOfWork.SaveChanges"/> persists the change.
    /// </summary>
    protected override IQueryable<TRecord> Table => _context.Set<TRecord>().AsTracking();

    // ---- IReadRepository ----

    async Task<TRecord?> IReadRepository<TRecord>.Find(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find {Entity} by specification: {Specification}", Entity, Name(specification));

        return await Find(specification).SingleOrDefaultAsync(cancellationToken);
    }

    async Task<TRecord?> IReadRepository<TRecord>.Find(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find {Entity} by specification: {Specification} with fetching: {FetchStrategy}", Entity, Name(specification), Name(fetchStrategy));

        return await Find(specification, fetchStrategy).SingleOrDefaultAsync(cancellationToken);
    }

    async Task<int> IReadRepository<TRecord>.Count(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Counting {Entity}", Entity);

        return await Table.CountAsync(cancellationToken);
    }

    async Task<int> IReadRepository<TRecord>.Count(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Counting {Entity} with spec: {Specification}", Entity, Name(specification));

        return await Table.FilterBy(specification).CountAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity}", Entity);

        return await FindAll().ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(Action<Orderable<TRecord>> order, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with ordering", Entity);

        return await FindAll().OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with ordering, skip:{Skip}, count:{Count}", Entity, skip, count);

        return await FindAll().OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification}", Entity, Name(specification));

        return await FindAll(specification).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, Action<Orderable<TRecord>> order, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification} and ordering", Entity, Name(specification));

        return await FindAll(specification).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification} and ordering, skip:{Skip}, count:{Count}", Entity, Name(specification), skip, count);

        return await FindAll(specification).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification}, fetch:{FetchStrategy}", Entity, Name(specification), Name(fetchStrategy));

        return await FindAll(specification, fetchStrategy).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification}, fetch:{FetchStrategy} and ordering", Entity, Name(specification), Name(fetchStrategy));

        return await FindAll(specification, fetchStrategy).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IQuerySpecification<TRecord> specification, IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with spec:{Specification}, fetch:{FetchStrategy} and ordering, skip:{Skip}, count:{Count}", Entity, Name(specification), Name(fetchStrategy), skip, count);

        return await FindAll(specification, fetchStrategy).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IFetchStrategy<TRecord> fetchStrategy, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with fetch:{FetchStrategy}", Entity, Name(fetchStrategy));

        return await FindAll(fetchStrategy).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with fetch:{FetchStrategy} and ordering", Entity, Name(fetchStrategy));

        return await FindAll(fetchStrategy).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord>.FindAll(IFetchStrategy<TRecord> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Find all {Entity} with fetch:{FetchStrategy} and ordering, skip:{Skip}, count:{Count}", Entity, Name(fetchStrategy), skip, count);

        return await FindAll(fetchStrategy).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<bool> IReadRepository<TRecord>.Exists(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Exists {Entity}", Entity);

        return await Table.AnyAsync(cancellationToken);
    }

    async Task<bool> IReadRepository<TRecord>.Exists(IQuerySpecification<TRecord> specification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Exists {Entity} with spec: {Specification}", Entity, Name(specification));

        return await Table.FilterBy(specification).AnyAsync(cancellationToken);
    }

    private static string Name(object? strategy) => strategy?.GetType().Name ?? "null";
}
