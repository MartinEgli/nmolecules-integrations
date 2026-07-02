# Visual Studio Release Checklist

Status: June 29, 2026

This checklist defines the release gate for the Visual Studio delivery channel.

## Release Source of Truth

- `../eng/release-version.txt`
- current VSIX manifest under `../nmolecules-roslyn/src/nMolecules.Analyzers/nMolecules.Analyzers.Vsix/source.extension.vsixmanifest`

The release version file remains the only manually maintained version source.
The VSIX manifest version is synchronized from it through `tools/release/sync-release-version.ps1`.

## Scope

- host support: Visual Studio 2022 (`17.x`) and Visual Studio 2026 (`18.x`)
- editions: `Community`, `Professional`, `Enterprise`
- payload: analyzers and code fixes

## Static Validation

Run these checks before any VSIX release candidate:

```powershell
pwsh .\tools\release\validate-release-version.ps1
pwsh ..\tools\validate-ide-channel-hardening.ps1 -Configuration Release
pwsh .\nmolecules-roslyn\tools\validate-vsix-metadata.ps1
dotnet test .\nmolecules-roslyn\test\nMolecules.Analyzers.Test\nMolecules.Analyzers.Test.csproj -v minimal
pwsh .\tools\validate-rule-doc-sync.ps1
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release
```

Expected:

- release-version validation passes
- IDE channel hardening validation passes
- VSIX metadata validation passes
- analyzer suite is green
- rule-doc sync is green
- installer artifacts are generated under `artifacts/installers/visual-studio/*`

## Tag Discipline

Visual Studio release tags use:

- `vs/vX.Y.Z`

The `X.Y.Z` part must match `eng/release-version.txt`.
That check remains enforced in `.github/workflows/release-by-tag.yml`.

For cross-channel parity checks, use [../../docs/ide-channel-parity-checklist.md](../../docs/ide-channel-parity-checklist.md).

## Host Smoke Validation

Run once for `17.x` and once for `18.x` on a machine with Visual Studio installed:

1. Open Visual Studio Developer PowerShell for the target host.
2. Build the VSIX:

```powershell
MSBuild.exe .\nmolecules-roslyn\src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj /restore /p:Configuration=Release
```

3. Start the experimental instance:

```powershell
devenv.exe /rootsuffix Exp /log
```

4. Open a valid or invalid sample solution once the Visual Studio sample surfaces exist under `nmolecules-visualstudio`.
5. Confirm diagnostics in Error List.
6. Confirm at least one code fix is available for a fixable rule.
7. Verify no extension load errors in `ActivityLog.xml`.

## Migration Note

The Visual Studio packaging projects still physically live in `nmolecules-roslyn` during this slice.
This document is already canonical here so later project moves can happen without rewriting the release story again.
