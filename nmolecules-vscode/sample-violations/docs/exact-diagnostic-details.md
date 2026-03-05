# Exact Diagnostic Details

These are the intended example messages for the isolated samples. They are written against the current analyzer message templates.

- `IdentityValueObject.ExternalId`
  `Value object 'IdentityValueObject' must not declare [Identity] member 'ExternalId'`

- `MutableValueObject.Code`
  `Value object 'MutableValueObject' must be immutable; member 'Code' is writable`

- `MissingEquatableValueObject`
  `Value object 'MissingEquatableValueObject' must implement IEquatable<MissingEquatableValueObject>`

- `NotSealedValueObject`
  `Value object 'NotSealedValueObject' should be sealed or declared as a value type`

- `MultiIdentityAccount`
  `Aggregate root 'MultiIdentityAccount' declares 2 [Identity] members: AccountId, CorrelationId. Exactly one is allowed`

- `FactoryWithIdentity.MisplacedId`
  `[Identity] member 'MisplacedId' is declared in 'FactoryWithIdentity', but only [Entity] or [AggregateRoot] may own identities`

- `LegacyRiskService`
  `Type 'LegacyRiskService' uses legacy [Service]; replace it with [DomainService] or [ApplicationService]`

- `PolicyDependingOnApplicationService.orchestrator`
  `Domain service 'orchestrator' must not depend on application service 'ApplicationOrchestrator'`

- `ConfusedApplicationService`
  `Application service 'ConfusedApplicationService' must not also be marked as 'ValueObject'`

- `ApplicationServiceUsingLegacyService.pricingService`
  `Application service 'pricingService' depends on legacy [Service] type 'LegacyPricingService'; use [DomainService] instead`

- `FactoryUsingApplicationService.workflow`
  `Factory 'workflow' must not depend on application service 'WorkflowFacade'`

- `DomainLayerUsingOtherLayers.applicationService`
  `Domain layer symbol 'applicationService' must not depend on application layer type 'ImportedApplicationService'`

- `DomainLayerUsingOtherLayers.infrastructureGateway`
  `Domain layer symbol 'infrastructureGateway' must not depend on infrastructure layer type 'ImportedInfrastructureGateway'`

- `DomainLayerUsingOtherLayers.viewModel`
  `Domain layer symbol 'viewModel' must not depend on interface layer type 'ImportedViewModel'`

- `MissingQueryHandlerQuery`
  `Query 'MissingQueryHandlerQuery' requires at least one [QueryHandler] in the same compilation`

- `EventLeakingAggregate.Account`
  `Domain event 'EventLeakingAggregate' must not reference aggregate root 'MultiIdentityAccount' in member 'Account'`

- `ForbiddenEventPublisherFactory`
  `Factory 'ForbiddenEventPublisherFactory' must not be marked as a domain event publisher`

- `BrokenEventHandlers.HandleWithoutPayload`
  `Domain event handler 'HandleWithoutPayload' must declare at least one parameter of a [DomainEvent] type`

- `BrokenEventHandlers.HandleTooMany`
  `Domain event handler 'HandleTooMany' declares multiple [DomainEvent] parameters`

- module metadata in `ArchitectureFamilyViolations.cs`
  `Module on module 'Banking.Violations.Domain.dll' references unknown BoundedContextId 'UnknownContext'. Declared bounded contexts: Billing, Sales`
  `Module on assembly 'Banking.Violations.Domain' declares Name/Value 'Accounts' in BoundedContextId 'Billing' with Id 'Accounts.Core', but this name maps to multiple Ids in that context: Accounts.Api, Accounts.Core`

- bounded-context metadata in `ArchitectureFamilyViolations.cs`
  `BoundedContext on module 'Banking.Violations.Domain.dll' declares Id 'Sales', but compilation contains multiple BoundedContext Ids: Billing, Sales`

- onion/cross-style mix in `ArchitectureFamilyViolations.cs`
  `Onion declaration on type 'ClassicDomainRing' mixes classic and simplified markers in the same compilation`
  `Scope 'bounded context 'Billing'' contains incompatible primary structural styles: Layered + Onion`
  `Bounded context 'bounded context 'Billing'' mixes classic and simplified onion markers`
