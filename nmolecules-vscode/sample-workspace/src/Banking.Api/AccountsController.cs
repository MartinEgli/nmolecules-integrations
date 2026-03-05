using Banking.Application;
using NMolecules.Architecture.Layered;

namespace Banking.Api;

[UserInterfaceLayer]
public class AccountsController
{
    private readonly TransferMoneyUseCase transferMoneyUseCase;

    public AccountsController(TransferMoneyUseCase transferMoneyUseCase)
    {
        this.transferMoneyUseCase = transferMoneyUseCase;
    }

    public void PostTransfer(Guid sourceAccountId, Guid targetAccountId, decimal amount)
    {
        transferMoneyUseCase.Execute(sourceAccountId, targetAccountId, amount);
    }
}
