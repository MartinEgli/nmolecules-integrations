using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    public static class Rules
    {
        public const string ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId = "XMoleculesApplicationService0001";
        public const string ApplicationServicesShouldNotUseLegacyServicesId = "XMoleculesApplicationService0002";
        public const string ApplicationServicesShouldNotUseApplicationServicesId = "XMoleculesApplicationService0003";
        public const string ApplicationServicesShouldNotExposeInfrastructureSignaturesId = "XMoleculesApplicationService0004";

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule = new(
            ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId,
            "Application services must not also be domain building blocks",
            "Application service '{0}' must not also be marked as '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The same type is marked both as an application orchestrator and as a domain building block.",
                "Application services own use-case coordination, while domain building blocks own domain state or domain logic.",
                "Keep the application service focused on orchestration and move domain behavior into a dedicated entity, aggregate root, value object, repository, or domain service."));

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotUseLegacyServicesRule = new(
            ApplicationServicesShouldNotUseLegacyServicesId,
            "Application services should not depend on legacy services",
            "Application service '{0}' depends on legacy [Service] type '{1}'; use [DomainService] instead",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The application service depends on a type that still uses the ambiguous legacy [Service] marker.",
                "Application-service dependencies should express whether the collaborator is a domain service or another orchestration component.",
                "Replace legacy [Service] usage with an explicit [DomainService] or [ApplicationService] role so the dependency direction is architecturally clear."));

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotUseApplicationServicesRule = new(
            ApplicationServicesShouldNotUseApplicationServicesId,
            "Application services should not depend on other application services directly",
            "Application service '{0}' must not depend on application service '{1}' directly",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "One application service calls another application service directly to continue orchestration.",
                "Each application service should own one use-case boundary instead of forming orchestration chains.",
                "Extract shared domain logic into domain services or shared infrastructure ports, and keep application services as the top-level use-case entry points."));

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotExposeInfrastructureSignaturesRule = new(
            ApplicationServicesShouldNotExposeInfrastructureSignaturesId,
            "Application services should not expose infrastructure-layer types in public signatures",
            "Application service '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A public application-service API leaks an infrastructure-specific type through a parameter, return type, or member signature.",
                "Application-service contracts should remain stable at the use-case boundary and must not expose technical adapter details.",
                "Replace the infrastructure type with a domain, contract, or DTO abstraction and keep infrastructure mapping behind the application boundary."));
    }
}
