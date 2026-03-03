# Sample Architecture

This sample uses a conventional layered DDD structure:

1. `Banking.Api`
2. `Banking.Application`
3. `Banking.Domain`
4. `Banking.Infrastructure`

The main intent is to give the VS Code extension a realistic workspace to inspect.

## Layer Intent

### `Banking.Api`

- user-interface boundary
- host-facing request translation
- delegates to the application layer

### `Banking.Application`

- use-case orchestration
- depends on domain abstractions
- contains the `TransferMoneyUseCase`

### `Banking.Domain`

- aggregate root `BankAccount`
- value object `Money`
- repository abstraction `IAccounts`
- domain service `TransferPolicy`
- factory `BankAccountFactory`

### `Banking.Infrastructure`

- in-memory repository implementation
- technical persistence concern for the sample

## Current Extension Relevance

The current VS Code extension does not yet surface Roslyn diagnostics in Problems.
It does, however, inspect this workspace and should recognize:

- all four projects as C# projects
- local analyzer project references
- local nMolecules core project references
- the sample docs folder for the `Open Workspace Docs` command
