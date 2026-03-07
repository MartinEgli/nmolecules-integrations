using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.HexagonalAnalyzers
{
    public static class Rules
    {
        public const string ApplicationCoreShouldNotDependOnPortsOrAdaptersId = "XMoleculesHexagonal0001";
        public const string PrimaryPortsShouldNotDependOnAdaptersId = "XMoleculesHexagonal0002";
        public const string SecondaryPortsShouldNotDependOnAdaptersId = "XMoleculesHexagonal0003";
        public const string PrimaryAdaptersShouldDependOnPrimaryPortsId = "XMoleculesHexagonal0004";
        public const string SecondaryAdaptersShouldDependOnSecondaryPortsId = "XMoleculesHexagonal0005";

        public static readonly DiagnosticDescriptor ApplicationCoreShouldNotDependOnPortsOrAdaptersRule = new(
            ApplicationCoreShouldNotDependOnPortsOrAdaptersId,
            "Hexagonal application core must not depend on ports or adapters",
            "Hexagonal application core '{0}' must not depend on port or adapter '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The application core depends on ports or adapters instead of remaining at the center of the hexagon.",
                "Hexagonal architecture keeps the application core independent from boundary contracts and adapter technology details.",
                "Move the dependency behind a core-owned abstraction or let adapters and ports depend on the application core instead."));

        public static readonly DiagnosticDescriptor PrimaryPortsShouldNotDependOnAdaptersRule = new(
            PrimaryPortsShouldNotDependOnAdaptersId,
            "Primary ports must not depend on adapters",
            "Primary port '{0}' must not depend on adapter '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A primary port depends on an adapter implementation.",
                "Primary ports are inbound contracts and must remain independent from concrete delivery mechanisms.",
                "Keep the port as a pure contract and move adapter details into the primary adapter layer."));

        public static readonly DiagnosticDescriptor SecondaryPortsShouldNotDependOnAdaptersRule = new(
            SecondaryPortsShouldNotDependOnAdaptersId,
            "Secondary ports must not depend on adapters",
            "Secondary port '{0}' must not depend on adapter '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A secondary port depends on an adapter implementation.",
                "Secondary ports are outbound contracts and must stay independent from concrete adapter technology.",
                "Leave the port as an abstraction and let the secondary adapter implement it from the outer edge."));

        public static readonly DiagnosticDescriptor PrimaryAdaptersShouldDependOnPrimaryPortsRule = new(
            PrimaryAdaptersShouldDependOnPrimaryPortsId,
            "Primary adapters should depend on primary ports",
            "Primary adapter '{0}' should depend on at least one primary port contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A primary adapter does not depend on any primary port contract.",
                "Primary adapters should expose the inbound boundary through primary ports, not by coupling directly to internal implementations.",
                "Introduce or depend on a primary port contract and let the adapter invoke the application core through that boundary."));

        public static readonly DiagnosticDescriptor SecondaryAdaptersShouldDependOnSecondaryPortsRule = new(
            SecondaryAdaptersShouldDependOnSecondaryPortsId,
            "Secondary adapters should depend on secondary ports",
            "Secondary adapter '{0}' should depend on at least one secondary port contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A secondary adapter does not depend on any secondary port contract.",
                "Secondary adapters should plug into the application core through outbound port contracts.",
                "Implement and depend on a secondary port contract instead of reaching the application core through concrete classes alone."));
    }
}
