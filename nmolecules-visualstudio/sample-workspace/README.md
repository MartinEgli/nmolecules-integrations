# Visual Studio Sample Workspace

Status: March 7, 2026

This folder contains the healthy Visual Studio sample solution for the current
integration surface.

Purpose:

- validate extension load and analyzer activation inside Visual Studio
- provide a documented Error List baseline with no nMolecules diagnostics
- provide the Visual Studio counterpart to `nmolecules-vscode/sample-workspace`
- avoid duplicating the banking sample code by pointing at the shared project files

## Open In Visual Studio

Open:

```text
nmolecules-integrations/nmolecules-visualstudio/sample-workspace/Banking.VisualStudio.Sample.sln
```

Then confirm:

1. the solution loads without missing project references
2. the four shared banking projects restore successfully
3. the Error List shows no `XMolecules*` diagnostics
4. the shared analyzer project references are active in all four projects

## Projects

- `Banking.Domain`
- `Banking.Application`
- `Banking.Infrastructure`
- `Banking.Api`

These are the same sample projects used by the VS Code channel. The Visual
Studio solution is a host-specific wrapper around the shared sample code so both
IDE channels stay aligned.

## Build The Sample

```powershell
dotnet build Banking.VisualStudio.Sample.sln -v minimal
```

## Documentation

- [Expected Error List](docs/expected-error-list.md)
- [Solution Layout](docs/solution-layout.md)
