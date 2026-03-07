using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.DomainServiceAnalyzers
{
    public static class Rules
    {
        public const string DomainServicesShouldNotUseApplicationServicesId = "XMoleculesDomainService0001";
        public const string DomainServicesShouldOnlyUseRepositoryContractsId = "XMoleculesDomainService0002";
        public const string DomainServicesShouldNotExposeInfrastructureSignaturesId = "XMoleculesDomainService0003";
        public const string DomainServicesShouldNotUseLegacyServicesId = "XMoleculesDomainService0004";

        public static readonly DiagnosticDescriptor DomainServicesShouldNotUseApplicationServicesRule = new(
            DomainServicesShouldNotUseApplicationServicesId,
            "Domain services must not depend on application services",
            "Domain service '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain service reaches outward to an application-service collaborator.",
                "Domain services are part of the domain model and must stay independent from use-case orchestration.",
                "Move orchestration back into the application layer and let the domain service depend only on domain concepts and stable repository contracts."));

        public static readonly DiagnosticDescriptor DomainServicesShouldOnlyUseRepositoryContractsRule = new(
            DomainServicesShouldOnlyUseRepositoryContractsId,
            "Domain services must depend on repository contracts only",
            "Domain service '{0}' must not depend on concrete repository '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The domain service references a concrete repository implementation instead of an abstraction.",
                "Domain services may rely on persistence capabilities, but only through repository contracts that preserve infrastructure independence.",
                "Inject an interface or abstract repository contract and keep the concrete implementation on the infrastructure side."));

        public static readonly DiagnosticDescriptor DomainServicesShouldNotExposeInfrastructureSignaturesRule = new(
            DomainServicesShouldNotExposeInfrastructureSignaturesId,
            "Domain services should not expose infrastructure-layer types in public signatures",
            "Domain service '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A public domain-service signature exposes an infrastructure type as part of its contract.",
                "Domain-service APIs must express domain intent without coupling callers to technical adapter or persistence types.",
                "Replace the infrastructure type with a domain abstraction or dedicated DTO and keep the infrastructure mapping outside the domain service."));

        public static readonly DiagnosticDescriptor DomainServicesShouldNotUseLegacyServicesRule = new(
            DomainServicesShouldNotUseLegacyServicesId,
            "Domain services should not depend on legacy services",
            "Domain service '{0}' should not depend on legacy service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The domain service depends on a collaborator marked with the generic legacy [Service] attribute.",
                "Domain dependencies should use explicit role markers so the boundary between domain and application responsibilities remains visible.",
                "Retype the dependency as [DomainService] or [ApplicationService] and keep the domain service dependent only on domain-level collaborators."));
    }
}
