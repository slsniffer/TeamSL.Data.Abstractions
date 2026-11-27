using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class ReadRepositoryTests : IDisposable
{
    private readonly TestDb _db = new();

    public ReadRepositoryTests()
    {
        _db.Seed(
            new User { Name = "bob", Posts = { new Post { Title = "p1" }, new Post { Title = "p2" } } },
            new User { Name = "alice" },
            new User { Name = "carol" },
            new User { Name = "bart" });
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Find_returns_the_single_match()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal("alice", (await users.Find(new ByName("alice")))?.Name);
    }

    [Fact]
    public async Task Find_returns_null_when_nothing_matches()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Null(await users.Find(new ByName("nobody")));
    }

    [Fact]
    public async Task Find_throws_when_more_than_one_record_matches()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        await Assert.ThrowsAsync<InvalidOperationException>(() => users.Find(new NameStartsWith("b")));
    }

    [Fact]
    public async Task Find_with_fetch_strategy_loads_related_records()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var bob = await users.Find(new ByName("bob"), new WithPosts());

        Assert.Equal(2, bob!.Posts.Count);
    }

    [Fact]
    public async Task Find_without_fetch_strategy_does_not_load_related_records()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var bob = await users.Find(new ByName("bob"));

        Assert.Empty(bob!.Posts);
    }

    [Fact]
    public async Task FindFirst_returns_the_first_match_in_order()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal("bart", (await users.FindFirst(new NameStartsWith("b"), o => o.Asc(u => u.Name)))?.Name);
        Assert.Equal("bob", (await users.FindFirst(new NameStartsWith("b"), o => o.Desc(u => u.Name)))?.Name);
    }

    [Fact]
    public async Task FindFirst_returns_null_when_nothing_matches()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Null(await users.FindFirst(new ByName("nobody"), o => o.Asc(u => u.Name)));
    }

    [Fact]
    public async Task FindFirst_with_fetch_strategy_loads_related_records()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var bob = await users.FindFirst(new NameStartsWith("b"), new WithPosts(), o => o.Desc(u => u.Name));

        Assert.Equal("bob", bob?.Name);
        Assert.Equal(2, bob!.Posts.Count);
    }

    [Fact]
    public async Task FindAll_returns_everything()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal(4, (await users.FindAll()).Count);
    }

    [Fact]
    public async Task FindAll_with_specification_filters()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal(["bart", "bob"], (await users.FindAll(new NameStartsWith("b"))).Select(u => u.Name).Order());
    }

    [Fact]
    public async Task FindAll_with_ordering_sorts_in_the_database()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var sorted = await users.FindAll(o => o.Desc(u => u.Name));

        Assert.Equal(["carol", "bob", "bart", "alice"], sorted.Select(u => u.Name));
    }

    [Fact]
    public async Task FindAll_with_specification_ordering_and_paging()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var page = await users.FindAll(new NameStartsWith("b"), o => o.Asc(u => u.Name), skip: 1, count: 1);

        Assert.Equal("bob", page.Single().Name);
    }

    [Fact]
    public async Task FindAll_with_fetch_ordering_and_paging()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        var page = await users.FindAll(new WithPosts(), o => o.Desc(u => u.Name), skip: 1, count: 1);

        Assert.Equal("bob", page.Single().Name);
        Assert.Equal(2, page.Single().Posts.Count);
    }

    [Fact]
    public async Task Count_and_Exists()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal(4, await users.Count());
        Assert.Equal(2, await users.Count(new NameStartsWith("b")));
        Assert.True(await users.Exists());
        Assert.True(await users.Exists(new ByName("carol")));
        Assert.False(await users.Exists(new ByName("nobody")));
    }

    [Fact]
    public async Task Works_for_entities_that_implement_none_of_the_library_interfaces()
    {
        _db.Seed(new Note { Text = "plain" });
        var notes = TestDb.Repository<Note>(_db.NewContext());

        Assert.Equal("plain", (await notes.FindAll()).Single().Text);
    }
}
