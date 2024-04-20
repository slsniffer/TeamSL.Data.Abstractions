# TeamSL.Data.Abstractions
Simple and light-weight library based on Repository pattern to implement basic CRUD operations.

Main features:
- Load single entity
- Find all entities
- Count entities
- Find entities using specification pattern
- Fetching strategies to include related entities
- Ordering

## Entity Framework
Implementation Repository pattern for Entity Framework.

## Examples

#### Load entity
`var user = await _userRepository.Load(userId, cancellationToken);`

#### Find entity
`var user = await _userRepository.Find(new GetUserByEmailSpecification(userEmail), cancellationToken);`

#### Load entity with related entity
`var user = await _userRepository.Load(userId, new UserWithTransactionsFetchStrategy(), accancellationToken);`

#### Find entities and sort
`var users = await _userRepository.FindAll(x => x.Desc(u => u.CreatedDate), cancellationToken);`
