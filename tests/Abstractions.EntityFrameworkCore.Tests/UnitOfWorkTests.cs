using Microsoft.EntityFrameworkCore;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore.Tests;

public class UnitOfWorkTests : IDisposable
{
    private readonly TestDb _db = new();

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Add_only_stages_until_SaveChanges()
    {
        var unitOfWork = TestDb.UnitOfWork(_db.NewContext());

        unitOfWork.Add(new User { Name = "a" });
        Assert.Equal(0, _db.Read(c => c.Users.Count()));

        Assert.Equal(1, await unitOfWork.SaveChanges());
        Assert.Equal(1, _db.Read(c => c.Users.Count()));
    }

    [Fact]
    public async Task SaveChanges_commits_different_entity_types_together()
    {
        var unitOfWork = TestDb.UnitOfWork(_db.NewContext());

        unitOfWork.Add(new User { Name = "a" });
        unitOfWork.Add(new Account { Name = "b" });
        unitOfWork.Add(new Tag { Id = "c" });

        Assert.Equal(3, await unitOfWork.SaveChanges());
    }

    [Fact]
    public async Task Remove_only_stages_until_SaveChanges()
    {
        _db.Seed(new User { Name = "gone" });
        using var context = _db.NewContext();
        var unitOfWork = TestDb.UnitOfWork(context);
        var user = await TestDb.Repository<User>(context).Find(new ByName("gone"));

        unitOfWork.Remove(user!);
        Assert.Equal(1, _db.Read(c => c.Users.Count()));

        await unitOfWork.SaveChanges();
        Assert.Equal(0, _db.Read(c => c.Users.Count()));
    }

    [Fact]
    public async Task Update_of_a_detached_record_persists_it()
    {
        var seeded = new User { Name = "before", Email = "e" };
        _db.Seed(seeded);
        var unitOfWork = TestDb.UnitOfWork(_db.NewContext());

        unitOfWork.Update(new User { Id = seeded.Id, PublicId = seeded.PublicId, Name = "after", Email = "e" });
        await unitOfWork.SaveChanges();

        Assert.Equal("after", _db.Read(c => c.Users.Single().Name));
    }

    [Fact]
    public async Task Update_of_a_tracked_record_keeps_only_the_changed_properties_modified()
    {
        _db.Seed(new User { Name = "before", Email = "e" });
        using var context = _db.NewContext();
        var user = await TestDb.Repository<User>(context).Find(new ByName("before"));

        user!.Name = "after";
        TestDb.UnitOfWork(context).Update(user);

        var entry = context.Entry(user);
        Assert.Equal(EntityState.Modified, entry.State);
        Assert.True(entry.Property(u => u.Name).IsModified);
        Assert.False(entry.Property(u => u.Email).IsModified);
    }

    [Fact]
    public void Null_records_are_rejected()
    {
        var unitOfWork = TestDb.UnitOfWork(_db.NewContext());

        Assert.Throws<ArgumentNullException>(() => unitOfWork.Add<User>(null!));
        Assert.Throws<ArgumentNullException>(() => unitOfWork.Update<User>(null!));
        Assert.Throws<ArgumentNullException>(() => unitOfWork.Remove<User>(null!));
    }
}

public class TrackingTests : IDisposable
{
    private readonly TestDb _db = new();

    public TrackingTests() => _db.Seed(new User { Name = "a", Email = "old" }, new User { Name = "b" });

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Repository_queries_track_their_results()
    {
        using var context = _db.NewContext();

        await TestDb.Repository<User>(context).FindAll();

        Assert.Equal(2, context.ChangeTracker.Entries<User>().Count());
    }

    [Fact]
    public async Task Load_change_save_persists_without_calling_Update()
    {
        using var context = _db.NewContext();
        var user = await TestDb.Repository<User>(context).Find(new ByName("a"));

        user!.Email = "new";
        var written = await TestDb.UnitOfWork(context).SaveChanges();

        Assert.Equal(1, written);
        Assert.Equal("new", _db.Read(c => c.Users.Single(u => u.Name == "a").Email));
    }

    [Fact]
    public async Task Repository_tracks_even_when_the_context_is_configured_as_no_tracking()
    {
        using var context = _db.NewContext(QueryTrackingBehavior.NoTracking);
        var user = await TestDb.Repository<User>(context).Find(new ByName("a"));

        user!.Email = "forced";
        await TestDb.UnitOfWork(context).SaveChanges();

        Assert.Equal("forced", _db.Read(c => c.Users.Single(u => u.Name == "a").Email));
    }
}
