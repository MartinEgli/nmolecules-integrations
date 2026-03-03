# nMolecules VS Code Extension

This project is the first Visual Studio Code entry point for the nMolecules workspace.

Current scope:

- inspect the current C# workspace for nMolecules package and project references
- surface the current state in a dedicated output channel
- refresh Roslyn analyzer diagnostics into the VS Code Problems view
- provide a stable command entry point for opening workspace documentation

This is intentionally a thin extension project.
The long-term direction is to reuse the same analyzer rule set that already exists in `nmolecules-roslyn`.

## Commands

- `nMolecules: Inspect Workspace`
- `nMolecules: Refresh Diagnostics`
- `nMolecules: Open Workspace Docs`

## Sample Workspace

A detailed sample workspace is included in:

- [sample-workspace/README.md](sample-workspace/README.md)
- [sample-violations/README.md](sample-violations/README.md)

They cover both a healthy layered .NET solution and a deliberately broken workspace that drives analyzer findings into the Problems view.

## Tests

Run:

```powershell
npm test --prefix nmolecules-vscode
```

For the VS Code extension host path:

```powershell
npm install --prefix nmolecules-vscode
npm run test:host --prefix nmolecules-vscode
```

## Next Steps

- surface rule help and diagnostics navigation in VS Code Problems
- add marketplace packaging and publishing automation
