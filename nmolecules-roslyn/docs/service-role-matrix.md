# nMolecules Service Role Matrix

Status: March 3, 2026

This document describes the functional separation between `Service`, `DomainService`, and `ApplicationService`.
It is the basis for dedicated analyzer rules.

## Goal

The analyzer currently treats `DomainService` and `ApplicationService` as service-like roles in several shared restrictions so that existing rules remain conservative and immediately useful.
That is practical in the short term, but not sufficient in the long term because the three roles are not semantically identical.

## Roles

### `Service`

Meaning:

- historical, general-purpose marker in the current API

Position:

- acceptable as a compatibility umbrella
- not precise enough as the long-term semantic basis

### `DomainService`

Meaning:

- part of the domain model
- captures domain behavior that does not naturally belong in an entity, aggregate, or value object

Expectations:

- no infrastructure responsibility
- no UI responsibility
- should express domain logic, not workflow orchestration

### `ApplicationService`

Meaning:

- orchestrates use cases
- coordinates domain objects and supporting abstractions
- belongs to the application layer, not the domain model

Expectations:

- may coordinate workflows
- may consume repositories and domain services
- should not replace domain policy with application scripting

## Short-Term Rule Strategy

Phase 1:

- `DomainService` and `ApplicationService` are included in the general service restrictions
- plain legacy `Service` now also emits a migration warning

Examples:

- entity must not reference `DomainService`
- entity must not reference `ApplicationService`
- repository must not reference `DomainService`
- repository must not reference `ApplicationService`
- value object must not reference any of the three service roles

## Target Rules for Later Phases

### Rules for `DomainService`

- must not depend on `ApplicationService`
- should not expose UI types
- should not expose infrastructure-heavy framework APIs
- should remain clearly inside the domain layer

### Rules for `ApplicationService`

- may coordinate domain objects
- may consume repositories
- may consume domain services
- should not also be a domain building block
- should not depend on the ambiguous legacy `Service` marker

### Rules for Legacy `Service`

- compatibility remains available for now
- new work should prefer `DomainService` or `ApplicationService`
- the current migration warning is the first enforcement step

## Open Decisions

- should `Service` eventually be deprecated or remain a compatibility alias?
- does `ApplicationService -> ApplicationService` make sense as a valid dependency?
- should repositories forbid application services explicitly, or is the shared service restriction sufficient?

## Next Technical Steps

1. decide `ApplicationService -> ApplicationService`
2. harden `DomainService` signatures against UI and infrastructure types
3. document and stabilize the `Service` migration path
4. connect the service role matrix to layer rules
