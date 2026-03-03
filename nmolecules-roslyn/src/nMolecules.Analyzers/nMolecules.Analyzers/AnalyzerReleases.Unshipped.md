### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
XMoleculesDomainService0001 | DDD | Error | DomainService should not use application services
XMoleculesIdentity0001 | DDD | Error | Identity members must belong to entities or aggregate roots
XMoleculesEntity0005 | DDD | Error | Entity should declare exactly one identity member
XMoleculesAggregateRoot0004 | DDD | Error | Aggregate root should declare exactly one identity member
XMoleculesApplicationService0001 | DDD | Error | Application service should not also be a domain building block
XMoleculesApplicationService0002 | DDD | Warning | Application service should depend on explicit domain services
XMoleculesAggregateRoot0005 | DDD | Error | Aggregate root should not reference aggregate roots directly
XMoleculesFactory0001 | DDD | Error | Factory should not use application services
XMoleculesService0001 | DDD | Warning | Service should use a specific role marker
XMoleculesValueObject0006 | DDD | Error | Value object should not declare identity members
