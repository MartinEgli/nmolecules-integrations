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
            DiagnosticDescriptions.Create(
                "The same architectural scope declares multiple incompatible primary structural styles.",
                "A scope should have one primary structural style so dependency rules and interpretation remain coherent.",
                "Choose one primary style for the scope or split the conflicting styles into separate scopes or bounded contexts."));

        public static readonly DiagnosticDescriptor CqrsMayOverlayPrimaryStyleRule = new(
            CqrsMayOverlayPrimaryStyleId,
            "CQRS should overlay a primary structural style",
            "Scope '{0}' uses CQRS markers without a primary structural style marker (Layered, Onion, or Hexagonal)",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "CQRS markers are present without any primary structural style marker.",
                "CQRS is an overlay pattern and should refine an existing structural style instead of replacing one.",
                "Add a primary structural style marker such as Layered, Onion, or Hexagonal, or remove the incomplete CQRS overlay markers."));

        public static readonly DiagnosticDescriptor ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextRule = new(
            ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextId,
            "Classic and simplified onion styles must not coexist in the same bounded context",
            "Bounded context '{0}' mixes classic and simplified onion markers",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "One bounded context mixes classic and simplified onion markers.",
                "A bounded context should follow one onion interpretation so ring semantics stay unambiguous.",
                "Standardize the bounded context on either classic onion or simplified onion markers and remove the conflicting style."));
    }
}
