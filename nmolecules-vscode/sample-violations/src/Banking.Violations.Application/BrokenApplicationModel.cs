using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Application;

[ApplicationLayer]
[ApplicationService]
[ValueObject]
public sealed class BrokenTransferFacade
{
    public string UseCaseName { get; } = "broken-transfer";
}
