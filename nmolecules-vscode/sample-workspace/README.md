# nMolecules VS Code Sample Workspace

This workspace is the primary sample for the current nMolecules VS Code extension.

It demonstrates:

- a layered C# workspace with `Domain`, `Application`, `Infrastructure`, and `UserInterface`
- explicit use of `NMolecules.DDD` and `NMolecules.Architecture`
- local analyzer project references to the Roslyn analyzer project
- the current extension commands against a realistic multi-project workspace

## Projects

- `Banking.Domain`
- `Banking.Application`
- `Banking.Infrastructure`
- `Banking.Api`

## Open In VS Code

Open:

```text
nmolecules-integrations/nmolecules-vscode/sample-workspace/nmolecules-sample.code-workspace
```

Then run:

1. `nMolecules: Inspect Workspace`
2. `nMolecules: Open Workspace Docs`

## What The Extension Should Report

Expected current result:

- `1` solution
- `4` C# projects
- `0` analyzer package references
- `4` analyzer project references
- `4` nMolecules core references

Because the sample uses a single local-project strategy, the current recommendation should be the positive path:

- `The workspace already exposes nMolecules references. The next useful step is wiring diagnostics into the VS Code Problems view.`

## Build The Sample

```powershell
dotnet build Banking.Sample.sln -v minimal
```

## Documentation

- [Architecture](docs/architecture.md)
- [Layer Matrix](docs/layer-matrix.md)
- [Expected Inspection Output](docs/expected-inspection-output.md)
