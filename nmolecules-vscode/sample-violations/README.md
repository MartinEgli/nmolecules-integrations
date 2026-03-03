# nMolecules VS Code Violations Workspace

This workspace is the deliberately broken companion to the main sample.

Use it to validate that the VS Code extension can:

- refresh nMolecules diagnostics into the Problems view
- point developers from diagnostics to the workspace documentation
- demonstrate concrete DDD and layered-architecture violations

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

## Build The Sample

```powershell
dotnet build Banking.Sample.Violations.sln -v minimal
```

## Documentation

- [Architecture](docs/architecture.md)
- [Expected Diagnostics](docs/expected-diagnostics.md)
- [Using The Extension](docs/using-the-extension.md)
