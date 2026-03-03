using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Domain;

[DomainLayer]
[AggregateRoot]
public class BankAccount
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    public Money Balance { get; private set; } = new(0m, "EUR");

    public void Credit(Money amount)
    {
        Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
    }

    public void Debit(Money amount)
    {
        Balance = new Money(Balance.Amount - amount.Amount, Balance.Currency);
    }
}

[DomainLayer]
[ValueObject]
public readonly struct Money : IEquatable<Money>
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public bool Equals(Money other)
    {
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }
}

[DomainLayer]
[Repository]
public interface IAccounts
{
    BankAccount Get(Guid id);
    void Save(BankAccount account);
}

[DomainLayer]
[Factory]
public class BankAccountFactory
{
    public BankAccount OpenAccount()
    {
        return new BankAccount();
    }
}

[DomainLayer]
[DomainService]
public class TransferPolicy
{
    public void Transfer(BankAccount source, BankAccount target, Money amount)
    {
        source.Debit(amount);
        target.Credit(amount);
    }
}
