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
            "Application service should not also be marked as '{0}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Application services orchestrate use cases and should not simultaneously model domain building blocks.");

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotUseLegacyServicesRule = new(
            ApplicationServicesShouldNotUseLegacyServicesId,
            "Application services should not depend on legacy services",
            "Application service should depend on [DomainService] instead of legacy [Service]",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Application services should use explicit domain service dependencies instead of the legacy [Service] marker.");
    }
}
