using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Ordering;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

public sealed class EntityFrameworkRepository<TRecord>
    : EntityFrameworkRepository<TRecord, long>, IReadRepository<TRecord>, IRepository<TRecord>
    where TRecord : class, IRecord
{
    public EntityFrameworkRepository(IContextProvider contextProvider, ILogger<EntityFrameworkRepository<TRecord, long>> logger)
        : base(contextProvider, logger)
    {
    }
}

public class EntityFrameworkRepository<TRecord, TKey> : RepositoryImpl<TRecord, TKey>, IReadRepository<TRecord, TKey>, IRepository<TRecord, TKey>
    where TRecord : class, IRecord<TKey>
{
    private readonly DbContext _context;
    private readonly ILogger<EntityFrameworkRepository<TRecord, TKey>> _logger;

    public EntityFrameworkRepository(IContextProvider contextProvider, ILogger<EntityFrameworkRepository<TRecord, TKey>> logger)
    {
        _context = contextProvider.Context();
        _logger = logger;
    }

    protected override IQueryable<TRecord> Table => _context.Set<TRecord>();

    async Task<TRecord> IReadRepository<TRecord, TKey>.Load(TKey id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Loading {0}.Id: {1}", typeof(TRecord).Name, id);

        return await Load(id).SingleOrDefaultAsync(cancellationToken);
    }

    async Task<TRecord> IReadRepository<TRecord, TKey>.Load(TKey id, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Loading {0}.Id: {1} with fetching: {2}", typeof(TRecord).Name, id, fetchStrategy.GetType().Name);

        return await Load(id, fetchStrategy).SingleOrDefaultAsync(cancellationToken);
    }

    async Task<TRecord> IReadRepository<TRecord, TKey>.Find(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find {0} by specification: {1}", typeof(TRecord).Name, specification.GetType().Name);

        return await Find(specification).SingleOrDefaultAsync(cancellationToken);
    }

    async Task<TRecord> IReadRepository<TRecord, TKey>.Find(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find {0} by specification: {1} with fetching: {2}", typeof(TRecord).Name, specification.GetType().Name, fetchStrategy.GetType().Name);

        return await Find(specification, fetchStrategy).FirstOrDefaultAsync(cancellationToken);
    }

    async Task<int> IReadRepository<TRecord, TKey>.Count(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Counting {0}", typeof(TRecord).Name);

        return await Table.CountAsync(cancellationToken);
    }

    async Task<int> IReadRepository<TRecord, TKey>.Count(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Counting {0} with spec: {1}", typeof(TRecord).Name, specification.GetType().Name);

        return await Table.FilterBy(specification).CountAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0}", typeof(TRecord).Name);

        return await FindAll().ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with ordering", typeof(TRecord).Name);

        return await FindAll().OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with ordering, skip:{1}, count:{2}", typeof(TRecord).Name, skip, count);

        return await FindAll().OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1}", typeof(TRecord).Name, specification.GetType().Name);

        return await FindAll(specification).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with fetch:{1}", typeof(TRecord).Name, fetchStrategy.GetType().Name);

        return await FindAll(fetchStrategy).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with fetch:{1} and ordering, skip:{2}, count:{3}", typeof(TRecord).Name, fetchStrategy.GetType().Name, skip, count);

        return await FindAll(fetchStrategy).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1}, fetch:{2}", typeof(TRecord).Name, specification.GetType().Name, fetchStrategy.GetType().Name);

        return await FindAll(specification, fetchStrategy).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1} and ordering", typeof(TRecord).Name, specification.GetType().Name);

        return await FindAll(specification).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1} and ordering, skip:{2}, count:{3}", typeof(TRecord).Name, specification.GetType().Name, skip, count);

        return await FindAll(specification).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with fetch:{1} and ordering", typeof(TRecord).Name, fetchStrategy.GetType().Name);

        return await FindAll(fetchStrategy).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1}, fetch:{2} and ordering", typeof(TRecord).Name, specification.GetType().Name, fetchStrategy.GetType().Name);

        return await FindAll(specification, fetchStrategy).OrderBy(order).ToListAsync(cancellationToken);
    }

    async Task<IList<TRecord>> IReadRepository<TRecord, TKey>.FindAll(IQuerySpecification<TRecord, TKey> specification, IFetchStrategy<TRecord, TKey> fetchStrategy, Action<Orderable<TRecord>> order, int skip, int count, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Find all {0} with spec:{1}, fetch:{2} and ordering, skip:{3}, count:{4}", typeof(TRecord).Name, specification.GetType().Name, fetchStrategy.GetType().Name, skip, count);

        return await FindAll(specification, fetchStrategy).OrderBy(order).Skip(skip).Take(count).ToListAsync(cancellationToken);
    }

    async Task IRepository<TRecord, TKey>.Create(TRecord record, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Create new {0}", typeof(TRecord).Name);

        await _context.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    async Task IRepository<TRecord, TKey>.Update(TRecord record, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Update [{0}] record with Id={1}", typeof(TRecord).Name, record.Id);

        _context.Update(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    async Task IRepository<TRecord, TKey>.Delete(TRecord record, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Delete {0}.Id: {1}", typeof(TRecord).Name, record.Id);

        _context.Remove(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TRecord> Load(TKey id)
    {
        return Table.Where(x => x.Id!.Equals(id));
    }
}