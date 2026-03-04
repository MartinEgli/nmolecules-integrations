### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
XMoleculesDomainService0001 | DDD | Error | DomainService should not use application services
XMoleculesDomainService0002 | DDD | Error | DomainService should only use repository contracts
XMoleculesIdentity0001 | DDD | Error | Identity members must belong to entities or aggregate roots
XMoleculesEntity0005 | DDD | Error | Entity should declare exactly one identity member
XMoleculesAggregateRoot0004 | DDD | Error | Aggregate root should declare exactly one identity member
XMoleculesApplicationService0001 | DDD | Error | Application service should not also be a domain building block
XMoleculesApplicationService0002 | DDD | Warning | Application service should depend on explicit domain services
XMoleculesApplicationService0003 | DDD | Warning | Application service should not depend on other application services
XMoleculesApplicationService0004 | DDD | Warning | Application service should not expose infrastructure-layer types in public signatures
XMoleculesAggregateRoot0005 | DDD | Error | Aggregate root should not reference aggregate roots directly
XMoleculesAggregateRoot0007 | DDD | Error | Aggregate root should not depend on external factories by default
XMoleculesFactory0001 | DDD | Error | Factory should not use application services
XMoleculesFactory0002 | DDD | Error | Factory should not use repositories
XMoleculesFactory0003 | DDD | Error | Factory must not also be a domain building block
XMoleculesService0001 | DDD | Warning | Service should use a specific role marker
XMoleculesValueObject0006 | DDD | Error | Value object should not declare identity members
XMoleculesDomainEvent0001 | Events | Error | Domain events must not reference entities
XMoleculesDomainEvent0002 | Events | Error | Domain events must not reference aggregate roots
XMoleculesDomainEvent0003 | Events | Error | Domain events must not reference repositories
XMoleculesDomainEvent0004 | Events | Error | Domain events must not reference services
XMoleculesDomainEvent0005 | Events | Warning | Domain event publishers should prefer aggregate roots or application services
XMoleculesDomainEvent0006 | Events | Error | Repositories and factories must not publish domain events directly
XMoleculesDomainEvent0007 | Events | Error | Domain event handlers must consume domain events
XMoleculesDomainEvent0008 | Events | Warning | Domain event publishers should expose domain event payloads explicitly
XMoleculesRepository0002 | DDD | Warning | Repositories must not expose infrastructure-layer types in public signatures
XMoleculesRepository0003 | DDD | Warning | Repositories should not depend on other repositories directly
XMoleculesLayered0001 | Architecture | Error | Domain layers must not depend on application layers
XMoleculesLayered0002 | Architecture | Error | Domain layers must not depend on infrastructure layers
XMoleculesLayered0003 | Architecture | Error | Domain layers must not depend on interface layers
XMoleculesLayered0004 | Architecture | Error | Interface layers must not depend on domain layers directly
XMoleculesCQRS0001 | Architecture | Error | CQRS support requires both query and query handler markers
XMoleculesCQRS0002 | Architecture | Error | Command handlers must not depend on query models directly
XMoleculesCQRS0003 | Architecture | Error | Query handlers must stay on the read side
XMoleculesCQRS0004 | Architecture | Error | Query models must be read-only
XMoleculesCQRS0005 | Architecture | Error | Projections may update query models but must not depend on write-side roles directly
XMoleculesCQRS0006 | Architecture | Error | Command dispatchers must route and must not contain domain rules
