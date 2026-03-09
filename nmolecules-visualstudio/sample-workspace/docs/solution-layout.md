# Solution Layout

`Banking.VisualStudio.Sample.sln` is intentionally a thin Visual Studio wrapper
around the shared banking sample projects used by the VS Code channel.

Included projects:

- `Banking.Domain`
- `Banking.Application`
- `Banking.Infrastructure`
- `Banking.Api`

Why this layout exists:

- it gives Visual Studio its own stable `.sln` entry point
- it keeps both IDE channels on the same code corpus
- it avoids duplicated sample code and divergent rule coverage
