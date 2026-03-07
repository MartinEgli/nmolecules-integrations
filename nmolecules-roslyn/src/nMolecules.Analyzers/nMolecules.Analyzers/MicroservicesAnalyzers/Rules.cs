using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.MicroservicesAnalyzers
{
    public static class Rules
    {
        public const string ApiGatewaysShouldDependOnServiceContractsId = "XMoleculesMicroservices0001";
        public const string BackendForFrontendsShouldDependOnServiceContractsId = "XMoleculesMicroservices0002";
        public const string ServiceContractsShouldNotDependOnMicroserviceImplementationsId = "XMoleculesMicroservices0003";
        public const string IntegrationEventsShouldNotDependOnMicroserviceImplementationsId = "XMoleculesMicroservices0004";
        public const string SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsId = "XMoleculesMicroservices0005";
        public const string SagaParticipantsShouldNotDependOnGatewayOrBffId = "XMoleculesMicroservices0006";

        public static readonly DiagnosticDescriptor ApiGatewaysShouldDependOnServiceContractsRule = new(
            ApiGatewaysShouldDependOnServiceContractsId,
            "API gateways should depend on service contracts",
            "API gateway '{0}' should depend on at least one service contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The API gateway is modeled without depending on any service contract.",
                "API gateways should aggregate stable service contracts instead of concrete service internals.",
                "Introduce or reference service contracts for the gateway boundary and keep service implementation details behind those contracts."));

        public static readonly DiagnosticDescriptor BackendForFrontendsShouldDependOnServiceContractsRule = new(
            BackendForFrontendsShouldDependOnServiceContractsId,
            "BFF components should depend on service contracts",
            "Backend-for-frontend '{0}' should depend on at least one service contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The backend-for-frontend component does not depend on any service contract.",
                "BFF components should compose stable service contracts rather than concrete microservice implementations.",
                "Depend on service contracts at the BFF boundary and move implementation-specific details behind those contracts."));

        public static readonly DiagnosticDescriptor ServiceContractsShouldNotDependOnMicroserviceImplementationsRule = new(
            ServiceContractsShouldNotDependOnMicroserviceImplementationsId,
            "Service contracts must not depend on microservice implementations",
            "Service contract '{0}' must not depend on microservice '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A service contract depends on a concrete microservice implementation.",
                "Service contracts define boundary agreements and must remain independent from service internals.",
                "Remove the implementation dependency and keep the contract limited to stable request, response, and message types."));

        public static readonly DiagnosticDescriptor IntegrationEventsShouldNotDependOnMicroserviceImplementationsRule = new(
            IntegrationEventsShouldNotDependOnMicroserviceImplementationsId,
            "Integration events must not depend on microservice implementations",
            "Integration event '{0}' must not depend on microservice '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An integration event depends on a concrete microservice implementation.",
                "Integration events are cross-service contracts and must stay stable and implementation-agnostic.",
                "Model the event as a pure contract payload and remove references to service internals."));

        public static readonly DiagnosticDescriptor SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsRule = new(
            SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsId,
            "Saga orchestrators should depend on contracts or integration events",
            "Saga orchestrator '{0}' should depend on at least one service contract or integration event",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A saga orchestrator does not depend on any service contract or integration event.",
                "Saga orchestration should coordinate through stable contracts and events instead of hidden implementation coupling.",
                "Drive the saga through service contracts or integration events that make the orchestration boundary explicit."));

        public static readonly DiagnosticDescriptor SagaParticipantsShouldNotDependOnGatewayOrBffRule = new(
            SagaParticipantsShouldNotDependOnGatewayOrBffId,
            "Saga participants must not depend on gateways or BFF components",
            "Saga participant '{0}' must not depend on gateway or BFF '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A saga participant depends on an API gateway or BFF component.",
                "Saga participants are service-internal collaborators and must not couple to presentation edge components.",
                "Collaborate through contracts, commands, or integration events instead of reaching through gateways or BFFs."));
    }
}
