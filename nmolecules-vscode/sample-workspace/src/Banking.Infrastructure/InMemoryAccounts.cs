using Banking.Domain;
using NMolecules.Architecture.Layered;

namespace Banking.Infrastructure;

[InfrastructureLayer]
public class InMemoryAccounts : IAccounts
{
    private readonly Dictionary<Guid, BankAccount> storage = new();

    public BankAccount Get(Guid id)
    {
        if (!storage.TryGetValue(id, out var account))
        {
            account = new BankAccount();
            storage[id] = account;
        }

        return account;
    }

    public void Save(BankAccount account)
    {
        storage[account.Id] = account;
    }
}
