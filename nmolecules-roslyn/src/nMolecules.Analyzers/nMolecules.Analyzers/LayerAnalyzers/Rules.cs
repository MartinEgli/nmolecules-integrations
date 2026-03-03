using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.LayerAnalyzers
{
    public static class Rules
    {
        public const string DomainLayersShouldNotUseApplicationLayersId = "XMoleculesDomainLayer0001";
        public const string DomainLayersShouldNotUseUserInterfaceLayersId = "XMoleculesDomainLayer0002";
        public const string DomainLayersShouldNotUseInfrastructureLayersId = "XMoleculesDomainLayer0003";
        public const string ApplicationLayersShouldNotUseUserInterfaceLayersId = "XMoleculesApplicationLayer0003";

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseApplicationLayersRule = new(
            DomainLayersShouldNotUseApplicationLayersId,
            "Domain layer should not use application layer types",
            "Domain layer should not use application layer types",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer must remain independent of application-layer orchestration.");

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseUserInterfaceLayersRule = new(
            DomainLayersShouldNotUseUserInterfaceLayersId,
            "Domain layer should not use user interface layer types",
            "Domain layer should not use user interface layer types",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer must not depend on presentation concerns.");

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseInfrastructureLayersRule = new(
            DomainLayersShouldNotUseInfrastructureLayersId,
            "Domain layer should not use infrastructure layer types",
            "Domain layer should not use infrastructure layer types",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer should depend on abstractions, not infrastructure implementations.");

        public static readonly DiagnosticDescriptor ApplicationLayersShouldNotUseUserInterfaceLayersRule = new(
            ApplicationLayersShouldNotUseUserInterfaceLayersId,
            "Application layer should not use user interface layer types",
            "Application layer should not use user interface layer types",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The application layer should be driven by user interface concerns, not depend on them.");
    }
}
