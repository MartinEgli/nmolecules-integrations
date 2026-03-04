using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    public static class Rules
    {
        public const string ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId = "XMoleculesApplicationService0001";
        public const string ApplicationServicesShouldNotUseLegacyServicesId = "XMoleculesApplicationService0002";

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
    }
}
