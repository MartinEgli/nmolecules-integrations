using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.HexagonalAnalyzers
{
    public static class Rules
    {
        public const string PrimaryPortsShouldNotDependOnAdaptersId = "XMoleculesHexagonal0002";
        public const string SecondaryPortsShouldNotDependOnAdaptersId = "XMoleculesHexagonal0003";

        public static readonly DiagnosticDescriptor PrimaryPortsShouldNotDependOnAdaptersRule = new(
            PrimaryPortsShouldNotDependOnAdaptersId,
            "Primary ports must not depend on adapters",
            "Primary port symbol '{0}' must not depend on adapter type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Primary ports are inbound abstractions and must stay independent from adapter implementations.");

        public static readonly DiagnosticDescriptor SecondaryPortsShouldNotDependOnAdaptersRule = new(
            SecondaryPortsShouldNotDependOnAdaptersId,
            "Secondary ports must not depend on adapters",
            "Secondary port symbol '{0}' must not depend on adapter type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Secondary ports are outbound abstractions and must stay independent from adapter implementations.");
    }
}
