using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    internal static class Diagnostics
    {
        public static Diagnostic ViolatesReadOnlyRule(this ISymbol symbol) =>
            symbol.Diagnostic(Rules.QueryModelsMustBeReadOnlyRule, symbol.ContainingType.DisplayName(), symbol.Name);
    }
}
