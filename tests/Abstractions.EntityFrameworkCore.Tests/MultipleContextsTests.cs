using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class Product : IRecord
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
}

public class Stock : IRecord
{
    public long Id { get; set; }
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
}

/// <summary>The "main" context (default schema).</summary>
public class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

/// <summary>A second context over the same database (on SQL Server it would map to e.g. the "wdb" schema).</summary>
public class WarehouseContext(DbContextOptions<WarehouseContext> options) : DbContext(options)
{
    public DbSet<Stock> Stocks => Set<Stock>();
}

/// <summary>Maps <see cref="Product"/> too, so it clashes with <see cref="ShopContext"/>.</summary>
public class ClashingContext(DbContextOptions<ClashingContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public class MultipleContextsTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public MultipleContextsTests()
    {
        _connection.Open();
        using var shop = Shop();
        shop.Database.EnsureCreated();
        // EnsureCreated skips a database that already has tables, so the second context creates its own.
        using var warehouse = Warehouse();
        warehouse.GetService<IRelationalDatabaseCreator>().CreateTables();
    }

    public void Dispose() => _connection.Dispose();

    private ShopContext Shop() => new(new DbContextOptionsBuilder<ShopContext>().UseSqlite(_connection).Options);

    private WarehouseContext Warehouse() => new(new DbContextOptionsBuilder<WarehouseContext>().UseSqlite(_connection).Options);

    private ServiceProvider BuildProvider(Action<IServiceCollection>? more = null)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddDbContext<ShopContext>(o => o.UseSqlite(_connection))
            .AddDbContext<WarehouseContext>(o => o.UseSqlite(_connection))
            .AddEfDataRepositories<ShopContext>()
            .AddEfDataRepositories<WarehouseContext>();
        more?.Invoke(services);
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }

    private void Seed(params object[] entities)
    {
        using var shop = Shop();
        using var warehouse = Warehouse();
        foreach (var entity in entities)
            (entity is Stock ? (DbContext)warehouse : shop).Add(entity);
        shop.SaveChanges();
        warehouse.SaveChanges();
    }

    [Fact]
    public void Each_repository_uses_the_context_that_maps_its_entity()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var contexts = scope.ServiceProvider.GetRequiredService<IContextProvider>();

        Assert.Same(scope.ServiceProvider.GetRequiredService<ShopContext>(), contexts.Context<Product>());
        Assert.Same(scope.ServiceProvider.GetRequiredService<WarehouseContext>(), contexts.Context<Stock>());
    }

    [Fact]
    public async Task Repositories_read_from_both_contexts()
    {
        Seed(new Product { Name = "chair" }, new Stock { Sku = "CH-1", Quantity = 5 });
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var product = await scope.ServiceProvider.GetRequiredService<IReadRepository<Product>>().Find(new ById<Product>(1));
        var stock = await scope.ServiceProvider.GetRequiredService<IReadRepository<Stock>>().Find(new ById<Stock>(1));

        Assert.Equal("chair", product?.Name);
        Assert.Equal(5, stock?.Quantity);
    }

    [Fact]
    public async Task Unit_of_work_adds_to_the_right_context_and_saves_both()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        unitOfWork.Add(new Product { Name = "table" });
        unitOfWork.Add(new Stock { Sku = "TB-1", Quantity = 2 });
        var saved = await unitOfWork.SaveChanges();

        Assert.Equal(2, saved);
        using var shop = Shop();
        using var warehouse = Warehouse();
        Assert.Equal("table", shop.Products.Single().Name);
        Assert.Equal("TB-1", warehouse.Set<Stock>().Single().Sku);
    }

    [Fact]
    public async Task Changes_to_loaded_records_of_both_contexts_are_saved()
    {
        Seed(new Product { Name = "chair" }, new Stock { Sku = "CH-1", Quantity = 5 });
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var product = await scope.ServiceProvider.GetRequiredService<IReadRepository<Product>>().Find(new ById<Product>(1));
        var stock = await scope.ServiceProvider.GetRequiredService<IReadRepository<Stock>>().Find(new ById<Stock>(1));
        product!.Name = "armchair";
        stock!.Quantity = 4;
        await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().SaveChanges();

        using var shop = Shop();
        using var warehouse = Warehouse();
        Assert.Equal("armchair", shop.Products.Single().Name);
        Assert.Equal(4, warehouse.Set<Stock>().Single().Quantity);
    }

    [Fact]
    public void An_entity_mapped_by_two_contexts_fails_with_a_clear_error()
    {
        using var provider = BuildProvider(more: s => s
            .AddDbContext<ClashingContext>(o => o.UseSqlite(_connection))
            .AddEfDataRepositories<ClashingContext>());
        using var scope = provider.CreateScope();

        var error = Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IReadRepository<Product>>());
        Assert.Contains(nameof(ShopContext), error.Message);
        Assert.Contains(nameof(ClashingContext), error.Message);
    }

    [Fact]
    public void An_unmapped_entity_fails_with_a_clear_error()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var error = Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IReadRepository<User>>());
        Assert.Contains(nameof(User), error.Message);
        Assert.Contains(nameof(WarehouseContext), error.Message);
    }

    [Fact]
    public void Registering_the_same_context_twice_keeps_it_once()
    {
        using var provider = BuildProvider(more: s => s.AddEfDataRepositories<ShopContext>());
        using var scope = provider.CreateScope();

        Assert.Equal(2, scope.ServiceProvider.GetRequiredService<IContextProvider>().Contexts().Count());
    }
}
