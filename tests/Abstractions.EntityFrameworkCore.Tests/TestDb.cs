using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class User : IPublicRecord
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<Post> Posts { get; set; } = new();
}

public class Post : IRecord
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public long UserId { get; set; }
}

public class Account : IPublicRecord<Guid>
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}

public class Tag : IRecord<string>
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
}

/// <summary>A non-Guid public identifier: a URL slug.</summary>
public class Article : IPublicRecord<string>
{
    public long Id { get; set; }
    public string PublicId { get; set; } = "";
    public string Title { get; set; } = "";
}

/// <summary>An entity that implements none of the library's interfaces.</summary>
public class Note
{
    public int NoteId { get; set; }
    public string Text { get; set; } = "";
}

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Note> Notes => Set<Note>();
}

/// <summary>Test double: an <see cref="IContextProvider"/> over one given context, for building repositories without DI.</summary>
public sealed class TestContextProvider(DbContext context) : IContextProvider
{
    public DbContext Context<TRecord>() where TRecord : class => context;

    public IEnumerable<DbContext> Contexts() => [context];
}

public class ByName(string name) : ExpressionQuerySpecification<User>(u => u.Name == name);

public class NameStartsWith(string prefix) : ExpressionQuerySpecification<User>(u => u.Name.StartsWith(prefix));

public class WithPosts : IFetchStrategy<User>
{
    public IQueryable<User> Apply(IQueryable<User> queryable) => queryable.Include(u => u.Posts);
}

/// <summary>
/// A fresh SQLite in-memory database per instance. xUnit creates a new test class instance per test,
/// so every test that owns a TestDb starts from an empty schema.
/// </summary>
public sealed class TestDb : IDisposable
{
    public TestDb()
    {
        Connection.Open();
        using var context = NewContext();
        context.Database.EnsureCreated();
    }

    public SqliteConnection Connection { get; } = new("DataSource=:memory:");

    public TestDbContext NewContext(QueryTrackingBehavior tracking = QueryTrackingBehavior.TrackAll)
        => new(new DbContextOptionsBuilder<TestDbContext>().UseSqlite(Connection).UseQueryTrackingBehavior(tracking).Options);

    public static IReadRepository<T> Repository<T>(DbContext context) where T : class
        => new EntityFrameworkRepository<T>(new TestContextProvider(context), NullLogger<EntityFrameworkRepository<T>>.Instance);

    public static IUnitOfWork UnitOfWork(DbContext context)
        => new EntityFrameworkUnitOfWork(new TestContextProvider(context), NullLogger<EntityFrameworkUnitOfWork>.Instance);

    public void Seed(params object[] entities)
    {
        using var context = NewContext();
        context.AddRange(entities);
        context.SaveChanges();
    }

    /// <summary>Reads through a separate context, so results come from the database, not a change tracker.</summary>
    public T Read<T>(Func<TestDbContext, T> query)
    {
        using var context = NewContext();
        return query(context);
    }

    public void Dispose() => Connection.Dispose();
}
