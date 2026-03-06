using Cqrs = NMolecules.Architecture.Cqrs;

namespace Banking.Violations.CqrsOnly;

[Cqrs.Query]
public sealed class CqrsOnlyQuery
{
}

public sealed class CqrsOnlyHandlers
{
    [Cqrs.QueryHandler]
    public string Handle(CqrsOnlyQuery query)
    {
        _ = query;
        return string.Empty;
    }
}
