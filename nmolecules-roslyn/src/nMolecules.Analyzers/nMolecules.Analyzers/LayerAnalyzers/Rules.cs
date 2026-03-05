using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.LayerAnalyzers
{
    public static class Rules
    {
        public const string DomainLayersShouldNotUseApplicationLayersId = "XMoleculesLayered0001";
        public const string DomainLayersShouldNotUseInfrastructureLayersId = "XMoleculesLayered0002";
        public const string DomainLayersShouldNotUseUserInterfaceLayersId = "XMoleculesLayered0003";
        public const string InterfaceLayersShouldNotUseDomainLayersId = "XMoleculesLayered0004";
        public const string ApplicationLayersShouldLimitInfrastructureDependenciesId = "XMoleculesLayered0005";
        public const string InfrastructureLayersShouldUseApplicationLayersForWiringOnlyId = "XMoleculesLayered0006";

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

        public static readonly DiagnosticDescriptor ApplicationLayersShouldLimitInfrastructureDependenciesRule = new(
            ApplicationLayersShouldLimitInfrastructureDependenciesId,
            "Application to infrastructure dependencies should stay explicit and limited",
            "Application layer symbol '{0}' depends on infrastructure layer type '{1}'; keep this dependency explicit and limited",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Application-layer infrastructure coupling should be explicit and constrained to narrowly scoped integration points.");

        public static readonly DiagnosticDescriptor InfrastructureLayersShouldUseApplicationLayersForWiringOnlyRule = new(
            InfrastructureLayersShouldUseApplicationLayersForWiringOnlyId,
            "Infrastructure to application dependencies should be wiring-only",
            "Infrastructure layer symbol '{0}' depends on application layer type '{1}'; keep this dependency wiring-only",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Infrastructure-layer references to application services should stay in composition/wiring boundaries, not business orchestration.");
    }
}
