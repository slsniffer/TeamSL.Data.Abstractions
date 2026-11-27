using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;
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
