using Banking.Application;
using Banking.Domain;
using NMolecules.Architecture.Layered;

namespace Banking.Api;

[UserInterfaceLayer]
public sealed class InvoiceEndpoints
{
    private readonly InvoiceSettlementUseCase useCase;

    public InvoiceEndpoints(InvoiceSettlementUseCase useCase)
    {
        this.useCase = useCase;
    }

    public bool PostSettlement(string invoiceNumber, decimal amount, string currency)
    {
        return useCase.Settle(invoiceNumber, new Money(amount, currency));
    }
}
