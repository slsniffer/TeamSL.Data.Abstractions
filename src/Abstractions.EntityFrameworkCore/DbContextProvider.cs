using Microsoft.EntityFrameworkCore;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

/// <summary>One DbContext registered with <c>AddEfDataRepositories&lt;TContext&gt;()</c>; DI holds one per call.</summary>
internal interface IRegisteredContext
{
    DbContext Context { get; }
}

internal sealed class RegisteredContext<TContext>(TContext context) : IRegisteredContext
    where TContext : DbContext
{
    public DbContext Context => context;
}

/// <summary>Picks, for each entity type, the registered DbContext whose model maps it.</summary>
internal sealed class DbContextProvider(IEnumerable<IRegisteredContext> registered) : IContextProvider
{
    private readonly DbContext[] _contexts = registered.Select(r => r.Context).ToArray();

    public DbContext Context<TRecord>() where TRecord : class
    {
        if (_contexts.Length == 1)
            return _contexts[0];

        var matches = _contexts.Where(c => c.Model.FindEntityType(typeof(TRecord)) != null).ToArray();
        return matches.Length switch
        {
            1 => matches[0],
            0 => throw new InvalidOperationException(
                $"Entity type '{typeof(TRecord).Name}' is not mapped by any registered DbContext ({Names(_contexts)})."),
            _ => throw new InvalidOperationException(
                $"Entity type '{typeof(TRecord).Name}' is mapped by more than one registered DbContext ({Names(matches)}). Map it in only one of them."),
        };
    }

    public IEnumerable<DbContext> Contexts() => _contexts;

    private static string Names(IEnumerable<DbContext> contexts) => string.Join(", ", contexts.Select(c => c.GetType().Name));
}
