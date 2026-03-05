using NMolecules.Architecture.Layered;
using NMolecules.DDD;
using NMolecules.Events;

namespace Banking.Domain;

[DomainEvent]
public sealed class InvoicePaidEvent
{
    public InvoicePaidEvent(string invoiceNumber, decimal amount, string currency)
    {
        InvoiceNumber = invoiceNumber;
        Amount = amount;
        Currency = currency;
    }

    public string InvoiceNumber { get; }
    public decimal Amount { get; }
    public string Currency { get; }
}

[DomainLayer]
[AggregateRoot]
public sealed class InvoiceEventSource
{
    [Identity]
    public string InvoiceNumber { get; } = Guid.NewGuid().ToString("N");

    [DomainEventPublisher]
    public InvoicePaidEvent PublishPaid(Money payment)
    {
        return new InvoicePaidEvent(InvoiceNumber, payment.Amount, payment.Currency);
    }
}

public sealed class InvoiceEventHandlers
{
    [DomainEventHandler]
    public void Handle(InvoicePaidEvent domainEvent)
    {
        _ = domainEvent;
    }
}
