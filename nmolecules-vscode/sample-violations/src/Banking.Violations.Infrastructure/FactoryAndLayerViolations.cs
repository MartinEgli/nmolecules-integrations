using NMolecules.Architecture.Layered;
using NMolecules.DDD;

namespace Banking.Violations.Infrastructure;

[ApplicationLayer]
[ApplicationService]
public sealed class WorkflowFacade
{
}

[InfrastructureLayer]
[Factory]
public sealed class FactoryUsingApplicationService
{
    private readonly WorkflowFacade workflow;

    public FactoryUsingApplicationService(WorkflowFacade workflow)
    {
        this.workflow = workflow;
    }
}

[ApplicationLayer]
public sealed class ImportedApplicationService
{
}

[InfrastructureLayer]
public sealed class ImportedInfrastructureGateway
{
}

[UserInterfaceLayer]
public sealed class ImportedViewModel
{
}

[DomainLayer]
public sealed class DomainLayerUsingOtherLayers
{
    private readonly ImportedApplicationService applicationService;
    private readonly ImportedInfrastructureGateway infrastructureGateway;
    private readonly ImportedViewModel viewModel;

    public DomainLayerUsingOtherLayers(
        ImportedApplicationService applicationService,
        ImportedInfrastructureGateway infrastructureGateway,
        ImportedViewModel viewModel)
    {
        this.applicationService = applicationService;
        this.infrastructureGateway = infrastructureGateway;
        this.viewModel = viewModel;
    }
}
