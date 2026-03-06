## Release 0.0.0.6

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
XMoleculesAggregateRoot0001 | DDD | Error | Aggregate roots must not depend on repositories
XMoleculesAggregateRoot0002 | DDD | Error | Aggregate roots must not depend on domain services
XMoleculesAggregateRoot0003 | DDD | Error | Aggregate roots must define an identity
XMoleculesEntity0001 | DDD | Error | Entities must not depend on repositories
XMoleculesEntity0002 | DDD | Error | Entities must not depend on aggregate roots
XMoleculesEntity0003 | DDD | Error | Entities must not depend on services
XMoleculesEntity0004 | DDD | Error | Entities must define an identity
XMoleculesRepository0001 | DDD | Error | Repositories must not depend on services
XMoleculesValueObject0001 | DDD | Error | Value objects must not depend on entities
XMoleculesValueObject0002 | DDD | Error | Value objects must not depend on services
XMoleculesValueObject0003 | DDD | Error | Value objects must not depend on repositories
XMoleculesValueObject0004 | DDD | Error | Value objects must not depend on aggregate roots
XMoleculesValueObject0005 | DDD | Error | Value objects must be immutable
XMoleculesValueObject1001 | DDD | Error | Value objects should implement IEquatable<T> (.NET specific)
XMoleculesValueObject1002 | DDD | Error | Value objects should be sealed (.NET specific)
