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
            "API gateways should compose explicit service contracts instead of concrete service internals.");

        public static readonly DiagnosticDescriptor BackendForFrontendsShouldDependOnServiceContractsRule = new(
            BackendForFrontendsShouldDependOnServiceContractsId,
            "BFF components should depend on service contracts",
            "Backend-for-frontend '{0}' should depend on at least one service contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "BFF components should compose service contracts and avoid direct implementation coupling.");

        public static readonly DiagnosticDescriptor ServiceContractsShouldNotDependOnMicroserviceImplementationsRule = new(
            ServiceContractsShouldNotDependOnMicroserviceImplementationsId,
            "Service contracts must not depend on microservice implementations",
            "Service contract '{0}' must not depend on microservice '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Service contracts should remain implementation-agnostic boundaries.");

        public static readonly DiagnosticDescriptor IntegrationEventsShouldNotDependOnMicroserviceImplementationsRule = new(
            IntegrationEventsShouldNotDependOnMicroserviceImplementationsId,
            "Integration events must not depend on microservice implementations",
            "Integration event '{0}' must not depend on microservice '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Integration events should be stable payload contracts and not bind to service internals.");

        public static readonly DiagnosticDescriptor SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsRule = new(
            SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsId,
            "Saga orchestrators should depend on contracts or integration events",
            "Saga orchestrator '{0}' should depend on at least one service contract or integration event",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Saga orchestration should be driven by contracts and integration events.");

        public static readonly DiagnosticDescriptor SagaParticipantsShouldNotDependOnGatewayOrBffRule = new(
            SagaParticipantsShouldNotDependOnGatewayOrBffId,
            "Saga participants must not depend on gateways or BFF components",
            "Saga participant '{0}' must not depend on gateway or BFF '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Saga participants should collaborate via contracts and events instead of presentation edge components.");
    }
}
