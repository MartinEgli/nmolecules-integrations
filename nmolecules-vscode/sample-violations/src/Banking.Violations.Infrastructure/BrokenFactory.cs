using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Infrastructure;

[InfrastructureLayer]
[ApplicationService]
public sealed class WrongLayerFacade
{
}

[InfrastructureLayer]
[Factory]
public sealed class BrokenTransferFactory
{
    public BrokenTransferFactory(WrongLayerFacade facade)
    {
        Facade = facade;
    }

    public WrongLayerFacade Facade { get; }
}
