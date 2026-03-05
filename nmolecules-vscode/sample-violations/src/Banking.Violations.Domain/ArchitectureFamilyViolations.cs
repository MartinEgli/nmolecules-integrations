using NMolecules.Architecture.Cqrs;
using NMolecules.Architecture.Layered;
using NMolecules.Architecture.Onion.Classic;
using NMolecules.Architecture.Onion.Simplified;
using NMolecules.DDD;
using NMolecules.Events;

[assembly: BoundedContext(Id = "Billing", Name = "Billing")]
[assembly: Module(Id = "Accounts.Core", Name = "Accounts", BoundedContextId = "Billing")]
[module: BoundedContext(Id = "Sales", Name = "Sales")]
[module: Module(Id = "Accounts.Api", Name = "Accounts", BoundedContextId = "Billing")]
[module: Module(Id = "Accounts", Name = "Accounts", BoundedContextId = "UnknownContext")]

namespace Banking.Violations.Domain;

[DomainLayer]
public sealed class LayeredCore
{
}

[DomainModelRing]
public sealed class ClassicDomainRing
{
}

[DomainRing]
public sealed class SimplifiedDomainRing
{
}

[Query]
public sealed class MissingQueryHandlerQuery
{
}

[DomainEvent]
public sealed class EventLeakingAggregate
{
    public EventLeakingAggregate(MultiIdentityAccount account)
    {
        Account = account;
    }

    public MultiIdentityAccount Account { get; }
}

[Factory]
[DomainEventPublisher]
public sealed class ForbiddenEventPublisherFactory
{
}

public sealed class BrokenEventHandlers
{
    [DomainEvent]
    public sealed class PaymentPosted
    {
    }

    [DomainEvent]
    public sealed class PaymentCancelled
    {
    }

    [DomainEventHandler]
    public void HandleWithoutPayload()
    {
    }

    [DomainEventHandler]
    public void HandleTooMany(PaymentPosted posted, PaymentCancelled cancelled)
    {
        _ = posted;
        _ = cancelled;
    }
}
