using Banking.Domain;
using NMolecules.Architecture.Layered;

namespace Banking.Infrastructure;

[InfrastructureLayer]
public sealed class InMemoryInvoices : IInvoices
{
    private readonly Dictionary<string, Invoice> invoices = new();

    public Invoice Get(string invoiceNumber)
    {
        return invoices[invoiceNumber];
    }

    public void Save(Invoice invoice)
    {
        invoices[invoice.InvoiceNumber] = invoice;
    }
}
