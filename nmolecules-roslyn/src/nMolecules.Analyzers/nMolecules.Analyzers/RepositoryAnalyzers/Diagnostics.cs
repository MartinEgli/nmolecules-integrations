using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsService())
            {
                yield return symbol.ViolatesServiceUsage();
            }

            if (type.IsRepository() && !symbol.AllowsRepositoryComposition())
            {
                yield return symbol.ViolatesRepositoryUsage(type);
            }
        }

        private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.RepositoriesShouldNotUseServicesRule);
        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(Rules.RepositoriesShouldNotDependOnRepositoriesRule, repository, type.DisplayName(), symbol.Name);
        }

        public static Diagnostic ViolatesInfrastructureSignature(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(
                Rules.RepositoriesShouldNotExposeInfrastructureSignaturesRule,
                repository,
                type.DisplayName(),
                symbol.Name);
        }
    }
}
