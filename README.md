# TeamSL.Data.Abstractions
Simple and light-weight library based on Repository and Specification patterns to implement basic CRUD operations and make you repositories more generic.

Main features:
- Load a single entity by Id (`ById` specification, any key type)
- Find all entities
- Count entities
- Find entities using specification pattern
- Fetching strategies to include related entities
- Ordering (chainable: `x.Asc(a).Desc(b)`)
- Existence checks
- Unit of work: add / update / remove, saved together in one transaction

## Entity Framework
Implementation Repository pattern for Entity Framework.

```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddEfDataRepositories<AppDbContext>();
```

`AddEfDataRepositories<TContext>()` registers `IReadRepository<>`, `IUnitOfWork` and an `IContextProvider` for `TContext`, all scoped, so everything in a request shares one DbContext.
Services you registered yourself beforehand (e.g. a custom `IContextProvider`) are kept.

- `IReadRepository<T>` queries. It always tracks the records it returns, so loading a record, changing it and calling `IUnitOfWork.SaveChanges` persists the change.
- `IUnitOfWork` writes. `Add` / `Update` / `Remove` only stage changes; `SaveChanges` commits everything staged in the scope in one transaction.
  `Update` is only needed for detached records (e.g. built from a DTO).

## Examples

#### Load entity by Id
`var user = await _userRepository.Find(new ById<User>(userId), cancellationToken);`  
`var order = await _orderRepository.Find(new ById<Order, Guid>(orderId), cancellationToken);` // non-long key

#### Load entity by PublicId
For entities implementing `IPublicRecord` / `IPublicRecord<TKey>` (an external Guid next to the internal Id):  
`var user = await _userRepository.Find(new ByPublicId<User>(publicId), cancellationToken);`  
`var order = await _orderRepository.Find(new ByPublicId<Order, Guid>(publicId), cancellationToken);` // non-long Id

#### Find entity
`var user = await _userRepository.Find(new GetUserByEmailSpecification(userEmail), cancellationToken);`

#### Load entity with related entity
`var user = await _userRepository.Find(new ById<User>(userId), new UserWithTransactionsFetchStrategy(), cancellationToken);`

#### Find entities and sort
`var users = await _userRepository.FindAll(x => x.Asc(u => u.LastName).Desc(u => u.CreatedDate), cancellationToken);`

#### Check existence
`var taken = await _userRepository.Exists(new GetUserByEmailSpecification(userEmail), cancellationToken);`

#### Save changes
```csharp
_unitOfWork.Add(user);
_unitOfWork.Add(order);
await _unitOfWork.SaveChanges(ct);      // one transaction

var existing = await _users.Find(new ById<User>(id), ct);
existing!.Email = newEmail;             // tracked: no Update call needed
await _unitOfWork.SaveChanges(ct);

_unitOfWork.Remove(existing);
await _unitOfWork.SaveChanges(ct);
```
