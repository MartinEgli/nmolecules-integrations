# nMolecules Visual Studio

Status: March 7, 2026

This folder is the dedicated Visual Studio product surface for the `nmolecules-integrations` workspace.

It exists to separate:

- the Roslyn analyzer core in `nmolecules-roslyn`
- the Visual Studio host, packaging, release, and smoke-test concerns

## Current Migration State

This is the first non-destructive migration slice.

Current position:

- the canonical Visual Studio product documentation now lives here
- dedicated Visual Studio sample and violations solutions now live here
- the actual VSIX project still lives under `nmolecules-roslyn`
- packaging and CI still build the current VSIX from the Roslyn subtree
- future Visual Studio-specific samples, host smokes, and setup projects should be created here

## Planned Structure

```text
nmolecules-visualstudio/
|-- README.md
|-- docs/
|-- sample-workspace/
|-- sample-violations/
`-- src/
```

## Scope

This surface should own:

- Visual Studio host guidance
- Visual Studio release checklist
- Visual Studio-specific packaging and installer documentation
- Visual Studio-specific sample solutions
- Visual Studio-specific smoke or host automation

The Roslyn analyzer core remains in `../nmolecules-roslyn`.

## Current Entry Points

- [docs/visual-studio-support.md](docs/visual-studio-support.md)
- [docs/release-checklist.md](docs/release-checklist.md)
- [sample-workspace/Banking.VisualStudio.Sample.sln](sample-workspace/Banking.VisualStudio.Sample.sln)
- [sample-violations/Banking.VisualStudio.Violations.sln](sample-violations/Banking.VisualStudio.Violations.sln)
- [src/README.md](src/README.md)
- [sample-workspace/README.md](sample-workspace/README.md)
- [sample-violations/README.md](sample-violations/README.md)
