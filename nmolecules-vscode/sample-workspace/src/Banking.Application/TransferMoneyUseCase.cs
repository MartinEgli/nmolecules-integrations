using Banking.Domain;
using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Application;

[ApplicationLayer]
[ApplicationService]
public class TransferMoneyUseCase
{
    private readonly IAccounts accounts;
    private readonly TransferPolicy transferPolicy;

    public TransferMoneyUseCase(IAccounts accounts, TransferPolicy transferPolicy)
    {
        this.accounts = accounts;
        this.transferPolicy = transferPolicy;
    }

    public void Execute(Guid sourceAccountId, Guid targetAccountId, decimal amount)
    {
        var source = accounts.Get(sourceAccountId);
        var target = accounts.Get(targetAccountId);
        var money = new Money(amount, "EUR");

        transferPolicy.Transfer(source, target, money);
        accounts.Save(source);
        accounts.Save(target);
    }
}
