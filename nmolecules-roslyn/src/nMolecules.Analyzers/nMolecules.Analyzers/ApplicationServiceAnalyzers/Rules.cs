using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    public static class Rules
    {
        public const string ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId = "XMoleculesApplicationService0001";

        public static readonly DiagnosticDescriptor ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule = new(
            ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId,
            "Application services should not also be domain building blocks",
            "Application service should not also be marked as '{0}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Application services orchestrate use cases and should not simultaneously model domain building blocks.");
    }
}
