# Visual Studio Release Checklist

Status: March 2026

This checklist defines the release gate for the Visual Studio (VSIX) delivery path.

## Scope

- host support: Visual Studio 2022 (`17.x`) and Visual Studio 2026 (`18.x`)
- editions: `Community`, `Professional`, `Enterprise`
- payload: analyzers + code-fixes only

## Static Validation (Always)

Run these checks before any VSIX release candidate:

```powershell
pwsh .\tools\validate-vsix-metadata.ps1
dotnet test .\test\nMolecules.Analyzers.Test\nMolecules.Analyzers.Test.csproj -v minimal
```

Expected:

- VSIX manifest and project metadata validation passes
- analyzer suite is green

## Host Smoke Validation (Per VS Major)

Run once for `17.x` and once for `18.x` on a machine with Visual Studio installed:

1. Open Visual Studio Developer PowerShell for the target host.
2. Build the VSIX:
```powershell
MSBuild.exe src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj /restore /p:Configuration=Release
```
3. Start experimental instance:
```powershell
devenv.exe /rootsuffix Exp /log
```
4. Open an invalid sample solution and confirm diagnostics in Error List.
5. Confirm at least one code-fix is available (`XMoleculesService0001` or value-object fixes).
6. Verify no extension load errors in ActivityLog.

## Product Decision (4.x)

Visual Studio delivery remains analyzer-first in the 4.x line.

- in scope: diagnostics and safe code-fixes
- out of scope: custom tool windows, custom designers, custom command UX
