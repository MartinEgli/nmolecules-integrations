# nMolecules VS Code Violations Workspace

This workspace is the deliberately broken companion to the main sample.

Use it to validate that the VS Code extension can:

- refresh nMolecules diagnostics into the Problems view
- point developers from diagnostics to the workspace documentation
- demonstrate concrete DDD and layered-architecture violations
- show isolated single-rule violations and combined multi-rule violations

Diagnostics are expected to be read in three layers:

- the title states the rule
- the message points to the concrete offending symbol
- the description explains why the violation happens, which architectural rule is broken, and which correction direction is intended

## Open In VS Code

Open:

```text
nmolecules-integrations/nmolecules-vscode/sample-violations/nmolecules-violations.code-workspace
```

The workspace is configured to target `Banking.Sample.Violations.sln` when you run:

- `nMolecules: Refresh Diagnostics`
- `nMolecules: Open Workspace Docs`

## Open In Visual Studio

Open:

```text
nmolecules-integrations/nmolecules-vscode/sample-violations/Banking.Sample.Violations.sln
```

Then confirm:

1. the solution loads with the local analyzer project references intact
2. the Error List shows multiple `XMolecules*` diagnostics
3. at least one violation from the current family under development is visible in the offending project

## Expected Diagnostics

The workspace now includes dedicated rule-matrix projects and is intentionally configured so that every currently implemented `XMolecules*` analyzer rule is violated at least once across the solution.

Coverage helper projects:

- `src/Banking.Violations.RuleMatrix`
- `src/Banking.Violations.MetadataMissing`
- `src/Banking.Violations.MetadataConsistency`
- `src/Banking.Violations.CqrsOnly`

## Build The Sample

```powershell
dotnet build Banking.Sample.Violations.sln -v minimal
```

Validate full rule coverage:

```powershell
./tools/validate-violations-rule-coverage.ps1
```

## Documentation

- [Architecture](docs/architecture.md)
- [Constellation Catalog](docs/constellation-catalog.md)
- [Expected Diagnostics](docs/expected-diagnostics.md)
- [Exact Diagnostic Details](docs/exact-diagnostic-details.md)
- [Diagnostic Reading Guide](docs/exact-diagnostic-details.md#description-shape)
- [Using The Extension](docs/using-the-extension.md)
- [Rule Family Sample Index](../rule-family-sample-index.md)

For channel-parity checks across new rule families, use [../../docs/ide-channel-parity-checklist.md](../../docs/ide-channel-parity-checklist.md).
