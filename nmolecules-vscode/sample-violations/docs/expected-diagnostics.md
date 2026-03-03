# Expected Diagnostics

The broken sample is designed to populate the VS Code Problems view with nMolecules diagnostics.

The exact line numbers can change as the sample evolves, but the current expected rule set is:

- `XMoleculesValueObject0005` for a mutable value object
- `XMoleculesValueObject0006` for an identity member inside a value object
- `XMoleculesValueObject1001` for a value object that does not implement `IEquatable<T>`
- `XMoleculesValueObject1002` for a value object class that is not `sealed`
- `XMoleculesAggregateRoot0004` for an aggregate root with more than one identity member
- `XMoleculesApplicationService0001` for an application service that is also marked as a domain building block
- `XMoleculesFactory0001` for a factory depending on an application service
- `XMoleculesService0001` for a legacy `[Service]` marker
