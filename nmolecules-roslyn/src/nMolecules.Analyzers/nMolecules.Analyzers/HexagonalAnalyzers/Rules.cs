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
            "The hexagonal application core should stay independent from boundary contracts and adapter technology details.");

        public static readonly DiagnosticDescriptor PrimaryPortsShouldNotDependOnAdaptersRule = new(
            PrimaryPortsShouldNotDependOnAdaptersId,
            "Primary ports must not depend on adapters",
            "Primary port '{0}' must not depend on adapter '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Primary ports are inbound abstractions and must stay independent from adapter implementations.");

        public static readonly DiagnosticDescriptor SecondaryPortsShouldNotDependOnAdaptersRule = new(
            SecondaryPortsShouldNotDependOnAdaptersId,
            "Secondary ports must not depend on adapters",
            "Secondary port '{0}' must not depend on adapter '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Secondary ports are outbound abstractions and must stay independent from adapter implementations.");

        public static readonly DiagnosticDescriptor PrimaryAdaptersShouldDependOnPrimaryPortsRule = new(
            PrimaryAdaptersShouldDependOnPrimaryPortsId,
            "Primary adapters should depend on primary ports",
            "Primary adapter '{0}' should depend on at least one primary port contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Primary adapters should expose their boundary via primary port contracts instead of direct implementation coupling.");

        public static readonly DiagnosticDescriptor SecondaryAdaptersShouldDependOnSecondaryPortsRule = new(
            SecondaryAdaptersShouldDependOnSecondaryPortsId,
            "Secondary adapters should depend on secondary ports",
            "Secondary adapter '{0}' should depend on at least one secondary port contract",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Secondary adapters should expose outbound boundaries via secondary port contracts.");
    }
}
