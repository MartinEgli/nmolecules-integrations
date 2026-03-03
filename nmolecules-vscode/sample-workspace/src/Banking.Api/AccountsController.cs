using Banking.Application;
using Banking.Infrastructure;
using NMolecules.Architecture.Layered;

namespace Banking.Api;

[UserInterfaceLayer]
public class AccountsController
{
    private readonly TransferMoneyUseCase transferMoneyUseCase;

    public AccountsController()
    {
        var accounts = new InMemoryAccounts();
        var transferPolicy = new Banking.Domain.TransferPolicy();
        transferMoneyUseCase = new TransferMoneyUseCase(accounts, transferPolicy);
    }

    public void PostTransfer(Guid sourceAccountId, Guid targetAccountId, decimal amount)
    {
        transferMoneyUseCase.Execute(sourceAccountId, targetAccountId, amount);
    }
}
