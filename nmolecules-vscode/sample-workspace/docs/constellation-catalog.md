# Valid Constellation Catalog

This workspace contains deliberately valid examples for all major rule families currently implemented.

## DDD Core Roles

- `Address`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[ValueObject]`

- `CustomerProfile`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Entity]`, `[Identity]`

- `Invoice`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[AggregateRoot]`, `[Identity]`

- `IInvoices`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Repository]`

- `InvoiceFactory`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[Factory]`

- `InvoiceSettlementPolicy`
  File: `src/Banking.Domain/ConstellationExamples.cs`
  Markers: `[DomainLayer]`, `[DomainService]`

- `InvoiceSettlementUseCase`
  File: `src/Banking.Application/ApplicationConstellationExamples.cs`
  Markers: `[ApplicationLayer]`, `[ApplicationService]`

## Layered Architecture

- `InvoiceSettlementUseCase`
  File: `src/Banking.Application/ApplicationConstellationExamples.cs`
  Markers: `[ApplicationLayer]`

- `InMemoryInvoices`
  File: `src/Banking.Infrastructure/InfrastructureConstellationExamples.cs`
  Markers: `[InfrastructureLayer]`

- `InvoiceEndpoints`
  File: `src/Banking.Api/UserInterfaceConstellationExamples.cs`
  Markers: `[UserInterfaceLayer]`

## CQRS

- `FindInvoiceBalance`, `InvoiceReadHandlers`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[Query]`, `[QueryHandler]`

- `InvoiceBalanceView`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[QueryModel]` (read-only shape)

- `InvoiceBalanceProjection`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[Projection]`

- `SettleInvoice`, `InvoiceWriteHandlers`, `InvoiceDispatcher`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[Command]`, `[CommandHandler]`, `[CommandDispatcher]`

## Hexagonal Architecture

- `InvoiceDecisionCore`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[Application]`

- `IInvoiceSubmissionPort`, `IInvoiceLedgerPort`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[PrimaryPort]`, `[SecondaryPort]`

- `HttpInvoiceSubmissionAdapter`, `InMemoryInvoiceLedgerAdapter`
  File: `src/Banking.Application/CqrsAndHexagonalConstellationExamples.cs`
  Markers: `[PrimaryAdapter]`, `[SecondaryAdapter]`

## Domain Events

- `InvoicePaidEvent`
  File: `src/Banking.Domain/EventConstellationExamples.cs`
  Markers: `[DomainEvent]`

- `InvoiceEventSource.PublishPaid`
  File: `src/Banking.Domain/EventConstellationExamples.cs`
  Markers: `[DomainEventPublisher]`

- `InvoiceEventHandlers.Handle`
  File: `src/Banking.Domain/EventConstellationExamples.cs`
  Markers: `[DomainEventHandler]`

## Bounded Context And Module Metadata

- assembly/module metadata
  File: `src/Banking.Domain/ContextMetadata.cs`
  Markers: `[BoundedContext]`, `[Module]`

## Combined Valid Flow

`InvoiceEndpoints -> InvoiceSettlementUseCase -> IInvoices + InvoiceSettlementPolicy -> Invoice`

This gives one navigable, working sample that combines layered, DDD, CQRS, hexagonal, event, and metadata markers without analyzer violations.
