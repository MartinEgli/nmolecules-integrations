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
- `nMolecules: Open Rule Catalog`
- `nMolecules: Show Diagnostics Summary`

## Diagnostics Targeting

`nMolecules: Refresh Diagnostics` now resolves its build target in this order:

1. the active C# or F# editor file, preferring a project that explicitly includes that file
2. `nmolecules.diagnosticsTarget` as a fallback target
3. the shallowest solution or project discovered in the workspace

Each refresh writes the chosen target source into the `nMolecules` output channel. If no exact project include matches the active file, the output also records the fallback path that was used.

## Sample Workspace

A detailed sample workspace is included in:

- [sample-workspace/README.md](sample-workspace/README.md)
- [sample-violations/README.md](sample-violations/README.md)
- [rule-family-sample-index.md](rule-family-sample-index.md)

They cover both a healthy layered .NET solution and a deliberately broken workspace that drives analyzer findings into the Problems view.
The samples now include valid constellation catalogs, isolated single-rule failures, and combined multi-rule failures.

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

The host smoke path now exercises both the healthy sample workspace and the violations workspace.

## Packaging

From the integrations repo root:

```powershell
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release -SkipVisualStudioVsix
```

This produces:

- `artifacts/installers/vscode/nMolecules.VSCode.vsix`
- `artifacts/installers/vscode/setup/nMolecules.Setup.VSCode.exe`

Release tags for this channel use `vscode/*` (for example `vscode/v0.2.2`).
The full release gate is documented in [docs/release-checklist.md](docs/release-checklist.md).
Cross-channel rule-family checks are documented in [../docs/ide-channel-parity-checklist.md](../docs/ide-channel-parity-checklist.md).

## Current Guidance Surface

- workspace docs and architecture docs can be opened directly from the command palette
- rule catalog docs are directly discoverable through `Open Rule Catalog`
- diagnostics summary includes per-rule counts after each refresh
- diagnostics refresh now reports whether the chosen target came from an active-file match, a nearest-project fallback, an explicit fallback target, or a workspace scan
