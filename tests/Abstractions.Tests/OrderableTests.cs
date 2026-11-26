using TeamSL.Data.Abstractions.Ordering;

namespace TeamSL.Data.Abstractions.Tests;

public class OrderableTests
{
    // Deliberately unsorted. Names and ages repeat so secondary sorts are visible.
    private static readonly User[] Users =
    [
        new() { Id = 1, Name = "b", Age = 2 },
        new() { Id = 2, Name = "a", Age = 1 },
        new() { Id = 3, Name = "b", Age = 1 },
        new() { Id = 4, Name = "a", Age = 2 },
    ];

    private static string Sorted(Action<Orderable<User>> order)
        => string.Join(",", Users.AsQueryable().OrderBy(order));

    [Fact]
    public void Asc_sorts_ascending() => Assert.Equal("a1,a2,b2,b1", Sorted(o => o.Asc(u => u.Name)));

    [Fact]
    public void Desc_sorts_descending() => Assert.Equal("b2,a2,a1,b1", Sorted(o => o.Desc(u => u.Age)));

    [Fact]
    public void Second_call_adds_a_tie_breaker_instead_of_replacing_the_first_sort()
        => Assert.Equal("a2,a1,b2,b1", Sorted(o => o.Asc(u => u.Name).Desc(u => u.Age)));

    [Fact]
    public void Directions_can_be_mixed_either_way()
        => Assert.Equal("b1,b2,a1,a2", Sorted(o => o.Desc(u => u.Name).Asc(u => u.Age)));

    [Fact]
    public void Two_key_Asc_equals_chained_Asc()
        => Assert.Equal(Sorted(o => o.Asc(u => u.Name).Asc(u => u.Age)), Sorted(o => o.Asc(u => u.Name, u => u.Age)));

    [Fact]
    public void Two_key_Desc_equals_chained_Desc()
        => Assert.Equal("b2,b1,a2,a1", Sorted(o => o.Desc(u => u.Name, u => u.Age)));

    [Fact]
    public void Three_key_overloads_chain_all_keys()
    {
        Assert.Equal("a1,a2,b1,b2", Sorted(o => o.Asc(u => u.Name, u => u.Age, u => u.Id)));
        Assert.Equal("b2,b1,a2,a1", Sorted(o => o.Desc(u => u.Name, u => u.Age, u => u.Id)));
    }

    [Fact]
    public void No_ordering_keeps_the_source_order() => Assert.Equal("b2,a1,b1,a2", Sorted(_ => { }));

    [Fact]
    public void Null_key_selector_throws()
    {
        Assert.Throws<ArgumentNullException>(() => Sorted(o => o.Asc<int>(null!)));
        Assert.Throws<ArgumentNullException>(() => Sorted(o => o.Desc<int>(null!)));
    }

    [Fact]
    public void Null_source_throws() => Assert.Throws<ArgumentNullException>(() => new Orderable<User>(null!));
}
