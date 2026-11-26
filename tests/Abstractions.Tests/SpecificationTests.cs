using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Tests;

public class SpecificationTests
{
    private static readonly IQueryable<User> Users = new[]
    {
        new User { Name = "young", Age = 10 },
        new User { Name = "old", Age = 70 },
    }.AsQueryable();

    [Fact]
    public void Expression_specification_filters() => Assert.Equal("old", new OlderThan(30).Satisfy(Users).Single().Name);

    [Fact]
    public void Null_criteria_matches_everything() => Assert.Equal(2, new MatchAll().Satisfy(Users).Count());

    [Fact]
    public void Expression_specification_rejects_a_null_expression()
        => Assert.Throws<ArgumentNullException>(() => new NullExpression());

    private sealed class NullExpression() : ExpressionQuerySpecification<User>(null!);
}

public class ExtensionGuardTests
{
    private static readonly IQueryable<User> Users = new[] { new User { Age = 1 }, new User { Age = 2 }, new User { Age = 3 } }.AsQueryable();

    [Fact]
    public void FilterBy_applies_the_specification() => Assert.Equal(2, Users.FilterBy(new OlderThan(1)).Count());

    [Fact]
    public void FilterBy_rejects_nulls()
    {
        Assert.Throws<ArgumentNullException>(() => ((IQueryable<User>)null!).FilterBy(new OlderThan(1)));
        Assert.Throws<ArgumentNullException>(() => Users.FilterBy(null!));
    }

    [Fact]
    public void PageBy_skips_and_takes() => Assert.Equal([2], Users.PageBy(1, 1).Select(u => u.Age));

    [Fact]
    public void PageBy_rejects_null() => Assert.Throws<ArgumentNullException>(() => ((IQueryable<User>)null!).PageBy(0, 1));

    [Fact]
    public void Fetch_applies_the_strategy()
    {
        var fetch = new RecordingFetch();
        Users.Fetch(fetch);
        Assert.Same(Users, fetch.Received);
    }

    [Fact]
    public void Fetch_rejects_nulls()
    {
        Assert.Throws<ArgumentNullException>(() => ((IQueryable<User>)null!).Fetch(new RecordingFetch()));
        Assert.Throws<ArgumentNullException>(() => Users.Fetch(null!));
    }
}
