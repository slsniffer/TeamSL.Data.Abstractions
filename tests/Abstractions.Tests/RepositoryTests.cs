using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
using TeamSL.Data.Abstractions.Ordering;
using TeamSL.Data.Abstractions.Repository;

namespace TeamSL.Data.Abstractions.Tests;

public class RepositoryImplTests
{
    private sealed class ListRepository(params User[] users) : RepositoryImpl<User>
    {
        protected override IQueryable<User> Table { get; } = users.AsQueryable();

        public IQueryable<User> FilterThenFetch(IQuerySpecification<User> spec, IFetchStrategy<User> fetch) => FindAll(spec, fetch);
        public IQueryable<User> FindOne(IQuerySpecification<User> spec, IFetchStrategy<User> fetch) => Find(spec, fetch);
        public IQueryable<User> All() => FindAll();
    }

    private readonly ListRepository _repository = new(new User { Age = 10 }, new User { Age = 50 }, new User { Age = 90 });

    [Fact]
    public void FindAll_filters_before_fetching()
    {
        var fetch = new RecordingFetch();

        var result = _repository.FilterThenFetch(new OlderThan(30), fetch).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(2, fetch.Received!.Count());   // the fetch step only saw the filtered rows
    }

    [Fact]
    public void Find_filters_before_fetching()
    {
        var fetch = new RecordingFetch();

        _repository.FindOne(new OlderThan(80), fetch).ToList();

        Assert.Equal(90, fetch.Received!.Single().Age);
    }

    [Fact]
    public void FindAll_without_arguments_returns_the_table() => Assert.Equal(3, _repository.All().Count());
}

public class ReadRepositoryDefaultTests
{
    /// <summary>A custom implementation that only provides Count: Exists must work through the interface defaults.</summary>
    private sealed class CountOnlyRepository(int count) : IReadRepository<User>
    {
        public IQuerySpecification<User>? CountedWith { get; private set; }

        public Task<int> Count(CancellationToken cancellationToken = default) => Task.FromResult(count);

        public Task<int> Count(IQuerySpecification<User> specification, CancellationToken cancellationToken = default)
        {
            CountedWith = specification;
            return Task.FromResult(count);
        }

        public Task<User?> Find(IQuerySpecification<User> s, CancellationToken c = default) => throw new NotSupportedException();
        public Task<User?> Find(IQuerySpecification<User> s, IFetchStrategy<User> f, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(Action<Orderable<User>> o, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(Action<Orderable<User>> o, int skip, int take, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, Action<Orderable<User>> o, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, Action<Orderable<User>> o, int skip, int take, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, IFetchStrategy<User> f, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, IFetchStrategy<User> f, Action<Orderable<User>> o, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IQuerySpecification<User> s, IFetchStrategy<User> f, Action<Orderable<User>> o, int skip, int take, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IFetchStrategy<User> f, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IFetchStrategy<User> f, Action<Orderable<User>> o, CancellationToken c = default) => throw new NotSupportedException();
        public Task<IList<User>> FindAll(IFetchStrategy<User> f, Action<Orderable<User>> o, int skip, int take, CancellationToken c = default) => throw new NotSupportedException();
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    public async Task Exists_defaults_to_Count_greater_than_zero(int count, bool expected)
    {
        IReadRepository<User> repository = new CountOnlyRepository(count);

        Assert.Equal(expected, await repository.Exists());
    }

    [Fact]
    public async Task Exists_with_a_specification_passes_it_to_Count()
    {
        var implementation = new CountOnlyRepository(1);
        IReadRepository<User> repository = implementation;
        var spec = new OlderThan(1);

        Assert.True(await repository.Exists(spec));
        Assert.Same(spec, implementation.CountedWith);
    }
}
