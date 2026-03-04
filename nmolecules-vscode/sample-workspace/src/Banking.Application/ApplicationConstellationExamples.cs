using Banking.Domain;
using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Application;

[ApplicationLayer]
[ApplicationService]
public sealed class InvoiceSettlementUseCase
{
    private readonly IInvoices invoices;
    private readonly InvoiceSettlementPolicy policy;

    public InvoiceSettlementUseCase(IInvoices invoices, InvoiceSettlementPolicy policy)
    {
        this.invoices = invoices;
        this.policy = policy;
    }

    public bool Settle(string invoiceNumber, Money payment)
    {
        var invoice = invoices.Get(invoiceNumber);

        if (!policy.CanSettle(invoice, payment))
        {
            return false;
        }

        invoice.RegisterPayment(payment);
        invoices.Save(invoice);
        return true;
    }
}
