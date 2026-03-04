using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Application;

[ApplicationLayer]
[ApplicationService]
[ValueObject]
public sealed class ConfusedApplicationService : IEquatable<ConfusedApplicationService>
{
    public bool Equals(ConfusedApplicationService? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ConfusedApplicationService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[DomainLayer]
[Service]
public sealed class LegacyPricingService
{
}

[ApplicationLayer]
[ApplicationService]
public sealed class ApplicationServiceUsingLegacyService
{
    private readonly LegacyPricingService pricingService;

    public ApplicationServiceUsingLegacyService(LegacyPricingService pricingService)
    {
        this.pricingService = pricingService;
    }
}

[UserInterfaceLayer]
public sealed class UiWizard
{
}

[ApplicationLayer]
public sealed class ApplicationLayerUsingUi
{
    private readonly UiWizard uiWizard;

    public ApplicationLayerUsingUi(UiWizard uiWizard)
    {
        this.uiWizard = uiWizard;
    }
}
