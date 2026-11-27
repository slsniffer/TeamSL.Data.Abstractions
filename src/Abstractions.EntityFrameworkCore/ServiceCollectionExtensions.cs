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
    /// Call it once per DbContext to use several (e.g. one per schema): each repository uses the context that maps
    /// its entity type, and <see cref="IUnitOfWork.SaveChanges"/> saves each of them.
    /// Existing registrations of these services (e.g. a custom <see cref="IContextProvider"/>) are kept.
    /// </summary>
    public static IServiceCollection AddEfDataRepositories<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IRegisteredContext, RegisteredContext<TContext>>());
        services.TryAddScoped<IContextProvider, DbContextProvider>();
        services.TryAdd(ServiceDescriptor.Scoped(typeof(IReadRepository<>), typeof(EntityFrameworkRepository<>)));
        services.TryAddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();

        return services;
    }
}
