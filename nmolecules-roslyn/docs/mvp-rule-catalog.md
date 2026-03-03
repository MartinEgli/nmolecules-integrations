# nMolecules DDD Rule Catalog

Status: March 3, 2026

This document defines the first shared rule catalog for DDD checks.
It is the functional foundation for:

- Roslyn analyzers
- Visual Studio integration
- Visual Studio Code integration
- user and contributor documentation

All IDE integrations should consume the same rule semantics.
Only the presentation should differ per host, not the domain meaning of the diagnostics.

## Rule Groups

### Group A: Identity and Structural Rules

- entity must declare exactly one identity
- aggregate root must declare exactly one identity
- identity is only valid inside entity or aggregate root
- value object must not declare identity
- value object should implement `IEquatable<T>`
- value object should be `sealed` when it is a class

### Group B: Dependency Rules

- aggregate root must not use repository
- aggregate root must not use service roles
- aggregate root must not directly reference aggregate root
- entity must not use repository
- entity must not use aggregate root
- entity must not use service roles
- repository must not use service roles
- value object must not use entity
- value object must not use aggregate root
- value object must not use repository
- value object must not use service roles
- domain service must not use application service
- factory must not use application service
- application service should not depend on plain legacy service

### Group C: Role Rules

- application service must not also be a domain building block
- plain legacy service should migrate to a more specific role

### Group D: Future Architecture Rules

- domain layer must not reference infrastructure
- domain layer must not reference UI
- application layer boundaries must be clarified
- module and bounded context rules will be added later

## Current MVP Focus

Phase 1 concentrates on rules that are either already present in code or close to being enforceable:

- keep existing rules stable and documented
- finish service-role separation
- harden repository and domain service signatures
- keep rule IDs, messages, and tests aligned

## Rule Metadata Requirements

Each rule should ultimately have:

- stable diagnostic ID
- category
- severity
- short message
- explanation
- positive and negative test cases
- optional code-fix expectation

## Definition of Done for New Rules

A rule is only done when:

- the functional rule is documented
- the diagnostic ID is stable
- positive and negative tests exist
- severity and message are fixed
- it is clear whether a code fix exists
- the same meaning can be surfaced in Visual Studio and later in VS Code
