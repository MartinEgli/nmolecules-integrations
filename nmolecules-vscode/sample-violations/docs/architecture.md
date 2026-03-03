# Broken Banking Architecture

This workspace keeps the same high-level banking theme as the main sample, but it is intentionally modeled incorrectly.

It demonstrates how the VS Code extension should surface nMolecules analyzer violations in the Problems view.

## Included Layers

- `Banking.Violations.Domain`
- `Banking.Violations.Application`
- `Banking.Violations.Infrastructure`

## Deliberate Mistakes

- mutable value objects
- identity markers on value objects
- aggregate roots with more than one identity member
- application services that also pretend to be domain building blocks
- factories that depend on application services
- legacy `[Service]` markers that should be replaced with a specific role
