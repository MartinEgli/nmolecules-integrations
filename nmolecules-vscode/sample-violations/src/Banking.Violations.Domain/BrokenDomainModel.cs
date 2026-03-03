using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Domain;

[DomainLayer]
[ValueObject]
public class BrokenMoney
{
    [Identity]
    public Guid Id { get; set; }

    public decimal Amount { get; set; }
}

[DomainLayer]
[AggregateRoot]
public class BrokenAccount
{
    [Identity]
    public Guid PrimaryId { get; } = Guid.NewGuid();

    [Identity]
    public Guid SecondaryId { get; } = Guid.NewGuid();
}

[DomainLayer]
[Service]
public class LegacyBillingService
{
}
