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
            "Application services should not also be domain building blocks",
            "Application service '{0}' must not also be marked as '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Application services orchestrate use cases and should not simultaneously model domain building blocks.");

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotUseLegacyServicesRule = new(
            ApplicationServicesShouldNotUseLegacyServicesId,
            "Application services should not depend on legacy services",
            "Application service '{0}' depends on legacy [Service] type '{1}'; use [DomainService] instead",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Application services should use explicit domain service dependencies instead of the legacy [Service] marker.");

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotUseApplicationServicesRule = new(
            ApplicationServicesShouldNotUseApplicationServicesId,
            "Application services should not depend on other application services",
            "Application service '{0}' must not depend on application service '{1}' directly",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Application services should orchestrate a use case boundary directly instead of chaining through other application services.");

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotExposeInfrastructureSignaturesRule = new(
            ApplicationServicesShouldNotExposeInfrastructureSignaturesId,
            "Application services should not expose infrastructure-layer types in public signatures",
            "Application service '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Application services should keep their public API independent from infrastructure-layer types.");
    }
}
