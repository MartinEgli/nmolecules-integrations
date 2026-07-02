using NMolecules.Architecture.Hexagonal;
using NMolecules.Architecture.Layered;
using NMolecules.Architecture.Onion.Classic;
using NMolecules.Architecture.Onion.Simplified;
using NMolecules.Bricks;
using NMolecules.DDD;
using NMolecules.Events;
using Cqrs = NMolecules.Architecture.Cqrs;
using Es = NMolecules.Architecture.EventStorming;
using Micro = NMolecules.Architecture.Microservices;

[assembly: Rule("BRK001", "ApiRole", "DomainRole", RuleMode.ForbidDependency, "Brick rule '{rule}': '{source}' must not depend on '{target}' via member '{member}'")]
[assembly: Rule("", "ApiRole", "DomainRole")]

namespace Banking.Violations.Coverage;

[InfrastructureLayer]
public sealed class InfrastructureGateway
{
}

[DomainService]
public sealed class DomainPolicyService
{
}

[ApplicationService]
public sealed class ApplicationWorkflowService
{
}

[Service]
public sealed class LegacyCoverageService
{
}

[Repository]
public sealed class CoverageRepository
{
}

[Factory]
public sealed class CoverageFactory
{
}

[Entity]
public sealed class EntityMissingIdentity
{
}

[Entity]
public sealed class EntityWithMultipleIdentities
{
    [Identity]
    public Guid EntityId { get; } = Guid.NewGuid();

    [Identity]
    public Guid CorrelationId { get; } = Guid.NewGuid();
}

[Entity]
public sealed class EntityDependingOnRepository
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly CoverageRepository repository;

    public EntityDependingOnRepository(CoverageRepository repository)
    {
        this.repository = repository;
    }
}

[AggregateRoot]
public sealed class AggregateRootReference
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();
}

[Entity]
public sealed class EntityDependingOnAggregateRoot
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly AggregateRootReference aggregateRoot;

    public EntityDependingOnAggregateRoot(AggregateRootReference aggregateRoot)
    {
        this.aggregateRoot = aggregateRoot;
    }
}

[Entity]
public sealed class EntityDependingOnService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly DomainPolicyService domainPolicyService;

    public EntityDependingOnService(DomainPolicyService domainPolicyService)
    {
        this.domainPolicyService = domainPolicyService;
    }
}

[Entity]
public sealed class EntityDependingOnApplicationService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly ApplicationWorkflowService applicationService;

    public EntityDependingOnApplicationService(ApplicationWorkflowService applicationService)
    {
        this.applicationService = applicationService;
    }
}

[Entity]
public sealed class EntityDependingOnLegacyService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly LegacyCoverageService legacyService;

    public EntityDependingOnLegacyService(LegacyCoverageService legacyService)
    {
        this.legacyService = legacyService;
    }
}

[Entity]
public sealed class EntityDependingOnFactory
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly CoverageFactory factory;

    public EntityDependingOnFactory(CoverageFactory factory)
    {
        this.factory = factory;
    }
}

[AggregateRoot]
public sealed class AggregateRootMissingIdentity
{
}

[AggregateRoot]
public sealed class AggregateRootDependingOnRepository
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly CoverageRepository repository;

    public AggregateRootDependingOnRepository(CoverageRepository repository)
    {
        this.repository = repository;
    }
}

[AggregateRoot]
public sealed class AggregateRootDependingOnDomainService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly DomainPolicyService domainService;

    public AggregateRootDependingOnDomainService(DomainPolicyService domainService)
    {
        this.domainService = domainService;
    }
}

[AggregateRoot]
public sealed class AggregateRootDependingOnAggregateRoot
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly AggregateRootReference otherAggregateRoot;

    public AggregateRootDependingOnAggregateRoot(AggregateRootReference otherAggregateRoot)
    {
        this.otherAggregateRoot = otherAggregateRoot;
    }
}

[AggregateRoot]
public sealed class AggregateRootDependingOnApplicationService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly ApplicationWorkflowService applicationService;

    public AggregateRootDependingOnApplicationService(ApplicationWorkflowService applicationService)
    {
        this.applicationService = applicationService;
    }
}

[AggregateRoot]
public sealed class AggregateRootDependingOnFactory
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly CoverageFactory factory;

    public AggregateRootDependingOnFactory(CoverageFactory factory)
    {
        this.factory = factory;
    }
}

[AggregateRoot]
public sealed class AggregateRootDependingOnLegacyService
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    private readonly LegacyCoverageService legacyService;

    public AggregateRootDependingOnLegacyService(LegacyCoverageService legacyService)
    {
        this.legacyService = legacyService;
    }
}

[Repository]
public sealed class RepositoryDependingOnService
{
    private readonly DomainPolicyService domainService;

    public RepositoryDependingOnService(DomainPolicyService domainService)
    {
        this.domainService = domainService;
    }
}

[Repository]
public sealed class RepositoryDependingOnApplicationService
{
    private readonly ApplicationWorkflowService applicationService;

    public RepositoryDependingOnApplicationService(ApplicationWorkflowService applicationService)
    {
        this.applicationService = applicationService;
    }
}

[Repository]
public sealed class RepositoryDependingOnLegacyService
{
    private readonly LegacyCoverageService legacyService;

    public RepositoryDependingOnLegacyService(LegacyCoverageService legacyService)
    {
        this.legacyService = legacyService;
    }
}

[Repository]
public sealed class RepositoryWithInfrastructureSignature
{
    public InfrastructureGateway Expose(InfrastructureGateway dependency)
    {
        return dependency;
    }
}

[Repository]
public sealed class RepositoryDependingOnRepository
{
    private readonly CoverageRepository dependency;

    public RepositoryDependingOnRepository(CoverageRepository dependency)
    {
        this.dependency = dependency;
    }
}

[Repository]
public sealed class RepositoryDependingOnFactory
{
    private readonly CoverageFactory dependency;

    public RepositoryDependingOnFactory(CoverageFactory dependency)
    {
        this.dependency = dependency;
    }
}

[Repository]
public sealed class RepositoryWithApprovedConcreteComposition
{
    [AllowRepositoryComposition]
    private readonly CoverageRepository dependency;

    public RepositoryWithApprovedConcreteComposition(CoverageRepository dependency)
    {
        this.dependency = dependency;
    }
}

[DomainService]
public sealed class DomainServiceDependingOnConcreteRepository
{
    private readonly CoverageRepository repository;

    public DomainServiceDependingOnConcreteRepository(CoverageRepository repository)
    {
        this.repository = repository;
    }
}

[DomainService]
public sealed class DomainServiceWithInfrastructureSignature
{
    public InfrastructureGateway Expose(InfrastructureGateway dependency)
    {
        return dependency;
    }
}

[DomainService]
public sealed class DomainServiceDependingOnLegacyService
{
    private readonly LegacyCoverageService legacyService;

    public DomainServiceDependingOnLegacyService(LegacyCoverageService legacyService)
    {
        this.legacyService = legacyService;
    }
}

[ApplicationService]
public sealed class ApplicationServiceWithInfrastructureSignature
{
    public InfrastructureGateway Expose(InfrastructureGateway dependency)
    {
        return dependency;
    }
}

[Factory]
public sealed class FactoryDependingOnRepository
{
    private readonly CoverageRepository repository;

    public FactoryDependingOnRepository(CoverageRepository repository)
    {
        this.repository = repository;
    }
}

[Factory]
[Entity]
public sealed class FactoryAlsoEntity
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();
}

[Factory]
public sealed class FactoryDependingOnFactory
{
    private readonly CoverageFactory factory;

    public FactoryDependingOnFactory(CoverageFactory factory)
    {
        this.factory = factory;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnEntity : IEquatable<ValueObjectDependingOnEntity>
{
    private readonly EntityDependingOnRepository entity;

    public ValueObjectDependingOnEntity(EntityDependingOnRepository entity)
    {
        this.entity = entity;
    }

    public bool Equals(ValueObjectDependingOnEntity? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnEntity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnRepository : IEquatable<ValueObjectDependingOnRepository>
{
    private readonly CoverageRepository repository;

    public ValueObjectDependingOnRepository(CoverageRepository repository)
    {
        this.repository = repository;
    }

    public bool Equals(ValueObjectDependingOnRepository? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnRepository other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnAggregateRoot : IEquatable<ValueObjectDependingOnAggregateRoot>
{
    private readonly AggregateRootReference aggregateRoot;

    public ValueObjectDependingOnAggregateRoot(AggregateRootReference aggregateRoot)
    {
        this.aggregateRoot = aggregateRoot;
    }

    public bool Equals(ValueObjectDependingOnAggregateRoot? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnAggregateRoot other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnFactory : IEquatable<ValueObjectDependingOnFactory>
{
    private readonly CoverageFactory factory;

    public ValueObjectDependingOnFactory(CoverageFactory factory)
    {
        this.factory = factory;
    }

    public bool Equals(ValueObjectDependingOnFactory? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnFactory other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnDomainService : IEquatable<ValueObjectDependingOnDomainService>
{
    private readonly DomainPolicyService domainService;

    public ValueObjectDependingOnDomainService(DomainPolicyService domainService)
    {
        this.domainService = domainService;
    }

    public bool Equals(ValueObjectDependingOnDomainService? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnDomainService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnApplicationService : IEquatable<ValueObjectDependingOnApplicationService>
{
    private readonly ApplicationWorkflowService applicationService;

    public ValueObjectDependingOnApplicationService(ApplicationWorkflowService applicationService)
    {
        this.applicationService = applicationService;
    }

    public bool Equals(ValueObjectDependingOnApplicationService? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnApplicationService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[ValueObject]
public sealed class ValueObjectDependingOnLegacyService : IEquatable<ValueObjectDependingOnLegacyService>
{
    private readonly LegacyCoverageService legacyService;

    public ValueObjectDependingOnLegacyService(LegacyCoverageService legacyService)
    {
        this.legacyService = legacyService;
    }

    public bool Equals(ValueObjectDependingOnLegacyService? other)
    {
        return other is not null;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObjectDependingOnLegacyService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return 17;
    }
}

[NMolecules.Events.DomainEvent]
public sealed class DomainEventWithEntityReference
{
    public DomainEventWithEntityReference(EntityDependingOnRepository entity)
    {
        Entity = entity;
    }

    public EntityDependingOnRepository Entity { get; }
}

[NMolecules.Events.DomainEvent]
public sealed class DomainEventWithRepositoryReference
{
    public DomainEventWithRepositoryReference(CoverageRepository repository)
    {
        Repository = repository;
    }

    public CoverageRepository Repository { get; }
}

[NMolecules.Events.DomainEvent]
public sealed class DomainEventWithServiceReference
{
    public DomainEventWithServiceReference(DomainPolicyService service)
    {
        Service = service;
    }

    public DomainPolicyService Service { get; }
}

[DomainEventPublisher]
public sealed class DomainEventPublisherHostWithoutPreferredRole
{
}

[AggregateRoot]
public sealed class AggregateRootPublisherWithoutPayload
{
    [Identity]
    public Guid Id { get; } = Guid.NewGuid();

    [DomainEventPublisher]
    public void Publish()
    {
    }
}

[DomainLayer]
public sealed class DomainLayerType
{
}

[InterfaceLayer]
public sealed class InterfaceLayerDependingOnDomain
{
    private readonly DomainLayerType domainLayerType;

    public InterfaceLayerDependingOnDomain(DomainLayerType domainLayerType)
    {
        this.domainLayerType = domainLayerType;
    }
}

[ApplicationLayer]
public sealed class ApplicationLayerDependingOnInfrastructure
{
    private readonly InfrastructureGateway infrastructureGateway;

    public ApplicationLayerDependingOnInfrastructure(InfrastructureGateway infrastructureGateway)
    {
        this.infrastructureGateway = infrastructureGateway;
    }
}

[NMolecules.Architecture.Onion.Classic.InfrastructureRing]
public sealed class OnionInfrastructure
{
}

[DomainModelRing]
public sealed class ClassicDomainModelDependingOnOuterRing
{
    private readonly ClassicDomainServiceDependingOnOuterRing domainService;

    public ClassicDomainModelDependingOnOuterRing(ClassicDomainServiceDependingOnOuterRing domainService)
    {
        this.domainService = domainService;
    }
}

[DomainServiceRing]
public sealed class ClassicDomainServiceDependingOnOuterRing
{
    private readonly ClassicApplicationServiceDependingOnInfrastructure applicationService;

    public ClassicDomainServiceDependingOnOuterRing(ClassicApplicationServiceDependingOnInfrastructure applicationService)
    {
        this.applicationService = applicationService;
    }
}

[ApplicationServiceRing]
public sealed class ClassicApplicationServiceDependingOnInfrastructure
{
    private readonly OnionInfrastructure infrastructure;

    public ClassicApplicationServiceDependingOnInfrastructure(OnionInfrastructure infrastructure)
    {
        this.infrastructure = infrastructure;
    }
}

[DomainRing]
public sealed class SimplifiedDomainDependingOnOuterRing
{
    private readonly SimplifiedApplicationDependingOnInfrastructure application;

    public SimplifiedDomainDependingOnOuterRing(SimplifiedApplicationDependingOnInfrastructure application)
    {
        this.application = application;
    }
}

[ApplicationRing]
public sealed class SimplifiedApplicationDependingOnInfrastructure
{
    private readonly OnionInfrastructure infrastructure;

    public SimplifiedApplicationDependingOnInfrastructure(OnionInfrastructure infrastructure)
    {
        this.infrastructure = infrastructure;
    }
}

[Application]
public sealed class HexagonalApplicationDependingOnPort
{
    private readonly IPrimaryPortContract primaryPort;

    public HexagonalApplicationDependingOnPort(IPrimaryPortContract primaryPort)
    {
        this.primaryPort = primaryPort;
    }
}

[PrimaryPort]
public interface IPrimaryPortContract
{
    PrimaryAdapterWithoutPrimaryPortDependency Resolve();
}

[SecondaryPort]
public interface ISecondaryPortContract
{
    SecondaryAdapterWithoutSecondaryPortDependency Resolve();
}

[PrimaryAdapter]
public sealed class PrimaryAdapterWithoutPrimaryPortDependency
{
}

[SecondaryAdapter]
public sealed class SecondaryAdapterWithoutSecondaryPortDependency
{
}

[Cqrs.QueryModel]
public sealed class MutableReadModel
{
    public string State { get; set; } = string.Empty;
}

public sealed class BrokenCqrsHandlers
{
    [Cqrs.CommandHandler]
    public void HandleCommand(MutableReadModel model)
    {
        _ = model;
    }

    [Cqrs.QueryHandler]
    public string HandleQuery(AggregateRootReference aggregateRoot)
    {
        _ = aggregateRoot;
        return string.Empty;
    }

    [Cqrs.CommandDispatcher]
    public void Dispatch(CoverageRepository repository)
    {
        _ = repository;
    }
}

[Cqrs.Projection]
public sealed class ProjectionDependingOnWriteModel
{
    private readonly AggregateRootReference aggregateRoot;

    public ProjectionDependingOnWriteModel(AggregateRootReference aggregateRoot)
    {
        this.aggregateRoot = aggregateRoot;
    }
}

[Es.Aggregate]
public sealed class EventStormingAggregate
{
}

[Es.Actor]
public sealed class EventStormingActorDependingOnAggregate
{
    private readonly EventStormingAggregate aggregate;

    public EventStormingActorDependingOnAggregate(EventStormingAggregate aggregate)
    {
        this.aggregate = aggregate;
    }
}

[Es.Command]
public sealed class EventStormingCommandWithoutAggregateDependency
{
}

[Es.Policy]
public sealed class EventStormingPolicyWithoutDomainEventDependency
{
}

[Es.ReadModel]
public sealed class EventStormingReadModelDependingOnAggregate
{
    private readonly EventStormingAggregate aggregate;

    public EventStormingReadModelDependingOnAggregate(EventStormingAggregate aggregate)
    {
        this.aggregate = aggregate;
    }
}

[Es.ExternalSystem]
public sealed class EventStormingExternalSystemDependingOnAggregate
{
    private readonly EventStormingAggregate aggregate;

    public EventStormingExternalSystemDependingOnAggregate(EventStormingAggregate aggregate)
    {
        this.aggregate = aggregate;
    }
}

[Micro.Microservice]
public sealed class MicroserviceImplementation
{
}

[Micro.ApiGateway]
public sealed class ApiGatewayWithoutContractDependency
{
}

[Micro.BackendForFrontend]
public sealed class BackendForFrontendWithoutContractDependency
{
}

[Micro.ServiceContract]
public interface IServiceContractDependingOnMicroservice
{
    MicroserviceImplementation Resolve();
}

[Micro.IntegrationEvent]
public sealed class IntegrationEventDependingOnMicroservice
{
    public IntegrationEventDependingOnMicroservice(MicroserviceImplementation microservice)
    {
        Microservice = microservice;
    }

    public MicroserviceImplementation Microservice { get; }
}

[Micro.SagaOrchestrator]
public sealed class SagaOrchestratorWithoutContractOrEventDependency
{
}

[Micro.SagaParticipant]
public sealed class SagaParticipantDependingOnEdgeComponents
{
    private readonly ApiGatewayWithoutContractDependency gateway;
    private readonly BackendForFrontendWithoutContractDependency backendForFrontend;

    public SagaParticipantDependingOnEdgeComponents(
        ApiGatewayWithoutContractDependency gateway,
        BackendForFrontendWithoutContractDependency backendForFrontend)
    {
        this.gateway = gateway;
        this.backendForFrontend = backendForFrontend;
    }
}

[Role("DomainRole")]
public sealed class BrickDomainComponent
{
}

[Role("ApiRole")]
public sealed class BrickApiComponent
{
    private readonly BrickDomainComponent domainComponent;

    public BrickApiComponent(BrickDomainComponent domainComponent)
    {
        this.domainComponent = domainComponent;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickOnlyOneMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickAllLeftMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickAllRightMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickRepeatedMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickXorLeftMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class BrickXorRightMarkerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
[RequireExactlyOneMember(typeof(BrickOnlyOneMarkerAttribute))]
public sealed class BrickExactlyOneContractAttribute : Attribute
{
}

[BrickExactlyOneContract]
public sealed class BrickExactlyOneContractViolation
{
    [BrickOnlyOneMarker]
    public string Primary { get; } = string.Empty;

    [BrickOnlyOneMarker]
    public string Duplicate { get; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Class)]
[RequireAllMembers(typeof(BrickAllLeftMarkerAttribute), typeof(BrickAllRightMarkerAttribute))]
public sealed class BrickAllMembersContractAttribute : Attribute
{
}

[BrickAllMembersContract]
public sealed class BrickAllMembersContractViolation
{
    [BrickAllLeftMarker]
    public string OnlyLeft { get; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Class)]
[RequireMemberCount(typeof(BrickRepeatedMarkerAttribute), 2)]
public sealed class BrickMemberCountContractAttribute : Attribute
{
}

[BrickMemberCountContract]
public sealed class BrickMemberCountContractViolation
{
    [BrickRepeatedMarker]
    public string OnlyOne { get; } = string.Empty;
}

[AttributeUsage(AttributeTargets.Class)]
[RequireExclusiveChoice(typeof(BrickXorLeftMarkerAttribute), typeof(BrickXorRightMarkerAttribute))]
public sealed class BrickExclusiveChoiceContractAttribute : Attribute
{
}

[BrickExclusiveChoiceContract]
public sealed class BrickExclusiveChoiceContractViolation
{
    [BrickXorLeftMarker]
    public string Left { get; } = string.Empty;

    [BrickXorRightMarker]
    public string Right { get; } = string.Empty;
}
