using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.LayerAnalyzers
{
    public static class Rules
    {
        public const string DomainLayersShouldNotUseApplicationLayersId = "XMoleculesLayered0001";
        public const string DomainLayersShouldNotUseInfrastructureLayersId = "XMoleculesLayered0002";
        public const string DomainLayersShouldNotUseUserInterfaceLayersId = "XMoleculesLayered0003";
        public const string InterfaceLayersShouldNotUseDomainLayersId = "XMoleculesLayered0004";

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseApplicationLayersRule = new(
            DomainLayersShouldNotUseApplicationLayersId,
            "Domain layers must not depend on application layers",
            "Domain layer symbol '{0}' must not depend on application layer type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer must remain independent of application-layer orchestration.");

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseUserInterfaceLayersRule = new(
            DomainLayersShouldNotUseUserInterfaceLayersId,
            "Domain layers must not depend on interface layers",
            "Domain layer symbol '{0}' must not depend on interface layer type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer must not depend on presentation concerns.");

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseInfrastructureLayersRule = new(
            DomainLayersShouldNotUseInfrastructureLayersId,
            "Domain layers must not depend on infrastructure layers",
            "Domain layer symbol '{0}' must not depend on infrastructure layer type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain layer should depend on abstractions, not infrastructure implementations.");

        public static readonly DiagnosticDescriptor InterfaceLayersShouldNotUseDomainLayersRule = new(
            InterfaceLayersShouldNotUseDomainLayersId,
            "Interface layers must not depend on domain layers directly",
            "Interface layer symbol '{0}' must not depend on domain layer type '{1}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The interface layer must not bypass application orchestration and couple directly to the domain layer.");
    }
}
