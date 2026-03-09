# nMolecules—Technology integrations

[![.NET](https://github.com/xmolecules/nmolecules-integrations/actions/workflows/buildAndTest.yml/badge.svg)](https://github.com/xmolecules/nmolecules-integrations/actions/workflows/buildAndTest.yml)

## Working Documents

For the current analyzer expansion work, see:

- [nmolecules-roslyn/docs/mvp-rule-catalog.md](nmolecules-roslyn/docs/mvp-rule-catalog.md)
- [nmolecules-roslyn/docs/service-role-matrix.md](nmolecules-roslyn/docs/service-role-matrix.md)
- [nmolecules-roslyn/docs/release-tracking-investigation.md](nmolecules-roslyn/docs/release-tracking-investigation.md)
- [nmolecules-visualstudio/docs/visual-studio-support.md](nmolecules-visualstudio/docs/visual-studio-support.md)

Current baseline highlights:

- analyzer rule catalog is synchronized through `XMoleculesModule0007` and `XMoleculesBoundedContext0009`
- analyzer quality and rule-doc sync gates are part of the default validation workflow
- release packaging now uses repo-local `eng/release-version.txt` plus sync/validate scripts
- VSIX and VS Code packaging are delivered from one shared installer script
- IDE channel parity is tracked through a shared checklist and VS Code host smoke coverage for both healthy and violating workspaces
- a dedicated `nmolecules-visualstudio` product surface now exists as the migration target for Visual Studio host concerns

## Product Surfaces

- [nmolecules-roslyn](nmolecules-roslyn): analyzer core, code fixes, and shared analyzer tests
- [nmolecules-vscode](nmolecules-vscode/README.md): VS Code host, packaging, samples, and docs
- [nmolecules-visualstudio](nmolecules-visualstudio/README.md): Visual Studio host, packaging, release, and sample surface

## Visual Studio

The Visual Studio delivery surface is now documented under:

- [nmolecules-visualstudio/README.md](nmolecules-visualstudio/README.md)
- [nmolecules-visualstudio/docs/visual-studio-support.md](nmolecules-visualstudio/docs/visual-studio-support.md)
- [nmolecules-visualstudio/docs/release-checklist.md](nmolecules-visualstudio/docs/release-checklist.md)
- [nmolecules-visualstudio/sample-workspace/Banking.VisualStudio.Sample.sln](nmolecules-visualstudio/sample-workspace/Banking.VisualStudio.Sample.sln)
- [nmolecules-visualstudio/sample-violations/Banking.VisualStudio.Violations.sln](nmolecules-visualstudio/sample-violations/Banking.VisualStudio.Violations.sln)

Current migration note:

- the canonical Visual Studio docs live under `nmolecules-visualstudio`
- dedicated Visual Studio sample solutions now live under `nmolecules-visualstudio`
- the physical VSIX project still lives under `nmolecules-roslyn` in this first migration slice

## VS Code Extension

The workspace contains the current Visual Studio Code delivery channel:

- [nmolecules-vscode/README.md](nmolecules-vscode/README.md)

Current scope:

- inspect C# workspaces for nMolecules package and project references
- open the relevant workspace documentation from within VS Code
- refresh Roslyn analyzer diagnostics into VS Code
- provide a stable starting point for cross-channel rule-family delivery

## Installer Build

The workspace now provides one packaging entry point for IDE delivery:

```powershell
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release
```

Local fallback (without Visual Studio MSBuild or VS Code packaging):

```powershell
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release -SkipVisualStudioVsix -SkipVsCodePackage
```

What it builds:

- Visual Studio extension package (`.vsix`) and a setup executable (`nMolecules.Setup.VisualStudio.exe`)
- VS Code extension package (`.vsix`) and a setup executable (`nMolecules.Setup.VSCode.exe`)
- helper install scripts in the setup folders (`install-visual-studio-extension.cmd`, `install-vscode-extension.cmd`)

Before release packaging, validate version alignment:

```powershell
pwsh .\tools\release\validate-release-version.ps1
```

Output path:

- `artifacts/installers/visual-studio/setup`
- `artifacts/installers/vscode/setup`

Installer note:

- `nMolecules.Setup.VSCode.exe` first checks its own folder, then common sibling artifact folders such as `..\nMolecules.VSCode.vsix` and `artifacts/installers/vscode/**`
- use `--vsix <path>` when the VS Code package is stored elsewhere

## CI Artifact Split

The CI pipeline publishes installer artifacts per IDE channel:

- `visual-studio-installers` from `artifacts/installers/visual-studio/**`
- `vscode-installers` from `artifacts/installers/vscode/**`

The tag-based release workflow is split the same way:

- `vs/*` tags trigger Visual Studio release artifact builds only
- `vscode/*` tags trigger VS Code release artifact builds only

## Tag Convention

Use channel-prefixed tags:

- `vs/v<semver>` for Visual Studio releases (example: `vs/v1.8.0`)
- `vscode/v<semver>` for VS Code releases (example: `vscode/v1.8.0`)

Helper script:

```powershell
pwsh .\tools\release\new-ide-tag.ps1 -Channel vs -Version 1.8.0 -Push
pwsh .\tools\release\new-ide-tag.ps1 -Channel vscode -Version 1.8.0 -Push
```
