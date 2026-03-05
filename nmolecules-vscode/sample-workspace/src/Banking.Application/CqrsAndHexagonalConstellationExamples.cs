using Banking.Domain;
using NMolecules.Architecture.Cqrs;
using NMolecules.Architecture.Hexagonal;

namespace Banking.Application;

[Query]
public sealed class FindInvoiceBalance
{
    public FindInvoiceBalance(string invoiceNumber)
    {
        InvoiceNumber = invoiceNumber;
    }

    public string InvoiceNumber { get; }
}

[QueryModel]
public sealed class InvoiceBalanceView
{
    public InvoiceBalanceView(string invoiceNumber, decimal balance)
    {
        InvoiceNumber = invoiceNumber;
        Balance = balance;
    }

    public string InvoiceNumber { get; }
    public decimal Balance { get; }
}

public sealed class InvoiceReadHandlers
{
    [QueryHandler]
    public InvoiceBalanceView Handle(FindInvoiceBalance query)
    {
        return new InvoiceBalanceView(query.InvoiceNumber, 0m);
    }
}

[Command]
public sealed class SettleInvoice
{
    public SettleInvoice(string invoiceNumber, decimal amount, string currency)
    {
        InvoiceNumber = invoiceNumber;
        Amount = amount;
        Currency = currency;
    }

    public string InvoiceNumber { get; }
    public decimal Amount { get; }
    public string Currency { get; }
}

public sealed class InvoiceWriteHandlers
{
    [CommandHandler]
    public void Handle(SettleInvoice command)
    {
        _ = command;
    }
}

[Projection]
public sealed class InvoiceBalanceProjection
{
    public InvoiceBalanceView Apply(InvoicePaidEvent domainEvent)
    {
        return new InvoiceBalanceView(domainEvent.InvoiceNumber, domainEvent.Amount);
    }
}

public sealed class InvoiceDispatcher
{
    [CommandDispatcher]
    public void Dispatch(SettleInvoice command)
    {
        _ = command;
    }
}

[Application]
public sealed class InvoiceDecisionCore
{
    public bool CanSettle(decimal currentBalance, decimal amount)
    {
        return amount <= currentBalance;
    }
}

[PrimaryPort]
public interface IInvoiceSubmissionPort
{
    void Submit(SettleInvoice command);
}

[SecondaryPort]
public interface IInvoiceLedgerPort
{
    void Record(InvoicePaidEvent domainEvent);
}

[PrimaryAdapter]
public sealed class HttpInvoiceSubmissionAdapter : IInvoiceSubmissionPort
{
    public void Submit(SettleInvoice command)
    {
        _ = command;
    }
}

[SecondaryAdapter]
public sealed class InMemoryInvoiceLedgerAdapter : IInvoiceLedgerPort
{
    public void Record(InvoicePaidEvent domainEvent)
    {
        _ = domainEvent;
    }
}
