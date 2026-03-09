# Expected Error List

Open `Banking.VisualStudio.Violations.sln` in Visual Studio and build once.

Expected result:

- the solution loads successfully
- the Error List shows multiple `XMolecules*` diagnostics
- violations are visible from at least these families:
  - Bricks
  - CQRS
  - Layered
  - DDD
  - metadata consistency and completeness

Key projects to inspect first:

- `Banking.Violations.RuleMatrix`
- `Banking.Violations.Domain`
- `Banking.Violations.Application`

The exact full rule set is maintained by the shared violations corpus in
`nmolecules-vscode/sample-violations`.
