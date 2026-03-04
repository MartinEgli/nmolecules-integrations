using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.OnionAnalyzers
{
    public static class Rules
    {
        public const string OnionDependenciesMustPointInwardId = "XMoleculesOnion0001";
        public const string ClassicAndSimplifiedOnionStylesShouldNotMixId = "XMoleculesOnion0005";

        public static readonly DiagnosticDescriptor OnionDependenciesMustPointInwardRule = new(
            OnionDependenciesMustPointInwardId,
            "Onion dependencies must point inward only",
            "Onion ring symbol '{0}' must not depend on outer ring type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Inner onion rings must stay independent from outer implementation rings.");

        public static readonly DiagnosticDescriptor ClassicAndSimplifiedOnionStylesShouldNotMixRule = new(
            ClassicAndSimplifiedOnionStylesShouldNotMixId,
            "Classic and simplified Onion styles must not be mixed in one compilation",
            "Onion declaration on {0} mixes classic and simplified markers in the same compilation",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Use either classic or simplified onion markers in one compilation unit, not both.");
    }
}
