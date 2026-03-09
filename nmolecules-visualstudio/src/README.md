# Visual Studio Source Surface

Status: March 7, 2026

This folder is reserved for Visual Studio-specific source projects.

Target contents for later slices:

- `nMolecules.VisualStudio.Vsix`
- `nMolecules.VisualStudio.Setup`
- optional Visual Studio-specific smoke helpers

Current migration note:

- the active VSIX project still lives in `../nmolecules-roslyn/src/nMolecules.Analyzers/nMolecules.Analyzers.Vsix`
- this folder exists now so packaging and host code can move here without changing the intended repo shape again
