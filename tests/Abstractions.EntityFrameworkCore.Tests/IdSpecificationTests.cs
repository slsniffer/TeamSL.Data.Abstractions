using Microsoft.EntityFrameworkCore;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class IdSpecificationTests : IDisposable
{
    private readonly TestDb _db = new();
    private readonly User _user = new() { Name = "u" };
    private readonly Account _account = new() { Name = "acc" };

    public IdSpecificationTests() => _db.Seed(
        _user, new User { Name = "other" }, _account,
        new Tag { Id = "t-1", Label = "one" },
        new Article { PublicId = "hello-world", Title = "Hello" });

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task ById_with_long_key()
    {
        var users = TestDb.Repository<User>(_db.NewContext());

        Assert.Equal("u", (await users.Find(new ById<User>(_user.Id)))?.Name);
        Assert.Null(await users.Find(new ById<User>(9999)));
    }

    [Fact]
    public async Task ById_with_guid_key()
    {
        var accounts = TestDb.Repository<Account>(_db.NewContext());

        Assert.Equal("acc", (await accounts.Find(new ById<Account>(_account.Id)))?.Name);
    }

    [Fact]
    public async Task ById_with_string_key()
    {
        var tags = TestDb.Repository<Tag>(_db.NewContext());

        Assert.Equal("one", (await tags.Find(new ById<Tag, string>("t-1")))?.Label);
        Assert.Null(await tags.Find(new ById<Tag, string>("missing")));
    }

    [Fact]
    public async Task ById_combines_with_a_fetch_strategy()
    {
        _db.Seed(new Post { Title = "p", UserId = _user.Id });
        var users = TestDb.Repository<User>(_db.NewContext());

        var user = await users.Find(new ById<User>(_user.Id), new WithPosts());

        Assert.Single(user!.Posts);
    }

    [Fact]
    public async Task ByPublicId_with_guid_public_id_regardless_of_internal_key_type()
    {
        using var context = _db.NewContext();

        Assert.Equal("u", (await TestDb.Repository<User>(context).Find(new ByPublicId<User>(_user.PublicId)))?.Name);          // long Id
        Assert.Equal("acc", (await TestDb.Repository<Account>(context).Find(new ByPublicId<Account, Guid>(_account.PublicId)))?.Name); // IPublicRecord<Guid>
        Assert.Null(await TestDb.Repository<User>(context).Find(new ByPublicId<User>(Guid.NewGuid())));
    }

    [Fact]
    public async Task ByPublicId_with_string_public_id()
    {
        var articles = TestDb.Repository<Article>(_db.NewContext());

        Assert.Equal("Hello", (await articles.Find(new ByPublicId<Article, string>("hello-world")))?.Title);
        Assert.Null(await articles.Find(new ByPublicId<Article, string>("missing")));
    }

    [Fact]
    public void ByPublicId_with_string_public_id_is_sent_as_a_query_parameter()
    {
        using var context = _db.NewContext();

        var sql = new ByPublicId<Article, string>("hello-world").Satisfy(context.Articles).ToQueryString();

        Assert.Contains("\"a\".\"PublicId\" = @", sql);
    }

    [Fact]
    public void ById_is_sent_as_a_query_parameter()
    {
        using var context = _db.NewContext();

        var sql = new ById<User>(_user.Id).Satisfy(context.Users).ToQueryString();

        Assert.Contains("\"u\".\"Id\" = @", sql);
    }

    [Fact]
    public void ByPublicId_is_sent_as_a_query_parameter()
    {
        using var context = _db.NewContext();

        var sql = new ByPublicId<User>(_user.PublicId).Satisfy(context.Users).ToQueryString();

        Assert.Contains("\"u\".\"PublicId\" = @", sql);
    }
}
