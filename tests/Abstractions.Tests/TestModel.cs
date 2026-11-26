using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;
using TeamSL.Data.Abstractions.Fetching;
using TeamSL.Data.Abstractions.Filtering;

namespace TeamSL.Data.Abstractions.Tests;

public class User : IPublicRecord
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public override string ToString() => $"{Name}{Age}";
}

public class Order : IPublicRecord<Guid, Guid>
{
    public Guid Id { get; set; }
    public Guid PublicId { get; set; }
    public string Code { get; set; } = "";
}

public class Tag : IRecord<string>
{
    public string Id { get; set; } = "";
}

public class Article : IPublicRecord<string>
{
    public long Id { get; set; }
    public string PublicId { get; set; } = "";
    public string Title { get; set; } = "";
}

public class OlderThan(int age) : ExpressionQuerySpecification<User>(u => u.Age > age);

public class MatchAll : QuerySpecification<User>
{
    protected override Expression<Func<User, bool>>? MatchingCriteria() => null;
}

/// <summary>Records the query it was applied to, so tests can see what reached the fetch step.</summary>
public class RecordingFetch : IFetchStrategy<User>
{
    public IQueryable<User>? Received { get; private set; }

    public IQueryable<User> Apply(IQueryable<User> queryable)
    {
        Received = queryable;
        return queryable;
    }
}
