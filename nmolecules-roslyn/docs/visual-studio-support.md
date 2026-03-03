# Visual Studio Support

Status: March 3, 2026

## Supported Host Matrix

The current VSIX manifest targets these Visual Studio products:

- Visual Studio 2022 `17.x` on `amd64`
- Visual Studio 2026 `18.x` on `amd64`
- Editions: `Community`, `Professional`, `Enterprise`

The manifest is defined in `src/nMolecules.Analyzers/nMolecules.Analyzers.Vsix/source.extension.vsixmanifest`.

## Build Toolchain

The VSIX project is a classic Visual Studio extension project that still targets `.NET Framework 4.7.2`.
That is intentional for compatibility with the Visual Studio host.

Current toolchain decisions:

- package tooling: `Microsoft.VSSDK.BuildTools 17.14.2120`
- debug root suffix: `Exp`
- analyzer payloads: `nMolecules.Analyzers` and `nMolecules.Analyzers.CodeFixes`

## Build Requirement

The VSIX project should be built with Visual Studio MSBuild, not with plain `dotnet build`.

Typical build entry points:

```powershell
MSBuild.exe src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj /restore /p:Configuration=Debug
```

or from a Visual Studio Developer PowerShell:

```powershell
devenv.exe src\nMolecules.Analyzers\nMolecules.Analyzers.Vsix\nMolecules.Analyzers.Vsix.csproj
```

`dotnet build` is still useful for the analyzer libraries and tests, but not a reliable verification path for the VSIX packaging project because the classic VSSDK tasks require the Visual Studio build environment.

## Smoke Test Checklist

Run the following checklist once for Visual Studio 2022 and once for Visual Studio 2026.

1. Open the matching Visual Studio Developer PowerShell.
2. Build the VSIX project with `MSBuild.exe`.
3. Start an experimental instance:

```powershell
devenv.exe /rootsuffix Exp /log
```

4. Open the workspace sample or analyzer test solution.
5. Verify that the extension loads without startup errors.
6. Confirm that diagnostics appear in the Error List for known invalid models.
7. Confirm that at least one code fix is offered for a known fixable rule.
8. Close Visual Studio and inspect `%APPDATA%\Microsoft\VisualStudio\<instance>\ActivityLog.xml` if the extension did not load cleanly.

## Current Local Limitation

On the current workstation used for this workspace update, `msbuild`, `devenv`, and `vswhere` were not available on the `PATH`.
That means the manifest and project were modernized, but the VSIX packaging build itself was not yet locally validated against an installed Visual Studio instance.
