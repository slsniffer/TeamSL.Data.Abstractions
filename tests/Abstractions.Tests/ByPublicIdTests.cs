using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Tests;

public class ByPublicIdTests
{
    private static readonly Guid Known = Guid.NewGuid();

    [Fact]
    public void Matches_the_record_with_the_public_id()
    {
        var users = new[] { new User { PublicId = Guid.NewGuid(), Name = "x" }, new User { PublicId = Known, Name = "y" } }.AsQueryable();

        Assert.Equal("y", new ByPublicId<User>(Known).Satisfy(users).Single().Name);
    }

    [Fact]
    public void Matches_nothing_for_an_unknown_public_id()
    {
        var users = new[] { new User { PublicId = Known } }.AsQueryable();

        Assert.Empty(new ByPublicId<User>(Guid.NewGuid()).Satisfy(users));
    }

    [Fact]
    public void Is_independent_of_the_internal_key_type()
    {
        var orders = new[] { new Order { Id = Guid.NewGuid(), PublicId = Known, Code = "z" } }.AsQueryable();

        Assert.Equal("z", new ByPublicId<Order, Guid, Guid>(Known).Satisfy(orders).Single().Code);
    }

    [Fact]
    public void Guid_shortcut_behaves_like_the_generic_version()
    {
        var users = new[] { new User { PublicId = Known, Name = "y" } }.AsQueryable();

        Assert.Same(new ByPublicId<User, Guid>(Known).Satisfy(users).Single(), new ByPublicId<User>(Known).Satisfy(users).Single());
    }

    [Fact]
    public void Works_with_non_guid_public_ids()
    {
        var articles = new[] { new Article { PublicId = "hello-world", Title = "Hello" }, new Article { PublicId = "other" } }.AsQueryable();

        Assert.Equal("Hello", new ByPublicId<Article, string>("hello-world").Satisfy(articles).Single().Title);
        Assert.Empty(new ByPublicId<Article, string>("missing").Satisfy(articles));
    }

    [Fact]
    public void Shortcuts_behave_like_the_full_three_parameter_version()
    {
        var users = new[] { new User { PublicId = Known, Name = "u" } }.AsQueryable();
        var articles = new[] { new Article { PublicId = "slug" } }.AsQueryable();

        Assert.Same(new ByPublicId<User, Guid, long>(Known).Satisfy(users).Single(), new ByPublicId<User>(Known).Satisfy(users).Single());
        Assert.Same(new ByPublicId<Article, string, long>("slug").Satisfy(articles).Single(), new ByPublicId<Article, string>("slug").Satisfy(articles).Single());
    }

    [Fact]
    public void Records_with_a_public_id_still_work_with_ById()
    {
        var orderId = Guid.NewGuid();
        var orders = new[] { new Order { Id = orderId, PublicId = Known, Code = "o" } }.AsQueryable();
        var users = new[] { new User { Id = 7, PublicId = Known, Name = "u" } }.AsQueryable();

        Assert.Equal("o", new ById<Order, Guid>(orderId).Satisfy(orders).Single().Code);   // IPublicRecord<Guid, Guid> is an IRecord<Guid>
        Assert.Equal("u", new ById<User>(7).Satisfy(users).Single().Name);                  // IPublicRecord is an IRecord
    }
}
