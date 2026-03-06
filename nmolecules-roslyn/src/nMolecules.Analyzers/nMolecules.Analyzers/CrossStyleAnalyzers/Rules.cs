using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CrossStyleAnalyzers
{
    public static class Rules
    {
        public const string PrimaryStylesMustFollowCompatibilityMatrixId = "XMoleculesCrossStyle0001";
        public const string CqrsMayOverlayPrimaryStyleId = "XMoleculesCrossStyle0002";
        public const string ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextId = "XMoleculesCrossStyle0003";

        public static readonly DiagnosticDescriptor PrimaryStylesMustFollowCompatibilityMatrixRule = new(
            PrimaryStylesMustFollowCompatibilityMatrixId,
            "Primary structural styles must follow the compatibility matrix",
            "Scope '{0}' contains incompatible primary structural styles: {1}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Layered and Onion are mutually exclusive as primary structural styles in the same architectural scope.");

        public static readonly DiagnosticDescriptor CqrsMayOverlayPrimaryStyleRule = new(
            CqrsMayOverlayPrimaryStyleId,
            "CQRS should overlay a primary structural style",
            "Scope '{0}' uses CQRS markers without a primary structural style marker (Layered, Onion, or Hexagonal)",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "CQRS should be used as an overlay on top of a primary structural style.");

        public static readonly DiagnosticDescriptor ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextRule = new(
            ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextId,
            "Classic and simplified onion styles must not coexist in the same bounded context",
            "Bounded context '{0}' mixes classic and simplified onion markers",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Choose either classic onion or simplified onion markers per bounded context.");
    }
}
