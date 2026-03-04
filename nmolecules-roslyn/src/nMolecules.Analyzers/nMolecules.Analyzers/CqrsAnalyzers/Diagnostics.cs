using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    internal static class Diagnostics
    {
        public static Diagnostic ViolatesCommandHandlerQueryModelDependency(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(Rules.CommandHandlersMustNotDependOnQueryModelsRule, symbol.DiagnosticTargetName(), type.DisplayName());

        public static Diagnostic ViolatesCommandDispatcherDomainDependency(this ISymbol symbol, string role, ITypeSymbol type) =>
            symbol.Diagnostic(Rules.CommandDispatchersMustNotContainDomainRulesRule, symbol.DiagnosticTargetName(), role, type.DisplayName());

        public static Diagnostic ViolatesReadOnlyRule(this ISymbol symbol) =>
            symbol.Diagnostic(Rules.QueryModelsMustBeReadOnlyRule, symbol.ContainingType.DisplayName(), symbol.Name);
    }
}
