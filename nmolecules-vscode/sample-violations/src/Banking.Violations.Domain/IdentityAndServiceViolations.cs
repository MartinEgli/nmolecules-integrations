using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Domain;

[DomainLayer]
[AggregateRoot]
public class MultiIdentityAccount
{
    [Identity]
    public Guid AccountId { get; } = Guid.NewGuid();

    [Identity]
    public Guid CorrelationId { get; } = Guid.NewGuid();
}

[DomainLayer]
[Factory]
public sealed class FactoryWithIdentity
{
    [Identity]
    public string MisplacedId { get; } = "factory-id";
}

[DomainLayer]
[Service]
public sealed class LegacyRiskService
{
}

[ApplicationLayer]
[ApplicationService]
public sealed class ApplicationOrchestrator
{
}

[DomainLayer]
[DomainService]
public sealed class PolicyDependingOnApplicationService
{
    private readonly ApplicationOrchestrator orchestrator;

    public PolicyDependingOnApplicationService(ApplicationOrchestrator orchestrator)
    {
        this.orchestrator = orchestrator;
    }
}
