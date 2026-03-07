# VS Code Release Checklist

Status: March 7, 2026

This checklist defines the release gate for the VS Code delivery path.

## Release Source of Truth

- `eng/release-version.txt`
- `package.json`
- `package-lock.json`

The first file is manually maintained.
The package files are synchronized from it via `tools/release/sync-release-version.ps1`.

## Static Validation (Always)

Run these checks before any VS Code release candidate:

```powershell
pwsh .\tools\release\validate-release-version.ps1
npm test --prefix nmolecules-vscode
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release -SkipAnalyzerTests -SkipVsCodeTests -SkipVisualStudioVsix
```

Expected:

- release-version validation passes
- VS Code extension tests are green
- `artifacts/installers/vscode/nMolecules.VSCode.vsix` is generated
- `artifacts/installers/vscode/setup/nMolecules.Setup.VSCode.exe` is generated

## Tag Discipline

VS Code release tags must use:

- `vscode/vX.Y.Z`

The `X.Y.Z` part must match `eng/release-version.txt`.
This is enforced in `.github/workflows/release-by-tag.yml`.

## Manual Smoke Validation

1. Install the packaged extension in a clean VS Code profile.
2. Open the sample violation workspace.
3. Run `nMolecules: Refresh Diagnostics`.
4. Confirm the Problems view is populated and the `nMolecules` output channel reports the selected target.
5. Confirm workspace documentation and rule-catalog commands still open the expected files.
