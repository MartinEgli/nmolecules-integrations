# Release Tracking Investigation

Status: March 3, 2026

## Problem

The analyzer project produces Roslyn release tracking warnings `RS2002` or `RS2003` even though the rule IDs are present in the analyzer classes.

Observed effect:

- `AnalyzerReleases.Unshipped.md` can trigger `RS2002`
- moving the same rules to `AnalyzerReleases.Shipped.md` can trigger `RS2003`

Both outcomes suggest that `ReleaseTrackingAnalyzers` does not reliably map the existing diagnostics to the current analyzer types.

## Likely Cause

The analyzers implement most logic through a generic base type:

- `Analyzer<TAttribute> : DiagnosticAnalyzer`

Probable consequence:

- `ReleaseTrackingAnalyzers` does not consistently recognize the concrete `SupportedDiagnostics` of the derived analyzers
- as a result, rule IDs can appear unsupported even though they are still active

This matches the current symptom:

- not just a single rule is affected
- the issue can apply to the whole analyzer set

## Current Handling

`RS2002` and `RS2003` are currently suppressed so that:

- builds remain readable
- functional rule work can continue
- the release tracking files can still be maintained as the semantic inventory

## Long-Term Options

### Option 1: Refactor Analyzer Structure

- derive analyzers directly from `DiagnosticAnalyzer`
- reduce or remove the generic base class

Advantage:

- best chance of native compatibility with `ReleaseTrackingAnalyzers`

Disadvantage:

- larger structural refactoring

### Option 2: Adjust the Tracking Strategy

- stop treating release tracking analyzers as hard build warnings
- maintain rule inventory through docs and tests

Advantage:

- much less structural change

Disadvantage:

- weaker automatic protection of release history

## Recommended Next Step

1. continue functional rule work
2. run a small spike with one analyzer implemented without the generic base type
3. verify whether `RS2002/RS2003` disappears in that reduced scenario
