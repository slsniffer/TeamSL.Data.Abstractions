using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeamSL.Data.Abstractions.EntityFrameworkCore;
using TeamSL.Data.Abstractions.Repository;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class EfDataRepositoriesServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IReadRepository{TRecord}"/> and <see cref="IUnitOfWork"/>
    /// backed by <typeparamref name="TContext"/>, all scoped so they share the request's DbContext.
    /// Register the context itself first, e.g. with <c>AddDbContext&lt;TContext&gt;()</c>.
    /// Existing registrations of these services (e.g. a custom <see cref="IContextProvider"/>) are kept.
    /// </summary>
    public static IServiceCollection AddEfDataRepositories<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IContextProvider, DbContextProvider<TContext>>();
        services.TryAdd(ServiceDescriptor.Scoped(typeof(IReadRepository<>), typeof(EntityFrameworkRepository<>)));
        services.TryAddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();

        return services;
    }

    private sealed class DbContextProvider<TContext>(TContext context) : IContextProvider
        where TContext : DbContext
    {
        public DbContext Context() => context;
    }
}
