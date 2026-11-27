using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

/// <summary>
/// Stages changes on, and saves, the DbContext(s) returned by <see cref="IContextProvider"/>.
/// Register it with the same lifetime (usually scoped) as the repositories so they share the contexts.
/// </summary>
public sealed class EntityFrameworkUnitOfWork : IUnitOfWork
{
    private readonly IContextProvider _contextProvider;
    private readonly IReadOnlyList<DbContext> _contexts;
    private readonly ILogger<EntityFrameworkUnitOfWork> _logger;

    public EntityFrameworkUnitOfWork(IContextProvider contextProvider, ILogger<EntityFrameworkUnitOfWork> logger)
    {
        ArgumentNullException.ThrowIfNull(contextProvider);
        ArgumentNullException.ThrowIfNull(logger);
        _contextProvider = contextProvider;
        _contexts = contextProvider.Contexts().ToList();
        _logger = logger;
    }

    public void Add<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Add new {Entity}", typeof(TRecord).Name);

        _contextProvider.Context<TRecord>().Add(record);
    }

    public void Update<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Update {Entity}", typeof(TRecord).Name);

        // Tracked records are left alone: EF detects their changes itself, and Update would mark
        // every property and the whole graph as modified.
        var context = _contextProvider.Context<TRecord>();
        if (context.Entry(record).State == EntityState.Detached)
            context.Update(record);
    }

    public void Remove<TRecord>(TRecord record) where TRecord : class
    {
        ArgumentNullException.ThrowIfNull(record);
        _logger.LogDebug("Remove {Entity}", typeof(TRecord).Name);

        _contextProvider.Context<TRecord>().Remove(record);
    }

    /// <summary>Saves every context of the scope in turn; each one commits in its own transaction.</summary>
    public async Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Saving changes");

        var saved = 0;
        foreach (var context in _contexts)
            saved += await context.SaveChangesAsync(cancellationToken);
        return saved;
    }
}
