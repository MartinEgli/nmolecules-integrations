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

When a C# or F# file is active, diagnostics refresh prefers a project that explicitly includes that file. If no active-file match is found, the workspace falls back to the configured `Banking.Sample.Violations.sln` target.

Check the `nMolecules` output channel after each refresh:

- `nMolecules target source: active-file exact include.` means the current file selected the build target directly.
- `nMolecules target source: active-file nearest project.` means no exact include matched, so the extension used the nearest project.
- `nMolecules target source: configured target.` means the workspace fallback setting was used.
- `nMolecules target source: workspace fallback.` means the extension scanned the workspace because neither an active-file target nor a configured fallback target resolved.
