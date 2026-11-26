using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Tests;

public class ByIdTests
{
    private static readonly IQueryable<User> Users = new[]
    {
        new User { Id = 1, Name = "one" },
        new User { Id = 2, Name = "two" },
        new User { Id = 3, Name = "three" },
    }.AsQueryable();

    [Fact]
    public void Matches_the_record_with_the_id() => Assert.Equal("two", new ById<User>(2).Satisfy(Users).Single().Name);

    [Fact]
    public void Matches_nothing_for_an_unknown_id() => Assert.Empty(new ById<User>(99).Satisfy(Users));

    [Fact]
    public void Long_shortcut_behaves_like_the_generic_version()
        => Assert.Equal(new ById<User, long>(3).Satisfy(Users).Single(), new ById<User>(3).Satisfy(Users).Single());

    [Fact]
    public void Works_with_guid_keys()
    {
        var id = Guid.NewGuid();
        var orders = new[] { new Order { Id = Guid.NewGuid(), Code = "x" }, new Order { Id = id, Code = "y" } }.AsQueryable();

        Assert.Equal("y", new ById<Order, Guid>(id).Satisfy(orders).Single().Code);
    }

    [Fact]
    public void Works_with_string_keys()
    {
        var tags = new[] { new Tag { Id = "a" }, new Tag { Id = "b" } }.AsQueryable();

        Assert.Equal("b", new ById<Tag, string>("b").Satisfy(tags).Single().Id);
        Assert.Empty(new ById<Tag, string>("c").Satisfy(tags));
    }
}
