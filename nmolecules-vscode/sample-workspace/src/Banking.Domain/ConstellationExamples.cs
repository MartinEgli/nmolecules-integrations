using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Domain;

[DomainLayer]
[ValueObject]
public sealed class Address : IEquatable<Address>
{
    public Address(string street, string city)
    {
        Street = street;
        City = city;
    }

    public string Street { get; }
    public string City { get; }

    public bool Equals(Address? other)
    {
        return other is not null && Street == other.Street && City == other.City;
    }

    public override bool Equals(object? obj)
    {
        return obj is Address other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City);
    }
}

[DomainLayer]
[Entity]
public class CustomerProfile
{
    [Identity]
    public string CustomerNumber { get; } = Guid.NewGuid().ToString("N");

    public Address PostalAddress { get; private set; } = new("Main Street 1", "Berlin");

    public void Relocate(Address newAddress)
    {
        PostalAddress = newAddress;
    }
}

[DomainLayer]
[AggregateRoot]
public class Invoice
{
    [Identity]
    public string InvoiceNumber { get; } = Guid.NewGuid().ToString("N");

    public Address BillingAddress { get; }
    public Money TotalAmount { get; private set; }

    public Invoice(Address billingAddress, Money totalAmount)
    {
        BillingAddress = billingAddress;
        TotalAmount = totalAmount;
    }

    public void RegisterPayment(Money payment)
    {
        TotalAmount = new Money(TotalAmount.Amount - payment.Amount, TotalAmount.Currency);
    }
}

[DomainLayer]
[Repository]
public interface IInvoices
{
    Invoice Get(string invoiceNumber);
    void Save(Invoice invoice);
}

[DomainLayer]
[Factory]
public sealed class InvoiceFactory
{
    public Invoice Create(Address billingAddress, Money openingAmount)
    {
        return new Invoice(billingAddress, openingAmount);
    }
}

[DomainLayer]
[DomainService]
public sealed class InvoiceSettlementPolicy
{
    public bool CanSettle(Invoice invoice, Money payment)
    {
        return invoice.TotalAmount.Currency == payment.Currency
            && payment.Amount <= invoice.TotalAmount.Amount;
    }
}
