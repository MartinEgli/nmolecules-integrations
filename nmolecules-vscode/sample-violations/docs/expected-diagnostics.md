# Expected Diagnostics

The violation workspace is now configured to trigger every currently implemented analyzer rule at least once across the solution.

## Full Rule-Coverage Baseline

- `XMoleculesEntity0001` to `XMoleculesEntity0008`
- `XMoleculesAggregateRoot0001` to `XMoleculesAggregateRoot0008`
- `XMoleculesValueObject0001` to `XMoleculesValueObject0009`
- `XMoleculesValueObject1001` to `XMoleculesValueObject1002`
- `XMoleculesRepository0001` to `XMoleculesRepository0007`
- `XMoleculesFactory0001` to `XMoleculesFactory0004`
- `XMoleculesDomainService0001` to `XMoleculesDomainService0004`
- `XMoleculesApplicationService0001` to `XMoleculesApplicationService0004`
- `XMoleculesIdentity0001`
- `XMoleculesBoundedContext0001` to `XMoleculesBoundedContext0009`
- `XMoleculesModule0001` to `XMoleculesModule0007`
- `XMoleculesDomainEvent0001` to `XMoleculesDomainEvent0009`
- `XMoleculesLayered0001` to `XMoleculesLayered0006`
- `XMoleculesOnion0001` to `XMoleculesOnion0005`
- `XMoleculesHexagonal0001` to `XMoleculesHexagonal0005`
- `XMoleculesCQRS0001` to `XMoleculesCQRS0006`
- `XMoleculesEventStorming0001` to `XMoleculesEventStorming0005`
- `XMoleculesMicroservices0001` to `XMoleculesMicroservices0006`
- `XMoleculesCrossStyle0001` to `XMoleculesCrossStyle0003`
- `XMoleculesBricks0001` to `XMoleculesBricks0002`
- `XMoleculesService0001`

## Rule-Matrix Projects

- `src/Banking.Violations.RuleMatrix`
  Covers the remaining DDD, Events, Layered, Onion, Hexagonal, CQRS, EventStorming, Microservices, and Bricks gaps.

- `src/Banking.Violations.MetadataMissing`
  Covers metadata-completeness violations (`BoundedContext0001/0002`, `Module0001/0002/0003`).

- `src/Banking.Violations.MetadataConsistency`
  Covers metadata-consistency violations (`BoundedContext0004`, `Module0005/0006`).

- `src/Banking.Violations.CqrsOnly`
  Covers `XMoleculesCrossStyle0002` in a CQRS-only compilation without a primary structural style marker.

## Verification

Run `tools/validate-violations-rule-coverage.ps1` in `sample-violations`.  
Expected output:

- `Violation workspace rule IDs: 111`
- `Analyzer code rule IDs:       111`
- `Missing IDs:                  0`
