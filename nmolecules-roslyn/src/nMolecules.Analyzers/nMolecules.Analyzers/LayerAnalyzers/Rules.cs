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
            "Domain layer '{0}' must not depend on application layer '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain-layer type depends on an application-layer type.",
                "In layered architecture, the domain layer must stay independent from use-case orchestration.",
                "Move the orchestration upward into the application layer and keep domain code free of application-layer dependencies."));

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseUserInterfaceLayersRule = new(
            DomainLayersShouldNotUseUserInterfaceLayersId,
            "Domain layers must not depend on interface layers",
            "Domain layer '{0}' must not depend on interface layer '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain-layer type depends on an interface or presentation-layer type.",
                "Presentation concerns belong at the outer edge of a layered architecture and must not leak into the domain layer.",
                "Introduce an application-layer contract or DTO and keep interface-layer types outside the domain layer."));

        public static readonly DiagnosticDescriptor DomainLayersShouldNotUseInfrastructureLayersRule = new(
            DomainLayersShouldNotUseInfrastructureLayersId,
            "Domain layers must not depend on infrastructure layers",
            "Domain layer '{0}' must not depend on infrastructure layer '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain-layer type depends directly on an infrastructure-layer type.",
                "The domain layer must remain free of technical implementation details and depend only on abstractions.",
                "Push the dependency behind a domain-facing port or contract and wire the infrastructure implementation from an outer layer."));

        public static readonly DiagnosticDescriptor InterfaceLayersShouldNotUseDomainLayersRule = new(
            InterfaceLayersShouldNotUseDomainLayersId,
            "Interface layers must not depend on domain layers directly",
            "Interface layer '{0}' must not depend on domain layer '{1}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An interface-layer type reaches directly into the domain layer.",
                "The interface layer should hand requests to the application layer instead of bypassing use-case orchestration.",
                "Route the interaction through an application service or use-case boundary and keep the interface layer thin."));

        public static readonly DiagnosticDescriptor ApplicationLayersShouldLimitInfrastructureDependenciesRule = new(
            ApplicationLayersShouldLimitInfrastructureDependenciesId,
            "Application layers should keep infrastructure dependencies explicit and limited",
            "Application layer '{0}' depends on infrastructure layer '{1}'; keep this dependency explicit and limited",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "An application-layer type depends on infrastructure and the dependency is broader than a narrow integration seam.",
                "Layered architecture allows application-to-infrastructure references only as explicit, limited integration points.",
                "Confine the dependency to a small adapter-facing port or move technical concerns into infrastructure wiring code."));

        public static readonly DiagnosticDescriptor InfrastructureLayersShouldUseApplicationLayersForWiringOnlyRule = new(
            InfrastructureLayersShouldUseApplicationLayersForWiringOnlyId,
            "Infrastructure layers should keep application dependencies wiring-only",
            "Infrastructure layer '{0}' depends on application layer '{1}'; keep this dependency wiring-only",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "An infrastructure-layer type depends on the application layer for more than composition or wiring.",
                "Infrastructure may reference application services only at the composition edge, not to run business orchestration itself.",
                "Limit the dependency to startup or adapter wiring, or invert the dependency through an interface owned by the application layer."));
    }
}
