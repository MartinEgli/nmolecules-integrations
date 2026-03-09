# Using The Visual Studio Host

Suggested Visual Studio smoke sequence:

1. Open `Banking.VisualStudio.Violations.sln`.
2. Wait for restore and background analysis to complete.
3. Open Error List and filter for `XMolecules`.
4. Navigate from one diagnostic into the offending symbol.
5. If a code fix exists for the selected diagnostic family, open Quick Actions
   and verify that the host surfaces it correctly.

Recommended first inspection points:

- `Banking.Violations.RuleMatrix`
- `Banking.Violations.MetadataMissing`
- `Banking.Violations.CqrsOnly`
