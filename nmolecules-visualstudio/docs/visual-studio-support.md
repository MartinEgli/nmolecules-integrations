# Visual Studio Support

Status: March 7, 2026

This document is the canonical Visual Studio host guide for `nmolecules-integrations`.

## Supported Host Matrix

The current VSIX manifest targets these Visual Studio products:

- Visual Studio 2022 `17.x` on `amd64`
- Visual Studio 2026 `18.x` on `amd64`
- editions: `Community`, `Professional`, `Enterprise`

The current manifest is still defined in:

- `../nmolecules-roslyn/src/nMolecules.Analyzers/nMolecules.Analyzers.Vsix/source.extension.vsixmanifest`

That location is transitional and will move into `nmolecules-visualstudio/src` in a later migration slice.

## Current Build Toolchain

The current VSIX project is a classic Visual Studio extension project that targets `.NET Framework 4.7.2`.
That remains intentional for Visual Studio host compatibility.

Current toolchain decisions:

- package tooling: `Microsoft.VSSDK.BuildTools 17.14.2120`
- debug root suffix: `Exp`
- analyzer payloads: `nMolecules.Analyzers` and `nMolecules.Analyzers.CodeFixes`

## Current Build Requirement

The VSIX project should be built with Visual Studio MSBuild, not with plain `dotnet build`.

Typical build entry points:

```powershell
MSBuild.exe ..\nmolecules-roslyn\src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj /restore /p:Configuration=Debug
```

or from a Visual Studio Developer PowerShell:

```powershell
devenv.exe ..\nmolecules-roslyn\src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj
```

`dotnet build` remains the right path for analyzer libraries and tests, but not for the classic VSIX host packaging project.

## Static Validation Gate

Before host smoke tests, run:

```powershell
pwsh ..\nmolecules-roslyn\tools\validate-vsix-metadata.ps1
dotnet test ..\nmolecules-roslyn\test\nMolecules.Analyzers.Test\nMolecules.Analyzers.Test.csproj -v minimal
```

For release packaging, use the shared installer flow from the integrations repo root:

```powershell
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release
```

This still creates the Visual Studio `.vsix` package and setup executable under `artifacts/installers/visual-studio/setup`.

## Smoke Test Checklist

Run the following checklist once for Visual Studio 2022 and once for Visual Studio 2026.

1. Open the matching Visual Studio Developer PowerShell.
2. Build the VSIX project with `MSBuild.exe`.
3. Start an experimental instance:

```powershell
devenv.exe /rootsuffix Exp /log
```

4. Open one of the Visual Studio sample solutions:

```text
nmolecules-visualstudio/sample-workspace/Banking.VisualStudio.Sample.sln
nmolecules-visualstudio/sample-violations/Banking.VisualStudio.Violations.sln
```
5. Verify that the extension loads without startup errors.
6. Confirm that diagnostics appear in the Error List for known invalid models.
7. Confirm that at least one code fix is offered for a known fixable rule.
8. Inspect `%APPDATA%\Microsoft\VisualStudio\<instance>\ActivityLog.xml` if the extension did not load cleanly.

The release gate is documented in [release-checklist.md](release-checklist.md).

## Product Decision

Visual Studio delivery is now treated as its own product surface.

- in scope: diagnostics, code fixes, packaging, install flow, Visual Studio host validation
- out of scope for the current slice: separate tool windows, designers, or non-analyzer UX
