# Using The Extension

Open `nmolecules-violations.code-workspace` in Visual Studio Code.

Then use the extension in this order:

1. Run `nMolecules: Refresh Diagnostics`.
2. Run `nMolecules: Show Diagnostics Summary` to see per-rule counts in the output channel.
3. Open the Problems view and inspect the emitted `nMolecules` diagnostics.
4. Start with the isolated examples from [Constellation Catalog](constellation-catalog.md).
5. Then compare the exact emitted text with [Exact Diagnostic Details](exact-diagnostic-details.md).
6. Run `nMolecules: Open Rule Catalog` to jump directly into analyzer rule documentation.
7. Run `nMolecules: Open Workspace Docs` to jump back into sample docs.
8. Use `nMolecules: Inspect Workspace` if you also want the wiring summary in the output channel.

The workspace is configured so that diagnostics target `Banking.Sample.Violations.sln` automatically on activation.
