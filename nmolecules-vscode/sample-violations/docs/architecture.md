# Broken Banking Architecture

This workspace keeps the same high-level banking theme as the main sample, but it is intentionally modeled incorrectly.

It demonstrates how the VS Code extension should surface nMolecules analyzer violations in the Problems view.

## Included Projects

- `Banking.Violations.Domain`
- `Banking.Violations.Application`
- `Banking.Violations.Infrastructure`
- `Banking.Violations.RuleMatrix`
- `Banking.Violations.MetadataMissing`
- `Banking.Violations.MetadataConsistency`
- `Banking.Violations.CqrsOnly`

## Deliberate Mistakes

- mutable value objects
- identity markers on value objects
- aggregate roots with more than one identity member
- application services that also pretend to be domain building blocks
- factories that depend on application services
- legacy `[Service]` markers that should be replaced with a specific role
- CQRS declarations without the required complementary handler/query markers
- mixed Onion style declarations (classic and simplified) in one bounded context
- cross-style collisions between layered and onion primary styles
- module metadata pointing to an undeclared bounded-context identifier
- domain-event contracts that leak aggregate references or violate handler/publisher rules
- metadata declarations with missing and inconsistent bounded-context/module fields
- CQRS-only style declarations without a primary structural style marker
- dedicated rule-matrix violations for the remaining analyzer families
