# Visual Studio Violations Workspace

Status: March 7, 2026

This folder contains the deliberately broken Visual Studio companion solution.

Purpose:

- validate `XMolecules*` diagnostics in Error List
- validate at least one code fix or quick action in the Visual Studio host
- provide the Visual Studio counterpart to `nmolecules-vscode/sample-violations`
- keep rule coverage aligned with the shared banking violations projects

## Open In Visual Studio

Open:

```text
nmolecules-integrations/nmolecules-visualstudio/sample-violations/Banking.VisualStudio.Violations.sln
```

Then confirm:

1. the solution loads with all shared project references intact
2. the Error List shows multiple `XMolecules*` diagnostics
3. at least one diagnostic from the current rule family under development is visible
4. the rule-matrix and metadata helper projects appear in Solution Explorer

## Projects

- `Banking.Violations.Domain`
- `Banking.Violations.Application`
- `Banking.Violations.Infrastructure`
- `Banking.Violations.CqrsOnly`
- `Banking.Violations.MetadataMissing`
- `Banking.Violations.MetadataConsistency`
- `Banking.Violations.RuleMatrix`

These projects are shared with the VS Code channel. The Visual Studio solution
is intentionally a host-specific wrapper so both IDE channels exercise the same
violation corpus.

## Build The Sample

```powershell
dotnet build Banking.VisualStudio.Violations.sln -v minimal
```

## Documentation

- [Expected Error List](docs/expected-error-list.md)
- [Using The Visual Studio Host](docs/using-the-host.md)
