# IDE Channel Parity Checklist

Status: March 2026

Use this checklist whenever a new analyzer family or rule family is introduced.

The goal is to keep the Visual Studio and VS Code delivery channels aligned enough that a new family is visible, explainable, and testable in both places.

## Required Parity Checks Per New Rule Family

1. Rule inventory is updated in the analyzer rule map and release catalog.
2. The family appears in sample coverage:
   `nmolecules-vscode/sample-workspace` for valid examples.
   `nmolecules-vscode/sample-violations` for invalid examples when diagnostics exist.
3. The VS Code channel can surface the family through:
   `nMolecules: Refresh Diagnostics`
   `nMolecules: Show Diagnostics Summary`
   workspace docs or rule-catalog navigation where applicable.
4. The Visual Studio channel can surface the same diagnostics through the solution-based analyzer path.
5. The packaging flow still emits the stable channel artifacts:
   `artifacts/installers/visual-studio/nMolecules.Analyzers.VisualStudio.vsix`
   `artifacts/installers/visual-studio/setup/install-visual-studio-extension.cmd`
   `artifacts/installers/vscode/nMolecules.VSCode.vsix`
   `artifacts/installers/vscode/setup/install-vscode-extension.cmd`
6. The setup executables still accept the documented command-line shape:
   `nMolecules.Setup.VisualStudio.exe [--vsix <path>] [--installer <path>] [--quiet]`
   `nMolecules.Setup.VSCode.exe [--vsix <path>] [--code <path>] [--no-force]`

## Smoke Sequence

Run this sequence before closing a new analyzer-family slice:

```powershell
npm install --prefix nmolecules-vscode
npm test --prefix nmolecules-vscode
npm run test:host --prefix nmolecules-vscode
pwsh .\tools\release\validate-release-version.ps1
pwsh .\tools\packaging\build-ide-installers.ps1 -Configuration Release
```

Then verify manually:

1. VS Code on `nmolecules-vscode/sample-workspace/nmolecules-sample.code-workspace`
2. VS Code on `nmolecules-vscode/sample-violations/nmolecules-violations.code-workspace`
3. Visual Studio on `nmolecules-vscode/sample-workspace/Banking.Sample.sln`
4. Visual Studio on `nmolecules-vscode/sample-violations/Banking.Sample.Violations.sln`

## Documentation Links

- Visual Studio release gate: `nmolecules-roslyn/docs/visual-studio-release-checklist.md`
- VS Code release gate: `nmolecules-vscode/docs/release-checklist.md`
