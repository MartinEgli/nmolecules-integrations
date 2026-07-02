# nMolecules DDD Rule Catalog

Status: June 29, 2026

This document defines the shared DDD rule baseline for all integrations.

Integration goal:

- same rule semantics in Roslyn, Visual Studio, and VS Code
- host-specific UX only (messages and IDs remain semantically identical)

## Implemented Baseline (Current)

The DDD analyzer baseline includes:

- identity ownership and singularity rules
- aggregate root, entity, value object, repository, factory, domain-service, and application-service dependency boundaries
- migration warning for legacy `Service`
- module metadata and boundary consistency through `XMoleculesModule0007`
- bounded-context metadata and dependency-boundary consistency through `XMoleculesBoundedContext0010`

For exact IDs and wording, the canonical source is:

- `docs/architecture/analyzer-rule-map.md` (superproject)
- `src/nMolecules.Analyzers/nMolecules.Analyzers/AnalyzerReleases.*.md`

## Current Focus

The MVP baseline is complete enough for cross-IDE delivery.
Current depth work is focused on:

- bounded-context dependency semantics beyond the current acyclic graph baseline (`XMoleculesBoundedContext0011+`)
- later conditional-policy depth and severity refinement where needed
- keeping rule map, release catalog, tests, and samples synchronized

Recent closure:

- direct DDD dependency-matrix prohibitions are now explicit for Entity, AggregateRoot, ValueObject, and Repository service-role pairs
- analyzer message quality baseline is complete: diagnostic descriptions now explain why a violation happens, which architectural rule is violated, and which target correction is intended

## Rule Metadata Requirements

Each rule must have:

- stable `XMolecules*` diagnostic ID
- category
- severity
- message template and explanation
- positive and negative analyzer tests
- release catalog entry (`AnalyzerReleases.*.md`)
- rule-map documentation entry

## Definition of Done for New Rules

A rule is done only when:

- implementation and documentation are both updated
- ID and message are final and synchronized
- analyzer tests cover healthy and violating cases
- release catalog includes the rule ID
- rule-doc sync gate passes
