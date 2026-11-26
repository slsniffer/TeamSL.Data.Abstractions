using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Repository;

using var connection = new SqliteConnection("DataSource=:memory:");
connection.Open();

// Registration: the DbContext, then the repositories and unit of work in one call.
await using var services = new ServiceCollection()
    .AddLogging(b => b.AddSimpleConsole().SetMinimumLevel(LogLevel.Debug).AddFilter("Microsoft", LogLevel.Warning))
    .AddDbContext<AppDbContext>(o => o.UseSqlite(connection))
    .AddEfDataRepositories<AppDbContext>()
    .BuildServiceProvider();

// One scope = one request: the repositories and the unit of work share the scope's DbContext.
await using var scope = services.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
context.Database.EnsureCreated();

var us = scope.ServiceProvider.GetRequiredService<IReadRepository<ApplicationUser>>();
var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

// Stage new records on the unit of work, then commit them in one transaction.
unitOfWork.Add(new ApplicationUser { PublicId = Guid.NewGuid(), Email = "bob@example.com", Transactions = { new Transaction { Amount = 10 } } });
unitOfWork.Add(new ApplicationUser { PublicId = Guid.NewGuid(), Email = "alice@example.com" });
unitOfWork.Add(new ApplicationUser { PublicId = Guid.NewGuid(), Email = "carol@example.com" });
await unitOfWork.SaveChanges();
context.ChangeTracker.Clear(); // so the reads below go to the database

var bob = await us.Find(new UserByEmailSpecification("bob@example.com"), new UserWithTransactionsFetchStrategy());
Console.WriteLine($"Found {bob?.Email} with {bob?.Transactions.Count} transaction(s)");

var byId = await us.Find(new ById<ApplicationUser>(bob!.Id), new UserWithTransactionsFetchStrategy());
Console.WriteLine($"ById({bob.Id}): {byId?.Email}");

var byPublicId = await us.Find(new ByPublicId<ApplicationUser>(bob.PublicId));
Console.WriteLine($"ByPublicId({bob.PublicId}): {byPublicId?.Email}");

var missing = await us.Find(new ById<ApplicationUser>(999));
Console.WriteLine($"ById(999) is null: {missing is null}");

var sorted = await us.FindAll(o => o.Asc(u => u.Email));
Console.WriteLine("Sorted: " + string.Join(", ", sorted.Select(u => u.Email)));

// Load from the database, change a property, save: the entity is tracked, so no Update call is needed.
var toEdit = await us.Find(new ById<ApplicationUser>(bob.Id));
toEdit!.Email = "robert@example.com";
var written = await unitOfWork.SaveChanges();
Console.WriteLine($"Saved {written} change(s)");

// Verify against the database, not the tracked instance.
context.ChangeTracker.Clear();
var reloaded = await us.Find(new ById<ApplicationUser>(bob.Id));
Console.WriteLine($"Reloaded email: {reloaded?.Email}");
if (reloaded?.Email != "robert@example.com")
    throw new InvalidOperationException("Change was not persisted");

// Remove: stage on the unit of work, then save.
var carol = await us.Find(new UserByEmailSpecification("carol@example.com"));
unitOfWork.Remove(carol!);
await unitOfWork.SaveChanges();
Console.WriteLine($"After remove: {await us.Count()} user(s), carol exists: {await us.Exists(new UserByEmailSpecification("carol@example.com"))}");

public class ApplicationUser : IPublicRecord
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public string Email { get; set; } = "";
    public List<Transaction> Transactions { get; set; } = new();
}

public class Transaction
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
}

public class UserByEmailSpecification(string email)
    : ExpressionQuerySpecification<ApplicationUser>(u => u.Email == email);

public class UserWithTransactionsFetchStrategy : IFetchStrategy<ApplicationUser>
{
    public IQueryable<ApplicationUser> Apply(IQueryable<ApplicationUser> queryable) => queryable.Include(u => u.Transactions);
}
