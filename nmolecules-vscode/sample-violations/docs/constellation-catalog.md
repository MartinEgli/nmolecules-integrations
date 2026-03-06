# Violation Constellation Catalog

This workspace is split into isolated violations and combined violations.

## Isolated Single-Rule Examples

- `IdentityValueObject`
  File: `src/Banking.Violations.Domain/IsolatedValueObjectViolations.cs`
  Expected primary diagnostic: `XMoleculesValueObject0006`

- `MutableValueObject`
  File: `src/Banking.Violations.Domain/IsolatedValueObjectViolations.cs`
  Expected primary diagnostic: `XMoleculesValueObject0005`

- `MissingEquatableValueObject`
  File: `src/Banking.Violations.Domain/IsolatedValueObjectViolations.cs`
  Expected primary diagnostic: `XMoleculesValueObject1001`

- `NotSealedValueObject`
  File: `src/Banking.Violations.Domain/IsolatedValueObjectViolations.cs`
  Expected primary diagnostic: `XMoleculesValueObject1002`

- `MultiIdentityAccount`
  File: `src/Banking.Violations.Domain/IdentityAndServiceViolations.cs`
  Expected primary diagnostic: `XMoleculesAggregateRoot0004`

- `FactoryWithIdentity`
  File: `src/Banking.Violations.Domain/IdentityAndServiceViolations.cs`
  Expected primary diagnostic: `XMoleculesIdentity0001`

- `LegacyRiskService`
  File: `src/Banking.Violations.Domain/IdentityAndServiceViolations.cs`
  Expected primary diagnostic: `XMoleculesService0001`

- `PolicyDependingOnApplicationService`
  File: `src/Banking.Violations.Domain/IdentityAndServiceViolations.cs`
  Expected primary diagnostic: `XMoleculesDomainService0001`

- `ConfusedApplicationService`
  File: `src/Banking.Violations.Application/ApplicationViolationCatalog.cs`
  Expected primary diagnostic: `XMoleculesApplicationService0001`

- `ApplicationServiceUsingLegacyService`
  File: `src/Banking.Violations.Application/ApplicationViolationCatalog.cs`
  Expected primary diagnostic: `XMoleculesApplicationService0002`

- `FactoryUsingApplicationService`
  File: `src/Banking.Violations.Infrastructure/FactoryAndLayerViolations.cs`
  Expected primary diagnostic: `XMoleculesFactory0001`

- `DomainLayerUsingOtherLayers`
  File: `src/Banking.Violations.Infrastructure/FactoryAndLayerViolations.cs`
  Expected primary diagnostics:
  `XMoleculesLayered0001`, `XMoleculesLayered0002`, `XMoleculesLayered0003`

- `ApplicationLayerUsingUi`
  File: `src/Banking.Violations.Application/ApplicationViolationCatalog.cs`
  Expected primary diagnostic: `XMoleculesLayered0003`

- `MissingQueryHandlerQuery`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostic: `XMoleculesCQRS0001`

- `EventLeakingAggregate`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostic: `XMoleculesDomainEvent0002`

- `ForbiddenEventPublisherFactory`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostic: `XMoleculesDomainEvent0006`

- `BrokenEventHandlers.HandleWithoutPayload`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostic: `XMoleculesDomainEvent0007`

- `BrokenEventHandlers.HandleTooMany`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostic: `XMoleculesDomainEvent0009`

- assembly/module metadata in `ArchitectureFamilyViolations.cs`
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostics: `XMoleculesBoundedContext0003`, `XMoleculesBoundedContext0006`, `XMoleculesBoundedContext0007`, `XMoleculesBoundedContext0008`, `XMoleculesBoundedContext0009`, `XMoleculesModule0004`

- onion/cross-style mix in one bounded context
  File: `src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`
  Expected primary diagnostics: `XMoleculesOnion0005`, `XMoleculesCrossStyle0001`, `XMoleculesCrossStyle0003`

- comprehensive rule-matrix coverage
  File: `src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`
  Expected primary diagnostics: remaining DDD, Events, Layered, Onion (`0001`-`0004`), Hexagonal, CQRS (`0002`-`0006`), EventStorming, Microservices, and Bricks family gaps

- metadata-missing rule coverage
  File: `src/Banking.Violations.MetadataMissing/MetadataMissingViolations.cs`
  Expected primary diagnostics: `XMoleculesBoundedContext0001`, `XMoleculesBoundedContext0002`, `XMoleculesModule0001`, `XMoleculesModule0002`, `XMoleculesModule0003`

- metadata-consistency rule coverage
  File: `src/Banking.Violations.MetadataConsistency/MetadataConsistencyViolations.cs`
  Expected primary diagnostics: `XMoleculesBoundedContext0004`, `XMoleculesModule0005`, `XMoleculesModule0006`

- CQRS-only cross-style overlay coverage
  File: `src/Banking.Violations.CqrsOnly/CqrsOnlyViolations.cs`
  Expected primary diagnostic: `XMoleculesCrossStyle0002`

## Combined Multi-Rule Examples

- `CombinedRoleMismatch`
  File: `src/Banking.Violations.Infrastructure/CombinedViolationScenarios.cs`
  Expected diagnostics: `XMoleculesApplicationService0001` plus the entity-related identity policy

- `CombinedBrokenSnapshot`
  File: `src/Banking.Violations.Infrastructure/CombinedViolationScenarios.cs`
  Expected diagnostics: `XMoleculesValueObject0005`, `XMoleculesValueObject0006`, `XMoleculesValueObject1001`, `XMoleculesValueObject1002`
