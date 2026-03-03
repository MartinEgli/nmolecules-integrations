# Expected Inspection Output

Running `nMolecules: Inspect Workspace` in this sample should produce a report equivalent to:

```text
nMolecules workspace inspection

Solutions: 1
Projects: 4
Projects using analyzer package refs: 0
Projects using analyzer project refs: 4
Projects using nMolecules core refs: 4

Recommendations:
- The workspace already exposes nMolecules references. The next useful step is wiring diagnostics into the VS Code Problems view.
```

If the output differs, the most likely causes are:

- the workspace was not opened at `sample-workspace`
- one of the project references was changed
- the extension command was executed from a different folder
