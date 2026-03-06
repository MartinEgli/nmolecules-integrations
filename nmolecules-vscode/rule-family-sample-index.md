# Rule Family Sample Index

This index is the fast navigation entry for Feature 5.1 sample coverage.

Each major rule family has at least one discoverable sample in either the healthy workspace or the violations workspace.

## DDD Core

- Entity/Aggregate/Identity/ValueObject/Repository/Factory/DomainService/ApplicationService
  Valid: `sample-workspace/src/Banking.Domain/ConstellationExamples.cs`, `sample-workspace/src/Banking.Application/ApplicationConstellationExamples.cs`
  Violations: `sample-violations/src/Banking.Violations.Domain/IsolatedValueObjectViolations.cs`, `sample-violations/src/Banking.Violations.Domain/IdentityAndServiceViolations.cs`, `sample-violations/src/Banking.Violations.Application/ApplicationViolationCatalog.cs`, `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Layered

- Valid: `sample-workspace/src/Banking.Api/UserInterfaceConstellationExamples.cs`, `sample-workspace/src/Banking.Application/ApplicationConstellationExamples.cs`, `sample-workspace/src/Banking.Infrastructure/InfrastructureConstellationExamples.cs`
- Violations: `sample-violations/src/Banking.Violations.Infrastructure/FactoryAndLayerViolations.cs`, `sample-violations/src/Banking.Violations.Application/ApplicationViolationCatalog.cs`, `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## CQRS

- Valid: `sample-workspace/src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
- Violations: `sample-violations/src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`, `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Domain Events

- Valid: `sample-workspace/src/Banking.Domain/EventConstellationExamples.cs`
- Violations: `sample-violations/src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`, `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Hexagonal

- Valid: `sample-workspace/src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
- Violations: `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Onion

- Violations: `sample-violations/src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`, `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`
- Valid inward-only examples: analyzer-focused examples in `nmolecules-roslyn/test/nMolecules.Analyzers.Test/OnionAnalyzerTests/OnionDependencies.cs`

## BoundedContext And Module Metadata

- Valid: `sample-workspace/src/Banking.Domain/ContextMetadata.cs`
- Violations: `sample-violations/src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`, `sample-violations/src/Banking.Violations.MetadataMissing/MetadataMissingViolations.cs`, `sample-violations/src/Banking.Violations.MetadataConsistency/MetadataConsistencyViolations.cs`

## CrossStyle

- Violations: `sample-violations/src/Banking.Violations.Domain/ArchitectureFamilyViolations.cs`, `sample-violations/src/Banking.Violations.CqrsOnly/CqrsOnlyViolations.cs`
- Focused analyzer examples: `nmolecules-roslyn/test/nMolecules.Analyzers.Test/CrossStyleAnalyzerTests/CrossStyleCompatibility.cs`

## EventStorming

- Valid: analyzer-focused examples in `nmolecules-roslyn/test/nMolecules.Analyzers.Test/EventStormingAnalyzerTests/EventStormingDependencies.cs`
- Violations: `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Microservices

- Valid: analyzer-focused examples in `nmolecules-roslyn/test/nMolecules.Analyzers.Test/MicroservicesAnalyzerTests/MicroservicesDependencies.cs`
- Violations: `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`

## Bricks

- Violations: `sample-violations/src/Banking.Violations.RuleMatrix/RuleMatrixViolations.cs`
