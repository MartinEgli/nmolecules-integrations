# Sample Layer Matrix

## Project To Layer

| Project | Layer | Reason |
|---|---|---|
| `Banking.Api` | `UserInterface` | External entry point. |
| `Banking.Application` | `Application` | Use-case orchestration. |
| `Banking.Domain` | `Domain` | Domain model and abstractions. |
| `Banking.Infrastructure` | `Infrastructure` | Technical implementation of repository concerns. |

## Intended Dependencies

| From | To | Status |
|---|---|---|
| `Banking.Api` | `Banking.Application` | allowed |
| `Banking.Application` | `Banking.Domain` | allowed |
| `Banking.Infrastructure` | `Banking.Domain` | allowed |
| `Banking.Domain` | `Banking.Application` | forbidden |
| `Banking.Domain` | `Banking.Api` | forbidden |
| `Banking.Domain` | `Banking.Infrastructure` | forbidden |

## Extension-Facing Outcome

The sample is intentionally clean:

- no mixed analyzer reference strategy
- no missing core references
- one solution file for onboarding

That makes it a good baseline sample for the current extension stage.
