# Valid Constellation Catalog

This workspace contains deliberately valid examples for the main nMolecules roles and for a combined end-to-end flow.

## Domain Building Blocks

- `Address`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[ValueObject]`
  Purpose: immutable value object with `IEquatable<Address>`

- `CustomerProfile`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Entity]`
  Purpose: entity with exactly one identity and a value-object reference

- `Invoice`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[AggregateRoot]`
  Purpose: aggregate root with exactly one identity and value-object state

- `IInvoices`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Repository]`
  Purpose: repository abstraction kept in the domain layer

- `InvoiceFactory`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Factory]`
  Purpose: factory creating a valid aggregate root

- `InvoiceSettlementPolicy`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[DomainService]`
  Purpose: domain service expressing business policy

## Application And Host Layers

- `InvoiceSettlementUseCase`
  File: `src/Banking.Application/ApplicationConstellationExamples.cs`
  Markers: `[ApplicationLayer]`, `[ApplicationService]`
  Purpose: application service orchestrating repository and domain service usage

- `InMemoryInvoices`
  File: `src/Banking.Infrastructure/InfrastructureConstellationExamples.cs`
  Markers: `[InfrastructureLayer]`
  Purpose: infrastructure implementation for the domain repository

- `InvoiceEndpoints`
  File: `src/Banking.Api/UserInterfaceConstellationExamples.cs`
  Markers: `[UserInterfaceLayer]`
  Purpose: user-interface endpoint depending on the application layer only

## Combined Valid Flow

The combined valid path is:

`InvoiceEndpoints -> InvoiceSettlementUseCase -> IInvoices + InvoiceSettlementPolicy -> Invoice`

This gives one navigable, working sample that combines multiple roles without raising analyzer diagnostics.
