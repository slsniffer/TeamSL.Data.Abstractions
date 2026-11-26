using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

/// <summary>
/// Stages changes on, and saves, the DbContext returned by <see cref="IContextProvider"/>.
/// Register it with the same lifetime (usually scoped) as the repositories so they share one context.
/// </summary>
public sealed class EntityFrameworkUnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly ILogger<EntityFrameworkUnitOfWork> _logger;

    public EntityFrameworkUnitOfWork(IContextProvider contextProvider, ILogger<EntityFrameworkUnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(contextProvider);
        ArgumentNullException.ThrowIfNull(logger);
        _context = contextProvider.Context();
        _logger = logger;
    }

    public void Add<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Add new {Entity}", typeof(TRecord).Name);

        _context.Add(record);
    }

    public void Update<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Update {Entity}", typeof(TRecord).Name);

        // Tracked records are left alone: EF detects their changes itself, and Update would mark
        // every property and the whole graph as modified.
        if (_context.Entry(record).State == EntityState.Detached)
            _context.Update(record);
    }

    public void Remove<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Remove {Entity}", typeof(TRecord).Name);

        _context.Remove(record);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes");

        return _context.SaveChangesAsync(cancellationToken);
    }
}
