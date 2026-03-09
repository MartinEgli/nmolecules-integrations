### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
XMoleculesDomainService0001 | DDD | Error | Domain services must not depend on application services
XMoleculesDomainService0002 | DDD | Error | Domain services must depend on repository contracts only
XMoleculesDomainService0003 | DDD | Warning | Domain services should not expose infrastructure-layer types in public signatures
XMoleculesDomainService0004 | DDD | Warning | Domain services should not depend on legacy services
XMoleculesIdentity0001 | DDD | Error | Identity members must belong to entities or aggregate roots
XMoleculesEntity0005 | DDD | Error | Entities must declare exactly one identity
XMoleculesEntity0006 | DDD | Error | Entities must not depend on factories
XMoleculesEntity0007 | DDD | Error | Entities must not depend on application services
XMoleculesEntity0008 | DDD | Error | Entities must not depend on legacy services
XMoleculesAggregateRoot0004 | DDD | Error | Aggregate roots must declare exactly one identity
XMoleculesApplicationService0001 | DDD | Error | Application services must not also be domain building blocks
XMoleculesApplicationService0002 | DDD | Warning | Application services should not depend on legacy services
XMoleculesApplicationService0003 | DDD | Warning | Application services should not depend on other application services directly
XMoleculesApplicationService0004 | DDD | Warning | Application services should not expose infrastructure-layer types in public signatures
XMoleculesAggregateRoot0005 | DDD | Error | Aggregate roots must not reference other aggregate roots directly
XMoleculesAggregateRoot0006 | DDD | Error | Aggregate roots must not depend on application services
XMoleculesAggregateRoot0007 | DDD | Error | Aggregate roots must not depend on factories
XMoleculesAggregateRoot0008 | DDD | Error | Aggregate roots must not depend on legacy services
XMoleculesFactory0001 | DDD | Error | Factories must not depend on application services
XMoleculesFactory0002 | DDD | Error | Factories must not depend on repositories
XMoleculesFactory0003 | DDD | Error | Factories must not also be domain building blocks
XMoleculesFactory0004 | DDD | Error | Factories must not depend on other factories
XMoleculesService0001 | DDD | Warning | Legacy services should use explicit role markers
XMoleculesBoundedContext0001 | DDD | Warning | BoundedContext should define stable Id metadata
XMoleculesBoundedContext0002 | DDD | Warning | BoundedContext should define readable Name metadata
XMoleculesBoundedContext0003 | DDD | Warning | BoundedContext declarations should use a single Id per compilation
XMoleculesBoundedContext0004 | DDD | Warning | BoundedContext declarations with same Id should use one Name/Value
XMoleculesBoundedContext0005 | DDD | Warning | Module ownership should match the bounded context declared on the same metadata scope
XMoleculesBoundedContext0006 | DDD | Warning | BoundedContext dependencies should reference declared contexts
XMoleculesBoundedContext0007 | DDD | Warning | BoundedContext dependency direction should be unidirectional per context pair
XMoleculesBoundedContext0008 | DDD | Warning | BoundedContext should not depend on itself
XMoleculesBoundedContext0009 | DDD | Warning | BoundedContext dependency targets should be unique
XMoleculesModule0001 | DDD | Warning | Module should define stable Id metadata
XMoleculesModule0002 | DDD | Warning | Module should define readable Name metadata
XMoleculesModule0003 | DDD | Warning | Module should define BoundedContextId metadata
XMoleculesModule0004 | DDD | Warning | Module should reference a declared bounded context
XMoleculesModule0005 | DDD | Warning | Module declarations with same Id should use one Name/Value
XMoleculesModule0006 | DDD | Warning | Module declarations with same Id should use one BoundedContextId
XMoleculesModule0007 | DDD | Warning | Module names should map to one Id inside a bounded context
XMoleculesValueObject0006 | DDD | Error | Value objects must not declare identities
XMoleculesValueObject0007 | DDD | Error | Value objects must not depend on factories
XMoleculesValueObject0008 | DDD | Error | Value objects must not depend on application services
XMoleculesValueObject0009 | DDD | Error | Value objects must not depend on legacy services
XMoleculesDomainEvent0001 | Events | Error | Domain events must not reference entities
XMoleculesDomainEvent0002 | Events | Error | Domain events must not reference aggregate roots
XMoleculesDomainEvent0003 | Events | Error | Domain events must not reference repositories
XMoleculesDomainEvent0004 | Events | Error | Domain events must not reference services
XMoleculesDomainEvent0005 | Events | Warning | Domain event publishers should prefer aggregate roots or application services
XMoleculesDomainEvent0006 | Events | Error | Repositories and factories must not publish domain events directly
XMoleculesDomainEvent0007 | Events | Error | Domain event handlers must consume domain events
XMoleculesDomainEvent0008 | Events | Warning | Domain event publishers should expose domain event payloads explicitly
XMoleculesDomainEvent0009 | Events | Warning | Domain event handlers should consume exactly one domain event payload
XMoleculesRepository0002 | DDD | Warning | Repositories should not expose infrastructure-layer types in public signatures
XMoleculesRepository0003 | DDD | Warning | Repositories should not depend on other repositories directly
XMoleculesRepository0004 | DDD | Warning | Approved repository composition should depend on repository contracts only
XMoleculesRepository0005 | DDD | Warning | Repositories should not depend on factories
XMoleculesRepository0006 | DDD | Error | Repositories must not depend on application services
XMoleculesRepository0007 | DDD | Error | Repositories must not depend on legacy services
XMoleculesLayered0001 | Architecture | Error | Domain layers must not depend on application layers
XMoleculesLayered0002 | Architecture | Error | Domain layers must not depend on infrastructure layers
XMoleculesLayered0003 | Architecture | Error | Domain layers must not depend on interface layers
XMoleculesLayered0004 | Architecture | Error | Interface layers must not depend on domain layers directly
XMoleculesLayered0005 | Architecture | Warning | Application layers should keep infrastructure dependencies explicit and limited
XMoleculesLayered0006 | Architecture | Warning | Infrastructure layers should keep application dependencies wiring-only
XMoleculesOnion0001 | Architecture | Error | Onion dependencies must point inward only
XMoleculesOnion0002 | Architecture | Error | Domain model rings must not depend on outer rings
XMoleculesOnion0003 | Architecture | Error | Domain service rings must not depend on outer rings
XMoleculesOnion0004 | Architecture | Error | Application service rings must not depend on infrastructure rings
XMoleculesOnion0005 | Architecture | Error | Classic and simplified onion styles must not mix in the same compilation
XMoleculesHexagonal0001 | Architecture | Error | Hexagonal application core must not depend on ports or adapters
XMoleculesHexagonal0002 | Architecture | Error | Primary ports must not depend on adapters
XMoleculesHexagonal0003 | Architecture | Error | Secondary ports must not depend on adapters
XMoleculesHexagonal0004 | Architecture | Warning | Primary adapters should depend on primary ports
XMoleculesHexagonal0005 | Architecture | Warning | Secondary adapters should depend on secondary ports
XMoleculesCQRS0001 | Architecture | Error | CQRS queries and query handlers must coexist in the same compilation
XMoleculesCQRS0002 | Architecture | Error | Command handlers must not depend on query models directly
XMoleculesCQRS0003 | Architecture | Error | Query handlers must stay on the read side
XMoleculesCQRS0004 | Architecture | Error | Query models must be read-only
XMoleculesCQRS0005 | Architecture | Error | Projections may update query models but must not depend on write-side roles directly
XMoleculesCQRS0006 | Architecture | Error | Command dispatchers must route and must not contain domain rules
XMoleculesCrossStyle0001 | Architecture | Error | Primary structural styles must follow the compatibility matrix
XMoleculesCrossStyle0002 | Architecture | Warning | CQRS should overlay a primary structural style
XMoleculesCrossStyle0003 | Architecture | Error | Classic and simplified onion styles must not coexist in the same bounded context
XMoleculesBricks0001 | Architecture | Error | Brick rules must be honored
XMoleculesBricks0002 | Architecture | Warning | Brick rule configuration must be valid
XMoleculesBricks0003 | Architecture | Error | Brick member contracts must declare exactly one required marker
XMoleculesBricks0004 | Architecture | Error | Brick member contracts must include all required markers
XMoleculesBricks0005 | Architecture | Error | Brick member contracts must use the configured marker count
XMoleculesBricks0006 | Architecture | Error | Brick member contracts must satisfy an exclusive choice
XMoleculesEventStorming0001 | Architecture | Error | Event Storming actors must not depend on aggregates
XMoleculesEventStorming0002 | Architecture | Warning | Event Storming commands should target aggregates
XMoleculesEventStorming0003 | Architecture | Warning | Event Storming policies should react to domain events
XMoleculesEventStorming0004 | Architecture | Error | Event Storming read models must not depend on aggregates
XMoleculesEventStorming0005 | Architecture | Error | Event Storming external systems must not depend on aggregates
XMoleculesMicroservices0001 | Architecture | Warning | API gateways should depend on service contracts
XMoleculesMicroservices0002 | Architecture | Warning | BFF components should depend on service contracts
XMoleculesMicroservices0003 | Architecture | Error | Service contracts must not depend on microservice implementations
XMoleculesMicroservices0004 | Architecture | Error | Integration events must not depend on microservice implementations
XMoleculesMicroservices0005 | Architecture | Warning | Saga orchestrators should depend on contracts or integration events
XMoleculesMicroservices0006 | Architecture | Error | Saga participants must not depend on gateways or BFF components
