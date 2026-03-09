# Expected Error List

Open `Banking.VisualStudio.Sample.sln` in Visual Studio and build once.

Expected result:

- the solution loads successfully
- restore succeeds for all four projects
- the Error List shows no `XMolecules*` diagnostics
- the analyzer references remain active because the shared sample projects already
  reference the Roslyn analyzer project

If you see unexpected diagnostics, check:

- the solution was opened from `nmolecules-visualstudio/sample-workspace`
- the shared projects under `nmolecules-vscode/sample-workspace/src` were not modified
- Visual Studio completed design-time restore before you inspected Error List
