# nMolecules VS Code Extension

This project is the first Visual Studio Code entry point for the nMolecules workspace.

Current scope:

- inspect the current C# workspace for nMolecules package and project references
- surface the current state in a dedicated output channel
- provide a stable command entry point for opening workspace documentation

This is intentionally a thin extension project.
The long-term direction is to reuse the same analyzer rule set that already exists in `nmolecules-roslyn`.

## Commands

- `nMolecules: Inspect Workspace`
- `nMolecules: Open Workspace Docs`

## Sample Workspace

A detailed sample workspace is included in:

- [sample-workspace/README.md](sample-workspace/README.md)

It contains a layered .NET solution and the documentation files that the current extension knows how to open.

## Tests

Run:

```powershell
npm test --prefix nmolecules-vscode
```

## Next Steps

- connect the extension to Roslyn-based diagnostics for C# workspaces
- surface rule help and diagnostics in VS Code Problems
- add marketplace packaging and publishing automation
