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

The sample is intentionally built to raise multiple nMolecules analyzer diagnostics, including:

- `XMoleculesValueObject0005`
- `XMoleculesValueObject0006`
- `XMoleculesValueObject1001`
- `XMoleculesValueObject1002`
- `XMoleculesAggregateRoot0004`
- `XMoleculesApplicationService0001`
- `XMoleculesFactory0001`
- `XMoleculesService0001`
- `XMoleculesCQRS0001`
- `XMoleculesOnion0005`
- `XMoleculesCrossStyle0001`
- `XMoleculesCrossStyle0003`
- `XMoleculesBoundedContext0003`
- `XMoleculesModule0004`
- `XMoleculesDomainEvent0002`
- `XMoleculesDomainEvent0006`
- `XMoleculesDomainEvent0007`
- `XMoleculesDomainEvent0009`

## Build The Sample

```powershell
dotnet build Banking.Sample.Violations.sln -v minimal
```

## Documentation

- [Architecture](docs/architecture.md)
- [Constellation Catalog](docs/constellation-catalog.md)
- [Expected Diagnostics](docs/expected-diagnostics.md)
- [Exact Diagnostic Details](docs/exact-diagnostic-details.md)
- [Using The Extension](docs/using-the-extension.md)
- [Rule Family Sample Index](../rule-family-sample-index.md)
