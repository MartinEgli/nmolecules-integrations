# nMolecules VS Code Violations Workspace

This workspace is the deliberately broken companion to the main sample.

Use it to validate that the VS Code extension can:

- refresh nMolecules diagnostics into the Problems view
- point developers from diagnostics to the workspace documentation
- demonstrate concrete DDD and layered-architecture violations
- show isolated single-rule violations and combined multi-rule violations

## Open In VS Code

Open:

```text
nmolecules-integrations/nmolecules-vscode/sample-violations/nmolecules-violations.code-workspace
```

The workspace is configured to target `Banking.Sample.Violations.sln` when you run:

- `nMolecules: Refresh Diagnostics`
- `nMolecules: Open Workspace Docs`

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
- [Using The Extension](docs/using-the-extension.md)
- [Rule Family Sample Index](../rule-family-sample-index.md)
