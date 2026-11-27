using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class ServiceCollectionTests : IDisposable
{
    private readonly TestDb _db = new();

    public void Dispose() => _db.Dispose();

    private ServiceProvider BuildProvider(Action<IServiceCollection>? before = null)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddDbContext<TestDbContext>(o => o.UseSqlite(_db.Connection));
        before?.Invoke(services);
        services.AddEfDataRepositories<TestDbContext>();
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }

    [Fact]
    public void Registers_read_repository_unit_of_work_and_context_provider()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        Assert.IsType<EntityFrameworkRepository<User>>(scope.ServiceProvider.GetRequiredService<IReadRepository<User>>());
        Assert.IsType<EntityFrameworkRepository<Account>>(scope.ServiceProvider.GetRequiredService<IReadRepository<Account>>());
        Assert.IsType<EntityFrameworkUnitOfWork>(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        Assert.Same(scope.ServiceProvider.GetRequiredService<TestDbContext>(), scope.ServiceProvider.GetRequiredService<IContextProvider>().Context<User>());
    }

    [Fact]
    public void Each_scope_gets_its_own_context()
    {
        using var provider = BuildProvider();
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();

        Assert.NotSame(
            first.ServiceProvider.GetRequiredService<IContextProvider>().Context<User>(),
            second.ServiceProvider.GetRequiredService<IContextProvider>().Context<User>());
    }

    [Fact]
    public async Task Repository_and_unit_of_work_in_a_scope_share_the_context()
    {
        _db.Seed(new User { Name = "a" });
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IReadRepository<User>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = await users.Find(new ByName("a"));
        user!.Email = "via-di";
        await unitOfWork.SaveChanges();

        Assert.Equal("via-di", _db.Read(c => c.Users.Single().Email));
    }

    [Fact]
    public void Keeps_a_context_provider_registered_beforehand()
    {
        using var provider = BuildProvider(s => s.AddScoped<IContextProvider>(p => new TestContextProvider(p.GetRequiredService<TestDbContext>())));
        using var scope = provider.CreateScope();

        Assert.IsType<TestContextProvider>(scope.ServiceProvider.GetRequiredService<IContextProvider>());
    }

    [Fact]
    public void Calling_it_twice_does_not_duplicate_registrations()
    {
        var services = new ServiceCollection().AddEfDataRepositories<TestDbContext>().AddEfDataRepositories<TestDbContext>();

        Assert.Equal(4, services.Count); // registered context, context provider, repository, unit of work
    }

    [Fact]
    public void Missing_DbContext_registration_fails_with_a_clear_error()
    {
        using var provider = new ServiceCollection().AddLogging().AddEfDataRepositories<TestDbContext>().BuildServiceProvider();
        using var scope = provider.CreateScope();

        var error = Assert.Throws<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        Assert.Contains(nameof(TestDbContext), error.Message);
    }
}
