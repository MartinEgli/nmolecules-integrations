# Using The Extension

Open `nmolecules-violations.code-workspace` in Visual Studio Code.

Then use the extension in this order:

1. Run `nMolecules: Refresh Diagnostics`.
2. Open the Problems view and inspect the emitted `nMolecules` diagnostics.
3. Run `nMolecules: Open Workspace Docs` to jump back into the sample documentation.
4. Use `nMolecules: Inspect Workspace` if you also want the wiring summary in the output channel.

The workspace is configured so that diagnostics target `Banking.Sample.Violations.sln` automatically on activation.
